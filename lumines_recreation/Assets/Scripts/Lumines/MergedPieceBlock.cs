using Tools;
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
        private Vector2 _centerPosition;

        // StartFrom 2x2 until the 10x10
        private Vector2Int _blockSize;

        private bool _isScanning;
        
        public void Init(Vector2Int size, Vector2 centerPosition)
        {
            _blockSize = size;
            _centerPosition = centerPosition;

            UpdateBlock();
        }

        private void UpdateBlock()
        {
            transform.localScale = _blockSize.ToVec3();
            transform.localPosition = _centerPosition.ToVec3();
        }

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