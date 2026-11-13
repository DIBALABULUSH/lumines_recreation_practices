using System;
using UnityEngine;

namespace Lumines
{
    /// <summary>
    /// Lumines Piece
    /// </summary>
    public class Piece : MonoBehaviour
    {
        // Piece type
        private readonly byte[,] piece = new byte[2, 2];

        public void Init()
        {
            
        }
        
        public void CwRotatePiece()
        {
            RotatePiece(-90f);
        }

        public void CcwRotatePiece()
        {
            RotatePiece(90f);
        }

        public void RotatePiece(float angle)
        {
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    var coord = new Vector2Int(x, y);
                    
                    
                }
            }
        }
    }
}
