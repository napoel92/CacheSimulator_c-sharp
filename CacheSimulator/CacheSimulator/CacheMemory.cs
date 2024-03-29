﻿using System;
using System.Collections.Generic;

namespace CacheSimulator
{

    /*                      Auxiliary struct for representing a layer of the cache-Memory,
                                        i.e L1-cache or L2-cache                                                 */
    //===============================================================================================================
    public class CacheMemory
    {

        private List<List<MemoryBlock>> sets;
        private List<uint> policyLRU;
        internal List<uint> usedWays;
        //------------------------------------------------
        public uint setsNumber { get; init; }
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
                usedWays.Add(0);
                policyLRU.Add(Const.INITIAL_LRU_COUNTER);
            }

            this.setsNumber = (uint)sets.Capacity;
            this.log2BlockSize = blockSize;
            this.cyclesNum = cyclesNum;
            this.waysNum = (uint)sets[0].Capacity;
        }





        // checks if this cache-level is currently holding the address's block
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




        // gets the tag of the address in this level-cache's point of view
        internal uint getTag(uint address)
        {
            int log2Ways = (int)Math.Log2(waysNum);
            int log2CacheSize = (int)Math.Log2(setsNumber)+(int)log2BlockSize+log2Ways;
            int bitsNum = 32 - (log2CacheSize - log2Ways + 1);

            return Program.ExtractBits(address, bitsNum, log2CacheSize - log2Ways + 1);
        }



        // gets the correct set-number by the address that maps into it
        internal int getSetIndex(uint address)
        {
            int setBits = (int)Math.Log2(setsNumber);
            return (int)Program.ExtractBits(address, setBits, (int)log2BlockSize + 1);
        }





        // gets the set in which the address is mapped to
        internal List<MemoryBlock> getSet(uint address)
        {
            int i = getSetIndex(address);
            return sets[i];
        }





        // method for managing the policy of Blocks-evacuation from the cache
        internal MemoryBlock updateLRU(uint address)
        {
            int i = getSetIndex(address);            
            blockOf(address).statusLRU = ++policyLRU[i];
            sets[i].Sort((x,y) => (x.statusLRU  < y.statusLRU) ? (-1) : (1));
            return blockOf(address);
        }






        // gets the memory-block in which 'address' is a part of it
        internal MemoryBlock  blockOf(uint address)
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






        //finds the free-way in the relevant set for this cache-layer. (assums THERE IS a free way)
        internal MemoryBlock freeWayFor(uint address)
        {
            var set = getSet(address);
            if (set.Count < set.Capacity) set.Add(new MemoryBlock());

            foreach (MemoryBlock block in set)
            {
                if (block.isValid == false)
                {
                    return block;
                }
            }

            throw new Exception("there was supposed to be a free block in the set while inserting");
        }





        //finds the least-recently-used way in the relevant set for this cache-level. (assums THERE IS a free way)
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

