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
        // todo : maybe we don't need this
        private bool _isHardDrop;
        
        public void StartFreeFall()
        {
            _isFreeFalling = true;
        }
        
        private void Update()
        {
            FreeFalling();
        }
        
        private void HardDropping()
        {
            if (!_isHardDrop)
            {
                return;
            }
            
            LockBlock();
        }

        private void FreeFalling()
        {
            if (!_isFreeFalling)
            {
                return;
            }

            var columnTopPosition =
                _boardField.GetBoardFieldColumnTopPosition(_gridPosition.x);
            var targetPosition = columnTopPosition + 1;
            
            if (_gridPosition.y == targetPosition)
            {
                LockBlock();
            }
        }

        private void LockBlock()
        {
            
        }
    }
}