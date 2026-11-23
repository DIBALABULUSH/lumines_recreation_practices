using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lumines
{
    /// <summary>
    /// 16 x 10 board field for placing pieces
    /// </summary>
    public class BoardField : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform pieceSpawnRoot;
        [SerializeField] private Transform pieceContainer;
        
        private Queue<PieceBlock>[] _pieceBlockQueueArray;
        
        private Vector2Int _boardSize = new Vector2Int(16, 10);
        public Vector2Int BoardSize => _boardSize;
        public Vector3 PieceSpawnPosition => pieceSpawnRoot.position;
        public Transform PieceContainer => pieceContainer;

        public float PieceSpawnPos => 11f;

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            _pieceBlockQueueArray ??= new Queue<PieceBlock>[_boardSize.x];

            for (var i = 0; i < _pieceBlockQueueArray.Length; i++)
            {
                _pieceBlockQueueArray[i] ??= new Queue<PieceBlock>();
                _pieceBlockQueueArray[i]?.Clear();
            }
        }

        public bool PlaceBlockOnField(Vector2Int gridPos)
        {
            var col = gridPos.x;
            var row = gridPos.y;
            
            // out array bound check
            if (col < 0 || col >= _pieceBlockQueueArray.Length)
            {
                Debug.Log($"Invalid grid position: {gridPos} for placing block on field");
                return false;
            }
            
            // Debug.Log($"Get block queue on col[{col}]");
            
            // get the column in board field
            var blockQueue = _pieceBlockQueueArray[col];
            var topBlockPosInColumn = Math.Clamp(blockQueue.Count - 1, 0, Math.Max(0, blockQueue.Count - 1));

            for (var i = 0; i < blockQueue.Count; i++)
            {
                
            }
            
            return true;
        }
        
        /// <summary>
        /// check the standard 2x2 piece is out of bound, only check horizontally.
        /// </summary>
        /// <returns></returns>
        public bool CheckPieceIsOutOfHorizontalBound(Vector2Int piecePos)
        {
            return piecePos.x >= _boardSize.x || piecePos.x <= 0;
        }
        
        public Vector2Int ClampPieceHorizontalPosition(Vector2Int piecePos)
        {
            var x = Math.Clamp(piecePos.x, 1, _boardSize.x - 1);
            
            return new Vector2Int(x, piecePos.y);
        }

        public int GetBoardFieldColumnTopPosition(int col)
        {
            // out array bound check
            if (col < 0 || col >= _pieceBlockQueueArray.Length)
            {
                return -1;
            }
            
            // Debug.Log($"Get block queue on col[{col}]");
            
            // get the column in board field
            var blockQueue = _pieceBlockQueueArray[col];
            var topBlockPosInColumn = Math.Clamp(blockQueue.Count - 1, 0, Math.Max(0, blockQueue.Count - 1));
            
            return topBlockPosInColumn;
        }

        public bool CheckIsTherePieceUnderThePiece(Piece piece)
        {
            // we can directly check the piece under the piece by _gridPos
            // but considering the dynamic freefall and piece attachment
            // we will most likely using some dynamic method to check the piece
            
            
            // check piece bottom block is touch other blocks on the board field
            var pieceBottomBlockPosY = piece.GridPos.y - 1;
            var leftColTop = GetBoardFieldColumnTopPosition(piece.GridPos.x - 1);
            var rightColTop = GetBoardFieldColumnTopPosition(piece.GridPos.x);
            if (pieceBottomBlockPosY - leftColTop <= 1 || pieceBottomBlockPosY - rightColTop <= 1)
            {
                return true;
            }
            
            // check ground, 1 is the lowest position for the standard lumines piece
            if (piece.GridPos.y <= 1)
            {
                return true;
            }
            
            return false;
        }
    }
}
