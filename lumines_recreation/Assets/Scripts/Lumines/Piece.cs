using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

namespace Lumines
{
    /// <summary>
    /// Lumines Piece,
    /// piece don't care how it generated
    /// it only set up by bytes value
    /// </summary>
    public class Piece : MonoBehaviour
    {
        private BoardField _boardField;
        
        [Header("Components")]
        public PieceVisual visual;

        [Header("Movement")]
        public float dasMoveDurationPerTile = 0.04f;
        public float normalMoveDuration = 0.12f;
        public bool useAnimCurve;
        public Ease ease = Ease.InOutQuad;
        public AnimationCurve moveAnimCurve = AnimationCurve.Linear(0, 0, 1, 1);
        
        [Header("Dropdown")]
        public float gravityDropDurationPerTile = 1f;
        public float hardDropDurationPerTile = 0.15f;
        
        // Piece type
        private readonly byte[,] piece = new byte[2, 2];
        
        private readonly Vector2 rotateCenter = new Vector2(0.5f, 0.5f);

        private List<PieceBlock> _pieceBlockList;
        
        // base data
        private byte _pieceByte;
        private int _rotateCount;
        // use center position, range is [0, 20]
        // standard spawn pos is (8, 11)
        private Vector2Int _gridPos;

        // Piece spawn time
        private float _spawnTime;
        // gravity and delay setting
        private float _gravity;
        // delay duration
        private float _gravityDelay;
        // piece landed and start counting lock delay 
        private float _lockStartTime;
        // land delay before locked
        private float _lockDelay;
        
        // piece movement status,
        // normally the rotate and move don't affect each other
        // in hard drop mode, you can't rotate or moving when piece start dropping
        // we can allow soft drop mode doing rotate when dropping 
        // todo : currently we implement the hard drop mode, implement the soft mode laterly
        private bool _isRotating;
        private bool _isHorizontalMoving;
        private bool _isHardDropping;
        private bool _isGravityDropping;
        // this status can be switched between _isDropping and itself 
        private bool _isLanded;

        private float _lastRotateTime;

        public bool IsHorizontalMoving => _isHorizontalMoving;
        public bool IsHardDropping => _isHardDropping;
        public bool IsRotating => _isRotating;
        
        public bool IsDasMoving => _isDasMoving;
        
        // TODO : temp
        public float rotateDuration = 0.2f;
        
        private Tween _moveTween;
        // DAS stuffs
        private float _lastDasStartTime;
        private bool _isDasMoving;
        private int _dasMoveCount;
        private int _dasDirection;

        // Dropdown stuffs
        private float _hardDropStartTime;
        
        private void Awake()
        {
            visual = GetComponent<PieceVisual>();
        }

        /// <summary>
        /// basic init
        /// </summary>
        /// <param name="boardField"></param>
        /// <param name="pieceValue"></param>
        public void Init(BoardField boardField, byte pieceValue)
        {
            _spawnTime = Time.time;
            _boardField = boardField;

            _pieceByte = pieceValue;
            piece[0, 0] = (byte)(_pieceByte & 1);
            piece[1, 0] = (byte)((_pieceByte >> 1) & 1);
            piece[0, 1] = (byte)((_pieceByte >> 2) & 1);
            piece[1, 1] = (byte)((_pieceByte >> 3) & 1);
            _rotateCount = 0;

            _gridPos = new Vector2Int(8, 11);
        }

        public void TryRotate(bool ccw)
        {
            var rotateColdDown = Time.time - _lastRotateTime < rotateDuration;
            if (_isRotating || rotateColdDown || _isHardDropping)
            {
                return;
            }
            
            _isRotating = true;
            _lastRotateTime = Time.time;
            
            if (ccw)
            {
                CcwRotatePiece();
            }
            else
            {
                CwRotatePiece();
            }
        }
        
