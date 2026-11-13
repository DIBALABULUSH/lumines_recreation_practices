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
        public float moveDuration = 0.2f;
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
        
        // TODO : temp
        public float rotateDuration = 0.2f;
        
        private Tween _moveTween;
        
        private void Awake()
        {
            visual = GetComponent<PieceVisual>();
        }

        public void Init()
        {
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
            
            visual.RotateAnimation(_rotateCount, -1);
        }

        public void CcwRotatePiece()
        {
            _rotateCount++;
            RotatePiece(90f);
            
            visual.RotateAnimation(_rotateCount);
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
            if (_boardField.CheckPieceIsOutOfBound())
            {
                return;
            }
            
            
        }
        
        public void MoveRight()
        {
            
        }

        [Button]
        private void Move(bool isLeft)
        {
            _moveTween?.Kill();
            
            var targetPos = new Vector3(_gridPos.x, _gridPos.y, 0);
            var tween =
                transform.DOMove(targetPos, moveDuration);
            if (useAnimCurve)
            {
                tween.SetEase(moveAnimCurve);
            }
            else
            {
                tween.SetEase(ease);
            }
            
            _moveTween = tween;
        }

        private void Update()
        {
            GravityMoveDown();
        }

        private void GravityMoveDown()
        {
            
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
