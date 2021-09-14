using System;

namespace CacheSimulator
{

    public class Memory
    {
        public readonly CacheMemory cacheL1;
        public readonly CacheMemory cacheL2;
        public CacheMemory CacheL1 { get; init; }
        public CacheMemory CacheL2 { get; init; }

        public bool writePolicy { get; init; }
        public uint ramCyclesNumber { get; init; }
        public uint totalAccessTime { get; set; } = 0;
        public uint accessNumber { get; set; } = 0;



        public Memory(ushort l1Assoc, ushort l1Size, ushort l1Cyc, ushort l2Assoc, ushort l2Size, ushort l2Cyc, bool wrAlloc, ushort bSize, ushort memCyc)
        {
            cacheL1 = new CacheMemory(l1Assoc, l1Size, bSize, l1Cyc);
            cacheL2 = new CacheMemory(l2Assoc, l2Size, bSize, l2Cyc);
            writePolicy = wrAlloc;
            ramCyclesNumber = memCyc;

        }


        



        public uint accessTimeOfCache(int cacheID)
        {
            if (1 == cacheID)
            {
                return cacheL1.cyclesNum;

            }
            else if (2 == cacheID)
            {
                return cacheL2.cyclesNum;
            }
            throw new ArgumentException("there are only cache-L1 and cache-L2");
        }


        public void increaseAccessToCache(int cacheID)
        {
            if (1 == cacheID)
            {
                ++cacheL1.accessNumber;
                return;

            }
            else if (2 == cacheID)
            {
                ++cacheL2.accessNumber;
                return;
            }
            throw new ArgumentException("there are only cache-L1 and cache-L2");
        }



        public void increaseMissesOfCache(int cacheID)
        {
            if (1 == cacheID)
            {
                ++cacheL1.missNumber;
                return;

            }
            else if (2 == cacheID)
            {
                ++cacheL2.missNumber;
                return;
            }
            throw new ArgumentException("there are only cache-L1 and cache-L2");
        }

        internal void L1_Hit(uint address, char operation)
        {
            std::vector<Block>::iterator modified = cacheL1.updateLRU(address);
            modified->dirtyBit = (operation == WRITE) ? DIRTY : modified->dirtyBit;
        }
    }
}