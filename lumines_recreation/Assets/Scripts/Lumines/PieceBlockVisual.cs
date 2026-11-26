using System;
using UnityEngine;

namespace Lumines
{
    /// <summary>
    /// Piece Block instance used for object recycle and reuse
    /// can be inherited to implement other visual effect
    /// </summary>
    public class PieceBlockVisual : MonoBehaviour
    {
        [Header("Components")] 
        public Transform transClearBox;

        private void Awake()
        {
            transClearBox.gameObject.SetActive(false);
        }

        public void ReadyToClear()
        {
            transClearBox.gameObject.SetActive(true);
        }
    }
}