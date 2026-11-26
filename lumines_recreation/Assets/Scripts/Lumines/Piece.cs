using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Tools;
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

        [Header("PieceSize")]
        public Vector2Int pieceSize = new Vector2Int(2, 2);
            
        private readonly Vector2 _rotateCenter = new Vector2(0.5f, 0.5f);
        
        [Header("Movement")]
        public float dasMoveDurationPerTile = 0.04f;
        public float normalMoveDuration = 0.12f;
        public float moveSmoothValue = 0.1f;
        public bool useAnimCurve;
        public Ease ease = Ease.InOutQuad;
        public AnimationCurve moveAnimCurve = AnimationCurve.Linear(0, 0, 1, 1);
        
        [Header("Dropdown")]
        public float gravityDropDurationPerTile = 1f;
        public float hardDropDurationPerTile = 0.015f;
        
        // Piece type
        // private readonly byte[] _piece = new byte[4];
        
        private int PieceWidth => pieceSize.x;
        private int PieceHeight => pieceSize.y;

        [SerializeField] private List<PieceBlock> _pieceBlockList;
        
        // base data
        private int _rotateCount;
        // use center position, range is [0, 20]
        // standard spawn pos is (8, 11),
        // and this position is align to the grid system
        private Vector2Int _gridPieceCenterPos;

        // Piece spawn time
        private float _spawnTime = -1f;
        // Piece picked by game system
        private float _pickUpTime = -1f;
        // gravity and delay setting
        private float _gravity;
        // delay duration
        [SerializeField] private float _gravityDelay = 0.5f;
        // piece landed and start counting lock delay 
        private float _lockStartCountingTime = -1f;
        // land delay before locked
        [SerializeField] private float _lockDelay;
        private bool _isLocked;
        
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

        public Vector2Int GridPieceCenterPos => _gridPieceCenterPos;
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
        public event Action<Piece> OnPieceLocked;
        
        private void Awake()
        {
            visual = GetComponent<PieceVisual>();

            if (_pieceBlockList == null)
            {
                _pieceBlockList = new List<PieceBlock>();
            }
            _pieceBlockList.Clear();

            var pieceBlocks = GetComponentsInChildren<PieceBlock>();

            // todo: we don't other shapes piece, now only handling the 2x2 piece
            //      we need a procedural solution for dynamic piece shape
            for (int y = 0; y < pieceSize.y; y++)
            {
                for (int x = 0; x < pieceSize.x; x++)
                {
                    var index =  y * pieceSize.x + x;
                    var pieceBlock = pieceBlocks[index];

                    var localGridPos = new Vector2Int(x, y);
                    pieceBlock.SetLocalGridPos(localGridPos);
                    _pieceBlockList.Add(pieceBlock);
                }
            }
            
            _lockStartCountingTime = -1f;
        }

        /// <summary>
        /// basic init
        /// </summary>
        /// <param name="boardField"></param>
        /// <param name="byteArray"></param>
        public void Init(BoardField boardField, byte[] byteArray)
        {
            _spawnTime = Time.time;
            _boardField = boardField;
            
            _rotateCount = 0;

            _gridPieceCenterPos = new Vector2Int(8, 11);

            var count = 0;
            foreach (var pieceBlock in _pieceBlockList)
            {
                pieceBlock.SetBoardField(_boardField);
                var byteValue = byteArray[count++];
                pieceBlock.UpdateBlock(byteValue);
            }
            
            visual.Init();
        }
        
        public void PickUp()
        {
            _pickUpTime = Time.time;
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
            for (var i = 0; i < _pieceBlockList.Count; i++)
            {
                var pieceBlock = _pieceBlockList[i];
                var localPos = (Vector2)pieceBlock.LocalGridPos;
                // use rotate matrix to rotate the coord with the rotate center position
                // create a coordinate based on rotate center position
                localPos -= _rotateCenter;
                // then rotate the point
                var targetCoord = Rotate2DMatrix(localPos, angle);
                // add back the adjustment
                var newX = targetCoord.x + _rotateCenter.x;
                var newY = targetCoord.y + _rotateCenter.y;
                
                var newLocalGridPos = new Vector2Int(Mathf.RoundToInt(newX), Mathf.RoundToInt(newY));
                pieceBlock.SetLocalGridPos(newLocalGridPos);

                // Debug.Log($"result: {targetCoord} old pos: {x}, {y} new pos: {newX}, {newY}");
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
            var targetGridPos = new Vector2Int(_gridPieceCenterPos.x - 1, _gridPieceCenterPos.y);
            if (_boardField.CheckPieceIsOutOfHorizontalBound(targetGridPos))
            {
                return;
            }
            // Debug.Log($"MoveLeft");

            _gridPieceCenterPos = targetGridPos;
            // MoveHorizontally(true);
        }
        
        public void MoveRight()
        {
            // can't move when hard drop
            if (_isHorizontalMoving || _isHardDropping)
            {
                return;
            }
            
            // Debug.Log($"MoveRight - targetGridPos: {_gridPos}");
            var targetGridPos = new Vector2Int(_gridPieceCenterPos.x + 1, _gridPieceCenterPos.y);
            if (_boardField.CheckPieceIsOutOfHorizontalBound(targetGridPos))
            {
                return;
            }
            // Debug.Log($"MoveRight");

            _gridPieceCenterPos = targetGridPos;
            // MoveHorizontally(false);
        }

        [Button]
        private void MoveHorizontally(bool isLeft)
        {
            // can't move when hard drop
            if (_isHardDropping)
            {
                return;
            }
            
            _isHorizontalMoving = true;
            
            _moveTween?.Kill();
            var targetPos = new Vector3(_gridPieceCenterPos.x, _gridPieceCenterPos.y, 0);
            
            var tween =
                transform.DOLocalMoveX(targetPos.x, normalMoveDuration);
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
            UpdateHardDropDown();
            UpdateGravityDropDown();
            
            UpdateSmoothHorizontalMove();
            
            UpdateTimerToLockPiece();
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
            
            var isReachSomething = _boardField.CheckIsThereBlockUnderThePiece(this);
            if (isReachSomething)
            {
                return;
            }
            
            var hardDropTimePass = Time.time - _hardDropStartTime;
            
            // todo : directly using deltaTime is not accurate for calculate or rendering piece position
            //          check this later, so as the gravity move down
            var frameMoveDistance = Time.deltaTime / hardDropDurationPerTile;
            
            // old calculate way
            // var targetPosY = _boardField.PieceSpawnPos - frameMoveDistance;
            var targetPosY = transform.localPosition.y - frameMoveDistance;
            transform.localPosition = new Vector3(_gridPieceCenterPos.x, targetPosY, 0);
            
            _gridPieceCenterPos = new Vector2Int(_gridPieceCenterPos.x, Mathf.RoundToInt(targetPosY));

            // check the piece is touch something then directly lock the piece
            var result = _boardField.CheckIsThereBlockUnderThePiece(this);
            // Debug.Log(result);
            if (result)
            {
                LockPiece();
            }
        }
        
        /// <summary>
        /// the piece is dropping because of the game gravity
        /// </summary>
        private void UpdateGravityDropDown()
        {
            if (_isHardDropping)
            {
                return;
            }
            
            // check the piece is touch something and start counting the lock delay
            var reachGround = _boardField.CheckIsThereBlockUnderThePiece(this);
            if (reachGround)
            {
                // already reach the ground, set piece into right ground position
                // TOdo : comment this because we need to wait the lock delay to set up the piece
                //      now we just don't continue move down
                // transform.localPosition = new Vector3(transform.localPosition.x, 1f, 0);
                return;
            }

            // reset lock counter when drop down by gravity
            _lockStartCountingTime = -1f;
            
            // start gravity drop down check
            var timePassSinceSpawn = Time.time - _pickUpTime;
            if (timePassSinceSpawn < _gravityDelay)
            {
                return;
            }

            var deltaFallingTiles = Time.deltaTime / gravityDropDurationPerTile;
            // var targetPosY = _boardField.PieceSpawnPos - fallingTiles;
            var targetPosY = transform.localPosition.y - deltaFallingTiles;
            transform.localPosition = new Vector3(transform.localPosition.x, targetPosY, 0);
            
            // todo : Mathf.RoundToInt is not good check for update grid pos, update this later
            _gridPieceCenterPos = new Vector2Int(_gridPieceCenterPos.x, Mathf.CeilToInt(targetPosY));
            
            // Debug.Log("Gravity" + _gridPos);
            
            var reachGroundThisFrame = _boardField.CheckIsThereBlockUnderThePiece(this);
            if (reachGroundThisFrame)
            {
                // TODO : Maybe not accurate set in this phase
                StartCountingLockPiece();
            }
        }

        private void StartCountingLockPiece()
        {
            _lockStartCountingTime = Time.time;
            Debug.LogWarning("StartCountingLockPiece");
        }

        private void UpdateTimerToLockPiece()
        {
            if (_isLocked || _lockStartCountingTime < 0f)
            {
                return;
            }
            
            var timePassed = Time.time - _lockStartCountingTime;
            if (timePassed < _lockDelay)
            {
                // Debug.LogError("counting lock delay");
                return;
            }

            LockPiece();
        }
        
        /// <summary>
        /// lock piece itself,
        /// split it then notice the game to spawn next piece and get next pice
        /// </summary>
        private void LockPiece()
        {
            _isLocked = true;
            
            _isHardDropping = false;
            _isDasMoving = false;
            _isHorizontalMoving = false;
            _isRotating = false;
            _isGravityDropping = false;
            
            // _isLanded = true;
            
            // split piece block
            SplitPieceBlock();

            OnPieceLocked?.Invoke(this);
            
            // all done destroy self
            Destroy(gameObject);
        }
        
        /// <summary>
        /// split the piece after locked the piece
        /// </summary>
        private void SplitPieceBlock()
        {
            D.Info("=== SplitPieceBlock ===", Color.cyan);
            
            // reverse iteration and detach the block inside the piece
            for (var i = 0; i < _pieceBlockList.Count; i++)
            {
                var block = _pieceBlockList[i];
                block.transform.SetParent(_boardField.PieceContainer, true);
                var targetPos =
                    _gridPieceCenterPos - Vector2Int.one + block.LocalGridPos;
                // Debug.Log($"{_gridPieceCenterPos} | {localGridPos}");
                
                // well start free fall must be executed earlier than set grid pos
                // otherwise the boolean will be reset back to start and execute one more Update()
                block.StartFreeFall();
                block.SetGridPos(targetPos);
            }
            
            _pieceBlockList.Clear();
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
        /// smooth horizontal move by player holding move key time
        /// when the das is valid 
        /// </summary>
        private void UpdateSmoothHorizontalMove()
        {
            if (_isHardDropping)
            {
                return;
            }
            
            if (!_isDasMoving)
            {
                // rough smooth movement
                var targetPosX = Mathf.Lerp(transform.localPosition.x, _gridPieceCenterPos.x, moveSmoothValue);
                transform.localPosition = new Vector3(targetPosX, transform.localPosition.y, 0);
                
                return;
            }
            
            // calculate das move distance
            var dasInputDuration = Time.time - _lastDasStartTime;
            // Debug.Log($"{dasInputDuration} | {_dasMoveCount * dasMoveDurationPerTile}");
            
            // if deltaDasTime is enough for one tile moving
            if (dasInputDuration >= _dasMoveCount * dasMoveDurationPerTile)
            {
                _dasMoveCount++;
                
                // calculate piece position by deltaDas value
                var targetPosX = _gridPieceCenterPos.x + _dasMoveCount * _dasDirection;
                var clampPos = _boardField.ClampPieceHorizontalPosition(new Vector2Int(targetPosX, _gridPieceCenterPos.y));
                
                // step moving solution: currently its not smooth right now, we will do it in future lol
                // transform.localPosition = new Vector3(clampPos.x, clampPos.y, 0);

                _gridPieceCenterPos = clampPos;
            }
            
            // rough smooth update piece move while DASing
            var finalPosX = Mathf.Lerp(transform.localPosition.x, _gridPieceCenterPos.x, moveSmoothValue);
            transform.localPosition = new Vector3(finalPosX, transform.localPosition.y, 0);
        }

        #region Tools

        /// <summary>
        /// rotate 2D matrix. rotate point with coordinate origin.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="angle"></param>
        [Button]
        private Vector2 Rotate2DMatrix(Vector2 point, float angle)
        {
            var rad = Mathf.Deg2Rad * angle;
            
            var x = point.x * Mathf.Cos(rad) - point.y * Mathf.Sin(rad);
            var y = point.x * Mathf.Sin(rad) + point.y * Mathf.Cos(rad);
            
            // Debug.Log($"{x} | {y}");
            
            return new Vector2(x, y);
        }

        #endregion
    }
}
