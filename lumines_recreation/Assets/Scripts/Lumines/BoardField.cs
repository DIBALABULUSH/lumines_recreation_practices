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

        public void Init()
        {
            _pieceBlockQueueArray = new Queue<PieceBlock>[_boardSize.x];
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
            if (col < 0 || col >= _boardSize.x)
            {
                return -1;
            }
            
            // get the column in board field
            var blockQueue = _pieceBlockQueueArray[col];
            var topBlockPosInColumn = Math.Clamp(blockQueue.Count - 1, 0, blockQueue.Count - 1);
            
            return topBlockPosInColumn;
        }

        public bool CheckPieceIsTouchSomethingBottom(Piece piece)
        {
            
            
            return true;
        }
    }
}
