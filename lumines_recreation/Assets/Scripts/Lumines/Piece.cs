using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

namespace Lumines
{
    /// <summary>
    /// Lumines Piece
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
        
        // Piece type
        private readonly byte[,] piece = new byte[2, 2];
        
        private readonly Vector2 rotateCenter = new Vector2(0.5f, 0.5f);
        
        // base data
        private byte _piece;
        private int _rotateCount;
        // use center position, range is [0, 20]
        // standard spawn pos is (8, 11)
        private Vector2Int _gridPos;

        // gravity and delay setting
        private float _gravity;
        private float _gravityDelay;
        private float _lockedDelay;
        
        // piece movement status,
        // normally the rotate and move don't affect each other
        // in hard drop mode, you can't rotate or moving when piece start dropping
        // we can allow soft drop mode doing rotate when dropping 
        // todo : currently we implement the hard drop mode, implement the soft mode laterly
        private bool _isRotating;
        private bool _isMoving;
        private bool _isDropping;

        private float _lastRotateTime;

        public bool IsMoving => _isMoving;
        public bool IsDropping => _isDropping;
        public bool IsRotating => _isRotating;
        
        public bool IsDasMoving => _isDasMoving;
        
        // TODO : temp
        public float rotateDuration = 0.2f;
        
        private Tween _moveTween;
        private float _lastDasStartTime;
        private bool _isDasMoving;
        private int _dasMoveCount;
        private int _dasDirection;

        private void Awake()
        {
            visual = GetComponent<PieceVisual>();
        }

        /// <summary>
        /// basic init
        /// </summary>
        /// <param name="boardField"></param>
        public void Init(BoardField boardField)
        {
            _boardField = boardField;
            
            _piece = RandomByteLast4Bits(0);
            piece[0, 0] = (byte)(_piece & 1);
            piece[1, 0] = (byte)((_piece >> 1) & 1);
            piece[0, 1] = (byte)((_piece >> 2) & 1);
            piece[1, 1] = (byte)((_piece >> 3) & 1);
            _rotateCount = 0;

            _gridPos = new Vector2Int(8, 11);
        }

        public void TryRotate(bool ccw)
        {
            if (_isRotating || Time.time - _lastRotateTime < rotateDuration)
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
        
        public void CwRotatePiece()
        {
            _rotateCount--;
            RotatePiece(-90f);
            
            var tween = visual.RotateAnimation(_rotateCount, -1);

            DOVirtual.DelayedCall(visual.RotateDuration, () => _isRotating = false);
        }

        public void CcwRotatePiece()
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
            if (_isMoving)
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
            if (_isMoving)
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
            _isMoving = true;
            
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

            tween.OnComplete(() => _isMoving = false);
            
            _moveTween = tween;
        }
        
        private void Update()
        {
            GravityMoveDown();
            
            SmoothDasMove();
        }

        private void GravityMoveDown()
        {
            
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

        private byte RandomByteLast4Bits(byte value)
        {
            // gen random 0 ~ 15 number
            int randomBits = new Random().Next(0, 16);

            // keep high 4 bits, random low 4 bits
            value = (byte)((value & 0xF0) | randomBits);

            return value;
        }

        #endregion
       
    }
}
