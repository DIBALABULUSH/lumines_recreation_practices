using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
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

        [Header("Animation")] 
        public Vector3 rotateAnglePerStep = new Vector3(0, 0, 90f);
        public float rotateDuration = 0.2f;
        public AnimationCurve rotateCurve;
        
        private Tween _rotateTween;
        
        [Button]
        public void RotateAnimation(int rotate, int ccw = 1)
        {
            _rotateTween?.Kill();
            
            _rotateTween = 
                rotateRoot.DORotate(rotate * ccw * rotateAnglePerStep, rotateDuration)
                .SetEase(rotateCurve);
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
