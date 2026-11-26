using System.Collections.Generic;
using UnityEngine;

namespace Lumines
{
    public class BlockManager : Singleton<BlockManager>
    {
        // every type of piece blocks
        private Dictionary<byte, ObjectPool<PieceBlock>> _pieceBlockDict;
        // every type of merged blocks
        private Dictionary<byte, ObjectPool<MergedPieceBlock>> _mergedBlockDict;
        // every type of merged block variants,
        // the variants are use for very big block like 6x6 block
        private Dictionary<byte, ObjectPool<MergedPieceBlock>> _mergedBlockVariantDict;

        private readonly byte[] _defaultBlockTypes = new byte[]{0, 1};
        private int maxBlockType = 2;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            _pieceBlockDict = new Dictionary<byte, ObjectPool<PieceBlock>>();
            _mergedBlockDict = new Dictionary<byte, ObjectPool<MergedPieceBlock>>();
            _mergedBlockVariantDict = new Dictionary<byte, ObjectPool<MergedPieceBlock>>();
            
            // init dictionary by default block types
            for (var i = 0; i < maxBlockType; i++)
            {
                _pieceBlockDict.Add((byte)i, new ObjectPool<PieceBlock>());
                _mergedBlockDict.Add((byte)i, new ObjectPool<MergedPieceBlock>());
                _mergedBlockVariantDict.Add((byte)i, new ObjectPool<MergedPieceBlock>());
            }
        }

        public PieceBlock GetBlock(byte blockType)
        {
            var pool = _pieceBlockDict.GetValueOrDefault(blockType);
            if (pool == null)
            {
                Debug.LogWarning("block type object pool is null");
                return null;
            }
            
            return pool.GetObj();
        }
        
        public void Clear()
        {
            foreach (var pieceBlock in _pieceBlockDict.Values)
            {
                pieceBlock.Clear();
            }
            foreach (var mergedBlock in _mergedBlockDict.Values)
            {
                mergedBlock.Clear();
            }
            foreach (var pieceBlock in _mergedBlockVariantDict.Values)
            {
                pieceBlock.Clear();
            }
        }
    }
}