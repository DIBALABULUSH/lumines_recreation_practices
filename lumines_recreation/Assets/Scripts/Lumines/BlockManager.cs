using System.Collections.Generic;
using UnityEngine;

namespace Lumines
{
    public static class BlockManager
    {
        // every type of piece blocks
        private static Dictionary<byte, ObjectPool<PieceBlock>> _pieceBlockDict;
        // every type of merged blocks
        private static Dictionary<byte, ObjectPool<MergedPieceBlock>> _mergedBlockDict;
        // every type of merged block variants,
        // the variants are use for very big block like 6x6 block
        private static Dictionary<byte, ObjectPool<MergedPieceBlock>> _mergedBlockVariantDict;
        
        public static GameObject GetBlock()
        {
            return null;
        }
    }
}