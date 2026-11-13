using UnityEngine;

namespace Lumines
{
    /// <summary>
    /// 16 x 10 board field for placing pieces
    /// </summary>
    public class BoardField : MonoBehaviour
    {
        
        private Vector2Int _boardSize = new Vector2Int(16, 10);
        public Vector2Int BoardSize => _boardSize;

        public bool CheckPieceIsOutOfBound()
        {
            return false;
        }
    }
}
