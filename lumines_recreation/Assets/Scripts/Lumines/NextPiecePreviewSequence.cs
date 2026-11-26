using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Lumines
{
    /// <summary>
    /// the sequence you can see what's next after you placed the piece 
    /// </summary>
    public class NextPiecePreviewSequence : MonoBehaviour
    {
        [Header("Resource")] 
        [SerializeField] private Piece piecePrefab;

        [Header("Components")] 
        [SerializeField] private BoardField boardField;
        [SerializeField] private Transform pieceRoot;
        
        [Header("Settings")] 
        public int maxPreviewCount = 3;
        public float topPieceOffsetY = 0f;
        public float scaleFromTopToBottom = 0.9f;
        public AnimationCurve scaleCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public float piecePadding = 2.2f;

        private Queue<Piece> _previewQueue = new Queue<Piece>();

        /// <summary>
        /// fill the preview sequence by man preview count
        /// </summary>
        public void FillThePreviewSequence()
        {
            _previewQueue ??= new Queue<Piece>();
            _previewQueue.Clear();
            
            for (var i = 0; i < maxPreviewCount; i++)
            {
                var piece = SpawnPiece();
                _previewQueue.Enqueue(piece);
            }
            
            ArrangeAllPiecesInQueueByOrder();
        }
        
        /// <summary>
        /// spawn piece to fill the sequence
        /// </summary>
        public Piece SpawnPiece()
        {
            var byteArray = GetRandomByteArray(4);
            
            var newPiece = Instantiate(piecePrefab, pieceRoot);
            newPiece.Init(boardField, byteArray);
            newPiece.enabled = false;
            
            return newPiece;
        }
        
        /// <summary>
        /// get piece from the sequence
        /// </summary>
        /// <returns></returns>
        public Piece EnqueueAndSupplementTailPieceInSequence()
        {
            if(_previewQueue.Count == 0)
                return null;
            
            // get the head piece in queue
            var returnPiece = _previewQueue.Dequeue();
            
            // fill the queue tail by new piece
            var newPiece = SpawnPiece();
            _previewQueue.Enqueue(newPiece);

            ArrangeAllPiecesInQueueByOrder();
            
            return returnPiece;
        }

        /// <summary>
        /// arrange pieces from top to bottom by queue head to tail
        /// by using order settings
        /// </summary>
        private void ArrangeAllPiecesInQueueByOrder()
        {
            var order = 0;
            foreach (var piece in _previewQueue)
            {
                var targetPosY = topPieceOffsetY - order * piecePadding;
                piece.transform.localPosition = new Vector3(0f, targetPosY, 0f);
                
                var ratio = order / (float)maxPreviewCount;
                var scale = scaleCurve.Evaluate(ratio);
                piece.transform.localScale = Vector3.one * scale;
                
                order++;
            }
        }
        
        private byte[] GetRandomByteArray(int quantity)
        {
            var byteArray = new byte[quantity];

            for (int i = 0; i < byteArray.Length; i++)
            {
                var randomBits =  new Random().Next(0, byte.MaxValue);
                byteArray[i] = (byte)randomBits;
            }
            return byteArray;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private byte RandomByteLast4Bits(byte value)
        {
            // gen random 0 ~ 15 number
            var randomBits = new Random().Next(0, 16);

            // keep high 4 bits, random low 4 bits
            value = (byte)((value & 0xF0) | randomBits);

            return value;
        }
    }
}