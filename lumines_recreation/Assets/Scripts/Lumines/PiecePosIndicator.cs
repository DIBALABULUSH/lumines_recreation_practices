using System;
using UnityEngine;

namespace Lumines
{
    /// <summary>
    /// Piece position indicator,
    /// use for showing the piece real position on the board field
    /// </summary>
    public class PiecePosIndicator : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private BoardField boardField;
        
        [SerializeField] private Piece followedPiece;
        private Vector2Int _gridPos;

        public void SetFollowedPiece(Piece piece)
        {
            followedPiece = piece;
        }
        
        private void Update()
        {
            if (followedPiece == null)
            {
                return;
            }
            
            // meh, just directly update the data from the followed piece
            var targetPosX =  followedPiece.GridPieceCenterPos.x;
            transform.localPosition = new Vector3(targetPosX,  transform.localPosition.y, transform.localPosition.z);
        }
    }
}
