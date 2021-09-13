using System;
using System.Collections.Generic;

namespace CacheSimulator
{
    public class CacheMemory
    {

        private List<List<MemoryBlock>> sets;
        private List<List<MemoryBlock>> policyLRU;

        public uint cacheSize { get; init; }
        public uint blockSize { get; init; }
        public uint cyclesNum { get; init; }
        public uint waysNum { get; init; }



        public int missNumber { get; set; }
        public int accessNumber { get; set; }

        public CacheMemory(uint associativity, uint layerSize, uint blockSize, uint cyclesNum)
        {
            var setsNumber = (int)Math.Pow(2, layerSize - blockSize - associativity);
            sets = new List<List<MemoryBlock>>(setsNumber);

            var waysNumber = (int)Math.Pow(2, associativity);
            for (int i = 0; i < sets.Count ; ++i)
            {
                sets[i].Capacity = waysNumber;
            }

            cacheSize = (uint)sets.Capacity;
            this.blockSize = blockSize;
            this.cyclesNum = cyclesNum;
            this.waysNum = (uint)sets[0].Capacity;
        }
    }
}