        private void CwRotatePiece()
        {
            _rotateCount--;
            RotatePiece(-90f);
            
            var tween = visual.RotateAnimation(_rotateCount, -1);

            DOVirtual.DelayedCall(visual.RotateDuration, () => _isRotating = false);
        }

        private void CcwRotatePiece()
        {
            _rotateCount++;
            RotatePiece(90f);
            
            var tween = visual.RotateAnimation(_rotateCount);
            DOVirtual.DelayedCall(visual.RotateDuration, () => _isRotating = false);
        }

        private void RotatePiece(float angle)
        {
            var newPiece = new byte[2, 2];
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    var point = new Vector2(x, y);
                    // use rotate matrix to rotate the coord with the 0.5, 0.5 point
                    // create a coordinate based on 0.5, 0.5 point
                    point -= rotateCenter;
                    // then rotate the point
                    var targetCoord = Rotate2DMatrix(point, angle);
                    var newX = (int)targetCoord.x;
                    var newY = (int)targetCoord.y;
                    // cache calculate result
                    newPiece[newX, newY] = piece[x, y];
                }
            }

            // update the piece
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    piece[x, y] = newPiece[x, y];
                }
            }
        }

        public void MoveLeft()
        {
            // can't move when hard drop
            if (_isHorizontalMoving || _isHardDropping)
            {
                return;
            }
            
            // Debug.Log($"MoveLeft - targetGridPos: {_gridPos}");
            var targetGridPos = new Vector2Int(_gridPos.x - 1, _gridPos.y);
            if (_boardField.CheckPieceIsOutOfHorizontalBound(targetGridPos))
            {
                return;
            }
            // Debug.Log($"MoveLeft");

            _gridPos = targetGridPos;
            Move(true);
        }
        
        public void MoveRight()
        {
            // can't move when hard drop
            if (_isHorizontalMoving || _isHardDropping)
            {
                return;
            }
            
            // Debug.Log($"MoveRight - targetGridPos: {_gridPos}");
            var targetGridPos = new Vector2Int(_gridPos.x + 1, _gridPos.y);
            if (_boardField.CheckPieceIsOutOfHorizontalBound(targetGridPos))
            {
                return;
            }
            // Debug.Log($"MoveRight");

            _gridPos = targetGridPos;
            Move(false);
        }

        [Button]
        private void Move(bool isLeft)
        {
            // can't move when hard drop
            if (_isHardDropping)
            {
                return;
            }
            
            _isHorizontalMoving = true;
            
            _moveTween?.Kill();
            var targetPos = new Vector3(_gridPos.x, _gridPos.y, 0);
            
            var tween =
                transform.DOLocalMove(targetPos, normalMoveDuration);
            if (useAnimCurve)
            {
                tween.SetEase(moveAnimCurve);
            }
            else
            {
                tween.SetEase(ease);
            }

            tween.OnComplete(() => _isHorizontalMoving = false);
            
            _moveTween = tween;
        }

        public void StartHardDropping()
        {
            _isHardDropping = true;
            
            // pause horizontal moving and rotation
            // the animation still continue playing
            _isHorizontalMoving = false;
            _isRotating = false;
            
            _hardDropStartTime = Time.time;
        }
        
        private void Update()
        {
            GravityDropDown();
            SmoothDasMove();
        }

        /// <summary>
        /// Hard drop piece.
        /// starting hard drop will disable the horizontal move and rotation
        /// </summary>
        public void UpdateHardDropDown()
        {
            if (!_isHardDropping)
            {
                return;
            }
            
            var hardDropTimePass = Time.time - _hardDropStartTime;
            var frameMoveDistance = hardDropTimePass / hardDropDurationPerTile;
            
            var targetPosY = _boardField.PieceSpawnPos - frameMoveDistance;
            transform.localPosition = new Vector3(_gridPos.x, targetPosY, 0);
            
            // todo : Mathf.RoundToInt is not good check for update grid pos, update this later
            _gridPos = new Vector2Int(_gridPos.x, Mathf.RoundToInt(_gridPos.y));

            // check the piece is touch something then directly lock the piece
            var result = _boardField.CheckPieceIsTouchSomethingBottom(this);
            if (result)
            {
                LockPiece();
            }
        }
        
        /// <summary>
        /// the piece is dropping because of the game gravity
        /// </summary>
        private void GravityDropDown()
        {
            if (_isHardDropping)
            {
                return;
            }
            
            var timePassWhenSpawn = Time.time - _spawnTime;
            if (timePassWhenSpawn < _gravityDelay)
            {
                return;
            }

            var fallingTiles = timePassWhenSpawn / gravityDropDurationPerTile;
            var targetPosY = _boardField.PieceSpawnPos - fallingTiles;
            transform.localPosition = new Vector3(_gridPos.x, targetPosY, 0);
            
            // todo : Mathf.RoundToInt is not good check for update grid pos, update this later
            _gridPos = new Vector2Int(_gridPos.x, Mathf.RoundToInt(_gridPos.y));
            
            // check the piece is touch something and start counting the lock delay
        }

        private void StartCountingLockPiece()
        {
            _lockStartTime = Time.time;
        }

        private void UpdateTimerToLockPiece()
        {
            
        }
        
        /// <summary>
        /// lock piece itself,
        /// split it then notice the game to spawn next piece and get next pice
        /// </summary>
        private void LockPiece()
        {
            
            
            SplitPieceBlock();
        }
        
        /// <summary>
        /// split the piece after locked the piece
        /// </summary>
        private void SplitPieceBlock()
        {
            // reverse iteration and detach the block inside the piece
            for (var i = _pieceBlockList.Count - 1; i >= 0 ; i--)
            {
                var block = _pieceBlockList[i];
                block.transform.SetParent(_boardField.PieceContainer, true);
                block.StartFreeFall();
                _pieceBlockList.RemoveAt(i);
            }
        }

        public void StartDas(int direction)
        {
            Debug.Log($"StartDas - {direction}");
            _lastDasStartTime = Time.time;
            _isDasMoving = true;
            _dasMoveCount = 0;
            _dasDirection = direction;
        }

        public void EndDas()
        {
            Debug.Log("EndDas");
            _isDasMoving = false;
            _lastDasStartTime = -1f;
            _dasDirection = 0;
        }
        
        /// <summary>
        /// smooth das move by player holding move key time
        /// when the das is valid 
        /// </summary>
        private void SmoothDasMove()
        {
            if (!_isDasMoving)
            {
                return;
            }
            
            // calculate das move distance
            var dasInputDuration = Time.time - _lastDasStartTime;
            Debug.Log($"{dasInputDuration} | {_dasMoveCount * dasMoveDurationPerTile}");
            
            // if deltaDasTime is enough for one tile moving
            if (dasInputDuration >= _dasMoveCount * dasMoveDurationPerTile)
            {
                _dasMoveCount++;
                
                // calculate piece position by deltaDas value
                var targetPosX = _gridPos.x + _dasMoveCount * _dasDirection;
                var clampPos = _boardField.ClampPieceHorizontalPosition(new Vector2Int(targetPosX, _gridPos.y));
                
                // todo : currently its not smooth right now, we will do it in future lol
                transform.localPosition = new Vector3(clampPos.x, clampPos.y, 0);
                _gridPos = clampPos;
            }
        }

        #region Tools

        /// <summary>
        /// rotate 2D matrix. rotate point with coordinate origin.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="angle"></param> 
        private Vector2 Rotate2DMatrix(Vector2 point, float angle)
        {
            var rad = Mathf.Deg2Rad * angle;
            
            var x = point.x * Mathf.Cos(rad) + point.y * Mathf.Sin(rad);
            var y = -point.x * Mathf.Sin(rad) + point.y * Mathf.Cos(rad);
            
            return new Vector2(x, y);
        }

        #endregion
       
    }
}
