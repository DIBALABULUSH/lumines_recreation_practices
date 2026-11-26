using System;
using Sirenix.OdinInspector;
using Tools;
using UnityEngine;

namespace Lumines
{
    /// <summary>
    /// Piece block which split piece when it is dropping on the field
    /// </summary>
    public class PieceBlock : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private PieceBlockVisual visual;
        
        [SerializeField, Min(0.01f)] private float freeFallTimePerTile;
        [SerializeField, ReadOnly] private Vector2Int _gridPosition;
        [SerializeField, ReadOnly] private Vector2Int _localGridPosition;
        
        private BoardField _boardField;

        // 0 is type A, 1 is type B,
        // 2 and other numbers are other types
        private byte _blockType;
        
        // the block drop down when the blocks under this block are clear
        private bool _isFreeFalling;
        private bool _isLocked;

        private bool _isReadyToClear;
        
        public Vector2Int LocalGridPos => _localGridPosition;
        public byte BlockType => _blockType;

        private void Awake()
        {
            visual = GetComponent<PieceBlockVisual>();
        }

        public void SetBoardField(BoardField boardField)
        {
            _boardField = boardField;
        }
        
        public void StartFreeFall()
        {
            _isFreeFalling = true;
            _isLocked = false;
            // D.Info($"StartFreeFall {_isFreeFalling} | {_isLocked}", Color.red);
        }
        
        public void SetGridPos(Vector2Int gridPosition)
        {
            _gridPosition = gridPosition;
            Debug.Log($"Set block grid pos with {_gridPosition} | local position {transform.localPosition}");
            
            CheckAndLockBlock();
        }
        
        private void Update()
        {
            FreeFalling();
        }

        private void FreeFalling()
        {
            if (!_isFreeFalling || _isLocked)
            {
                return;
            }
            
            // get the target column top empty grid position before moving
            var columnTopEmptyPosition =
                _boardField.GetColumnTopEmptyGridPosition(_gridPosition.x);
            
            var deltaPosY = Time.deltaTime / freeFallTimePerTile;
            transform.localPosition -= Vector3.up * deltaPosY;
            
            // convert continue position into grid position
            // use round for converting, DO NOT use floor, it will cause occupy order issue
            var pos = transform.localPosition - Vector3.one * 0.5f;
            var newGridPosX =
                Mathf.Clamp(Mathf.RoundToInt(pos.x), 0, _boardField.BoardWidth - 1);
            // check after free fall the block is not beyond the top empty grid position
            var newGridPosY= 
                Mathf.Clamp(Mathf.RoundToInt(pos.y), columnTopEmptyPosition, _boardField.BoardHeight - 1);
            _gridPosition = new Vector2Int(newGridPosX, newGridPosY);
            
            CheckAndLockBlock();
        }

        private void CheckAndLockBlock()
        {
            if (_isLocked)
            {
                return;
            }
            
            var columnTopEmptyPosition =
                _boardField.GetColumnTopEmptyGridPosition(_gridPosition.x);
            
            // todo : block over the board directly disappear
            
            if (columnTopEmptyPosition < 0)
            {
                Debug.LogWarning("Column Top Position is negative");
                return;
            }

            // local position Y minus 0.5f for real grid position
            // block will have 0.5f Y offset to adjust to the grid center
            var targetGridPosition = columnTopEmptyPosition;
            if (transform.localPosition.y - 0.5f > targetGridPosition)
            {
                // Debug.LogError($"not {transform.localPosition.y} | {targetGridPosition}");
                return;
            }

            LockBlock();
        }

        private void LockBlock()
        {
            _isFreeFalling = false;
            _isLocked = true;
            
            _boardField.PlaceBlockOnField(this, _gridPosition);
            
            // 0.5 is for set the block in the grid center
            var lockPosition =
                new Vector3(_gridPosition.x + 0.5f, _gridPosition.y + 0.5f, transform.localPosition.z);
            transform.localPosition = lockPosition;
            D.Info($"{lockPosition}", Color.green); 
        }

        public void SetLocalGridPos(Vector2Int localGridPosition)
        {
            _localGridPosition = localGridPosition;
        }

        public void PrepareToClear()
        {
            _isReadyToClear = true;
            visual.ReadyToClear();
        }
    }
}