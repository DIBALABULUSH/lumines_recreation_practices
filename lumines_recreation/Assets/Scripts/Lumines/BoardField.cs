using System;
using System.Linq;
using Tools;
using UnityEngine;

namespace Lumines
{
    /// <summary>
    /// 16 x 10 board field for placing pieces
    /// </summary>
    public class BoardField : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private MergedPieceBlock mergedPieceBlockPrefab;
        
        [Header("Components")]
        [SerializeField] private Transform pieceSpawnRoot;
        [SerializeField] private Transform pieceContainer;
        
        private ArrayGrid<PieceBlock> _pieceBlockArrayGrid;
        
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
            _pieceBlockArrayGrid ??= new ArrayGrid<PieceBlock>(_boardSize.x, _boardSize.y);
        }

        public void ClearBoard()
        {
            foreach (var pieceBlock in _pieceBlockArrayGrid)
            {
                if (pieceBlock == null)
                {
                    continue;
                }
                Destroy(pieceBlock.gameObject);
            }
            _pieceBlockArrayGrid.Clear();
        }
        
        public bool PlaceBlockOnField(PieceBlock block, Vector2Int gridPos)
        {
            var col = gridPos.x;
            var row = gridPos.y;
            
            // out array bound check
            if (col < 0 || col >= _pieceBlockArrayGrid.Length)
            {
                Debug.Log($"Invalid grid position: {gridPos} for placing block on field");
                return false;
            }
            
            // Debug.Log($"Get block queue on col[{col}]");
            
            // get the column in board field
            var blockList = _pieceBlockArrayGrid.GetColumnItems(col);

            var targetBlock = blockList[row];
            if (targetBlock == null)
            {
                D.Info($"Place block on {gridPos}", Color.green);
                _pieceBlockArrayGrid.SetItemAt(col, row, block);
                
                // if we successfully place a block on the field
                // we can check the blocks surround the block we just placed
                CheckAndMerge2X2CubeAtPosition(gridPos, block.BlockType, new Vector2Int(2, 2));
                
                return true;
            }
            
            Debug.LogWarning($"Target position {gridPos} already have an existing block");
            return false;
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

        public int GetColumnTopEmptyGridPosition(int col)
        {
            // out array bound check
            if (col < 0 || col >= _pieceBlockArrayGrid.Length)
            {
                return -1;
            }
            
            // Debug.Log($"Get block queue on col[{col}]");
            
            // get the column in board field
            var blockList = _pieceBlockArrayGrid.GetColumnItems(col);

            for (var i = 0; i < blockList.Length; i++)
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
            var leftColTop = GetColumnTopEmptyGridPosition(piece.GridPieceCenterPos.x - 1);
            var rightColTop = GetColumnTopEmptyGridPosition(piece.GridPieceCenterPos.x);
            
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

        private Vector2Int[] checkOffsetArray = new Vector2Int[]
        {
            new Vector2Int(-1, -1), // LB
            new Vector2Int(-1, 0), // LT
            new Vector2Int(0, -1), // RB
            new Vector2Int(0, 0), // RT
        };
        
        // todo : make it can check NxM rect at position
        public void CheckAndMerge2X2CubeAtPosition(
            Vector2Int blockPos, byte blockType, Vector2Int mergedSize)
        {
            // check four position
            for (int i = 0; i < 4; i++)
            {
                var targetPos = blockPos + checkOffsetArray[i];
                if((uint)targetPos.x >= _boardSize.x || (uint)targetPos.y >= _boardSize.y)
                {
                    continue;
                }

                var blocks = 
                    _pieceBlockArrayGrid.GetFullRectItems(targetPos.x, targetPos.y, 2, 2);
                var canMerge = blocks.All(block => block.BlockType == blockType);
                
                if (!canMerge)
                {
                    continue;
                }
                
                // todo : recycle block
                // hide piece block and spawn merged block
                foreach (var block in blocks)
                {
                    block.enabled = false;
                    block.gameObject.SetActive(false);
                }
                
                // spawn merged block
                var mergedPieceBlock = Instantiate(mergedPieceBlockPrefab, pieceContainer);
                var centerPos = blockPos.ToVec2() - Vector2.one * mergedSize * 0.5f;
                mergedPieceBlock.Init(mergedSize, centerPos);
            }
        }
        
        private void OnDrawGizmos()
        {
            if (_pieceBlockArrayGrid == null)
            {
                return;
            }

            var count = 0;
            foreach (var block in _pieceBlockArrayGrid)
            {
                if (block == null)
                {
                    var coord = _pieceBlockArrayGrid.GetGridCoord(count);
                    Gizmos.color = Color.red;
                    var pos = new Vector3(coord.x, coord.y) + Vector3.one * 0.5f - new Vector3(8, 5, 2f);
                    Gizmos.DrawCube(pos, Vector3.one * 0.3f);
                    continue;
                }
                    
                Gizmos.color = Color.green;
                Gizmos.DrawCube(block.transform.position - Vector3.forward * 2f, Vector3.one * 0.3f);
            }
        }
    }
}
