using System;
using System.Collections.Generic;

namespace CacheSimulator
{
    public class CacheMemory
    {

        private List<List<MemoryBlock>> sets;
        private List<uint> policyLRU;
        internal List<uint> usedWays;

        public uint cacheSize { get; init; }
        public uint log2BlockSize { get; init; }
        public uint cyclesNum { get; init; }
        public uint waysNum { get; init; }
        internal int missNumber { get; set; } = 0;
        internal int accessNumber { get; set; } = 0;

        public CacheMemory(uint associativity, uint layerSize, uint blockSize, uint cyclesNum)
        {
            var setsNumber = (int)Math.Pow(2, layerSize - blockSize - associativity);
            sets = new List<List<MemoryBlock>>(setsNumber);
            policyLRU = new List<uint>(setsNumber);
            usedWays = new List<uint>(setsNumber);

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




        internal bool containsBlockOf(uint address)
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





        internal uint getTag(uint address)
        {
            int log2Ways = (int)Math.Log2(waysNum);
            int log2CacheSize = (int)Math.Log2(cacheSize);
            int bitsNum = 32 - (log2CacheSize - log2Ways + 1);

            return Program.ExtractBits(address, bitsNum, log2CacheSize - log2Ways + 1);
        }



        internal int getSetIndex(uint address)
        {
            int setBits = (int)Math.Log2(cacheSize) - (int)Math.Log2(waysNum) - (int)log2BlockSize;
            return (int)Program.ExtractBits(address, setBits, (int)log2BlockSize + 1);
        }





        internal List<MemoryBlock> getSet(uint address)
        {
            int i = getSetIndex(address);
            return sets[i];
        }








        internal MemoryBlock updateLRU(uint address)
        {
            int i = getSetIndex(address);            
            blockOf(address).statusLRU = ++policyLRU[i];
            sets[i].Sort((x,y) => (y.statusLRU < x.statusLRU) ? (-1) : (1));
            return blockOf(address);
        }




        internal MemoryBlock blockOf(uint address)
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

        internal MemoryBlock freeWayFor(uint address)
        {
            var set = getSet(address);
            foreach (MemoryBlock block in set)
            {
                if (block.isValid == false)
                {
                    break;
                }
            }
            throw new Exception("there was supposed to be a free block in the set while inserting");
        }

        internal MemoryBlock leastRecentlyUsed(uint address)
        {
            var lruBlock = getSet(address)[Const.LEAST_RECENTLY_USED];
            if( lruBlock.isValid==Const.INVALID )
            {
                throw new Exception("LRU-block should always be VALID");
            }
            return lruBlock;
        }
    }
}

