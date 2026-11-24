using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using Tools;
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
        // toDO : use some config
        public int BoardWidth => 16;
        public int BoardHeight => 10;

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
                D.Info($"Place block on {gridPos}", Color.green);
                _pieceBlockListArray[col][row] = block;
                return true;
            }
            else
            { 
                Debug.LogWarning($"Target position {gridPos} already have an existing block");
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
            var pieceBottomBlockPosY = piece.transform.localPosition.y;
            // grid piece center pos is [1, 15], minus 1 to get the 0 position grid block
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

        private void OnDrawGizmos()
        {
            if (_pieceBlockListArray == null)
            {
                return;
            }
            
            for (int x = 0; x < _pieceBlockListArray.Length; x++)
            {
                var blockList = _pieceBlockListArray[x];
                for (int y = 0; y < blockList.Count; y++)
                {
                    var block = blockList[y];
                    if (block == null)
                    {
                        Gizmos.color = Color.red;
                        var pos = new Vector3(x, y) + Vector3.one * 0.5f - new Vector3(8, 5, 2f);
                        Gizmos.DrawCube(pos, Vector3.one * 0.3f);
                        continue;
                    }
                    
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(block.transform.position - Vector3.forward * 2f, Vector3.one * 0.3f);
                }
            }
        }
    }
}
