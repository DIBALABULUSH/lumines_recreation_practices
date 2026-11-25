using UnityEngine;

namespace Lumines
{
    /// <summary>
    /// Merged piece block, it combined by piece block on the board field
    /// and scanned by timeline and play some cool vfx 
    /// </summary>
    public class MergedPieceBlock : MonoBehaviour
    {
        [SerializeField] private MergedPieceBlockVisual visual;
        
        // this position is different from grid position,
        // it can be 
        private Vector2Int _centerPosition;

        // StartFrom 2x2 until the 10x10
        private Vector2Int _blockSize;

        private bool _isScanning;
        
        public void StartScannedByTimeline()
        {
            if (_isScanning)
            {
                return;
            }
            
            _isScanning = true;
        }
        
        public void EndScannedByTimeline()
        {
            Destroy(gameObject);
        }
    }
}