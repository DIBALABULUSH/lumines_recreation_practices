using System.Collections.Generic;
using UnityEngine;

namespace Lumines
{
    public class PieceVisual : MonoBehaviour
    {
        [Header("Prefabs")]
        public Transform blockAPrefab;
        public Transform blockBPrefab;

        [Header("Binding")]
        public Transform rotateRoot;
        public List<Transform> blockRootList = new List<Transform>();
        
        public void RotateAnimation()
        {
            
        }
        
        public void UpdatePiece(byte[, ] piece)
        {
            for (int i = 0; i < blockRootList.Count; i++)
            {
                var value = blockRootList[i];
                var block = value ? blockBPrefab : blockAPrefab;
                
            }
        }
    }
}
