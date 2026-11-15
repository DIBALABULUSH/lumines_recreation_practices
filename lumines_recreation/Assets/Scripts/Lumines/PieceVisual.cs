using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Lumines
{
    public class PieceVisual : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private Transform blockAPrefab;
        [SerializeField] private Transform blockBPrefab;

        [Header("Binding")]
        [SerializeField] private Transform rotateRoot;
        [SerializeField] private List<Transform> blockRootList = new List<Transform>();

        [Header("Animation")] 
        [SerializeField] private Vector3 rotateAnglePerStep = new Vector3(0, 0, 90f);
        [SerializeField] private float rotateDuration = 0.2f;
        [SerializeField] private AnimationCurve rotateCurve;
        
        private Tween _rotateTween;
        
        public float RotateDuration => rotateDuration;
        
        [Button]
        public Tween RotateAnimation(int rotate, int ccw = 1)
        {
            _rotateTween?.Kill();
            
            var tween = 
                rotateRoot.DORotate(rotate * ccw * rotateAnglePerStep, rotateDuration, RotateMode.FastBeyond360)
                .SetEase(rotateCurve);
            
            _rotateTween = tween;
            return tween;
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
