using System;
using UnityEngine;
using Random = System.Random;

namespace Lumines
{
    /// <summary>
    /// Lumines Piece
    /// </summary>
    public class Piece : MonoBehaviour
    {
        public PieceVisual visual;
        
        // Piece type
        private readonly byte[,] piece = new byte[2, 2];
        
        private readonly Vector2 rotateCenter = new Vector2(0.5f, 0.5f);
        
        // base data
        private byte _piece;
        private int _rotateCount;
        
        // piece status, these status can exist all at once
        private bool _isRotating;
        private bool _isDropping;
        private bool _isMoving;

        private float _lastRotateTime;

        // TODO : temp
        public float rotateDuration = 0.2f;
        
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

        public void RotatePiece(float angle)
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

        /// <summary>
        /// Rotate 2D matrix. Rotate point with coordinate origin.
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
            // 生成 0～15 的随机数
            int randomBits = new Random().Next(0, 16);

            // 保留高4位，随机替换低4位
            value = (byte)((value & 0xF0) | randomBits);

            return value;
        }
    }
}
