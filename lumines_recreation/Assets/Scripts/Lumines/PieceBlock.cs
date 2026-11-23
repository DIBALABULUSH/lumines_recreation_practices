using System;
using UnityEngine;

namespace Lumines
{
    /// <summary>
    /// Piece block which split piece when it is dropping on the field
    /// </summary>
    public class PieceBlock : MonoBehaviour
    {
        private float _hardDropSpeed;
        private float _freeFallSpeed;
        private Vector2Int _gridPosition;
        
        private BoardField _boardField;

        // the block drop down when the blocks under this block are clear
        private bool _isFreeFalling;
        
        public void SetBoardField(BoardField boardField)
        {
            _boardField = boardField;
        }
        
        public void StartFreeFall()
        {
            _isFreeFalling = true;
        }
        
        private void Update()
        {
            FreeFalling();
        }

        private void FreeFalling()
        {
            if (!_isFreeFalling)
            {
                return;
            }

            // TODO : free falling code logic
            
            CheckAndLockBlock();
        }

        private void CheckAndLockBlock()
        {
            var columnTopEmptyPosition =
                _boardField.GetBoardFieldColumnTopEmptyPosition(_gridPosition.x);
            if (columnTopEmptyPosition < 0)
            {
                Debug.LogWarning("Column Top Position is negative");
                return;
            }

            var targetGridPosition = columnTopEmptyPosition;
            if (transform.localPosition.y > targetGridPosition)
            {
                return;
            }

            LockBlock();
            
            // 0.5 is for set the block in the grid center
            transform.localPosition = 
                new Vector3(transform.localPosition.x, targetGridPosition + 0.5f, transform.localPosition.z);
        }

        private void LockBlock()
        {
            _isFreeFalling = false;
            
            _boardField.PlaceBlockOnField(this, _gridPosition);
            
            Debug.Log($"Lock block on {_gridPosition}");
        }

        public void SetGridPos(Vector2Int gridPosition)
        {
            _gridPosition = gridPosition;
            Debug.Log($"Set block grid pos with {_gridPosition}");
            
            CheckAndLockBlock();
        }
    }
}