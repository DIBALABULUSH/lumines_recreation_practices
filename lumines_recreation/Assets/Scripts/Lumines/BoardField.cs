using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
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
        
        private List<PieceBlock>[] _pieceBlockListArray;
        
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
            _pieceBlockListArray ??= new List<PieceBlock>[_boardSize.x];

            for (var i = 0; i < _pieceBlockListArray.Length; i++)
            {
                _pieceBlockListArray[i] ??= new List<PieceBlock>();
                _pieceBlockListArray[i]?.Clear();
                
                _pieceBlockListArray[i].SetLength(10);
            } 
        }

        public bool PlaceBlockOnField(PieceBlock block, Vector2Int gridPos)
        {
            var col = gridPos.x;
            var row = gridPos.y;
            
            // out array bound check
            if (col < 0 || col >= _pieceBlockListArray.Length)
            {
                Debug.Log($"Invalid grid position: {gridPos} for placing block on field");
                return false;
            }
            
            // Debug.Log($"Get block queue on col[{col}]");
            
            // get the column in board field
            var blockList = _pieceBlockListArray[col];

            var targetBlock = blockList[row];
            if (targetBlock == null)
            {
                Debug.Log($"Place block on {gridPos}");
                blockList[row] = block;
                return true;
            }
            else
            { 
                Debug.LogWarning("Target position already have an existing block");
                return false;
            }
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

        public int GetBoardFieldColumnTopEmptyPosition(int col)
        {
            // out array bound check
            if (col < 0 || col >= _pieceBlockListArray.Length)
            {
                return -1;
            }
            
            // Debug.Log($"Get block queue on col[{col}]");
            
            // get the column in board field
            var blockList = _pieceBlockListArray[col];

            for (var i = 0; i < blockList.Count; i++)
            {
                var block = blockList[i];
                if (block == null)
                {
                    return i;
                }
            }
            
            return -1;
        }

        public bool CheckIsThereBlockUnderThePiece(Piece piece)
        {
            // we can directly check the piece under the piece by _gridPos
            // but considering the dynamic freefall and piece attachment
            // we will most likely using some dynamic method to check the piece
            
            
            // check piece bottom block is touch other blocks on the board field
            var pieceBottomBlockPosY = piece.transform.localPosition.y - 1;
            var leftColTop = GetBoardFieldColumnTopEmptyPosition(piece.GridPieceCenterPos.x - 1);
            var rightColTop = GetBoardFieldColumnTopEmptyPosition(piece.GridPieceCenterPos.x);
            
            if (pieceBottomBlockPosY - leftColTop <= 1 || pieceBottomBlockPosY - rightColTop <= 1)
            {
                return true;
            }
            
            // check ground, 1 is the lowest position Y for the standard lumines piece(use center position)
            if (piece.transform.localPosition.y <= 1f)
            {
                return true;
            }
            
            return false;
        }
    }
}
