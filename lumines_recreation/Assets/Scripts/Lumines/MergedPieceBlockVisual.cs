using UnityEngine;

namespace Lumines
{
    /// <summary>
    /// the visual module of the MergedPieceBlock. playing some vfx
    /// </summary>
    public class MergedPieceBlockVisual : MonoBehaviour
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