using System.Collections.Generic;
using UnityEngine;

namespace Lumines
{
    /// <summary>
    /// An object pool which recycle the piece block in game
    /// </summary>
    public class PieceBlockRecycler : MonoBehaviour
    {
        private readonly Queue<PieceBlock> _pieceBlockQueue = new Queue<PieceBlock>();
        
        
        
        public void Recycle(PieceBlock block)
        {
            block.gameObject.SetActive(false);
            _pieceBlockQueue.Enqueue(block);
        }
        
        public PieceBlock GetPieceBlock()
        {
            if (_pieceBlockQueue.Count > 0)
            {
                return _pieceBlockQueue.Dequeue();
            }
            
            Debug.LogError("No piece block instance in recycler");
            return null;
        }
    }
}