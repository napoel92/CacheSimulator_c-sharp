using System;
using System.Collections.Generic;

namespace CacheSimulator
{
    public class CacheMemory
    {

        private List<List<MemoryBlock>> sets;
        private List<int> policyLRU;

        public uint cacheSize { get; init; }
        public uint log2BlockSize { get; init; }
        public uint cyclesNum { get; init; }
        public uint waysNum { get; init; }
        public int missNumber { get; set; } = 0;
        public int accessNumber { get; set; } = 0;

        public CacheMemory(uint associativity, uint layerSize, uint blockSize, uint cyclesNum)
        {
            var setsNumber = (int)Math.Pow(2, layerSize - blockSize - associativity);
            sets = new List<List<MemoryBlock>>(setsNumber);
            policyLRU = new List<int>(setsNumber);

            var waysNumber = (int)Math.Pow(2, associativity);
            for (int i = 0; i < sets.Capacity ; ++i)
            {
                sets.Add(new List<MemoryBlock>(waysNumber));
                policyLRU.Add(Const.INITIAL_LRU_COUNTER);
            }

            cacheSize = (uint)sets.Capacity;
            this.log2BlockSize = blockSize;
            this.cyclesNum = cyclesNum;
            this.waysNum = (uint)sets[0].Capacity;
        }

        public bool containsBlockOf(uint address)
        {
            List<MemoryBlock> set = getSet(address);
            uint tag = getTag(address);

            foreach(MemoryBlock block in set)
            {
                if ((block.isValid) && (tag == block.tag))
                {
                    return true;
                }
            }
            return false;
        }

        private uint getTag(uint address)
        {
            int log2Ways = (int)Math.Log2(waysNum);
            int log2CacheSize = (int)Math.Log2(cacheSize);
            int bitsNum = 32 - (log2CacheSize - log2Ways + 1);

            return Program.ExtractBits(address, bitsNum, log2CacheSize - log2Ways + 1);
        }

        private List<MemoryBlock> getSet(uint address)
        {
            int setBits = (int)Math.Log2(cacheSize) -(int)Math.Log2(waysNum) - (int)log2BlockSize;
            int index = (int)Program.ExtractBits(address, setBits, (int)log2BlockSize + 1);
            return sets[index];
        }

        public List<MemoryBlock> updateLRU(uint address)
        {
            int setBits = (int)Math.Log2(cacheSize) - (int)Math.Log2(waysNum) - (int)log2BlockSize;
            int i = (int)Program.ExtractBits(address, setBits, (int)log2BlockSize + 1);
            getBlock(address);
        }

        private MemoryBlock getBlock(uint address)
        {
            List<MemoryBlock> set = getSet(address);
            uint tag = getTag(address);

            foreach (MemoryBlock block in set)
            {
                if ((block.isValid) && (tag == block.tag))
                {
                    return block;
                }
            }
            throw new Exception("block should have been in the chache");
        }
    }
}

