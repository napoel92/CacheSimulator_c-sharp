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
            var modified = cacheL1.updateLRU(address);
            modified.isDirty = (operation == Const.WRITE) ? Const.DIRTY : modified.isDirty;
        }







        internal void L2_Hit(uint address, char operation)
        {
            var set = CacheL1.getSet(address);
            int setIndex = CacheL1.getSetIndex(address);
            char missInL1 = operation;


            if (Const.WRITE == missInL1)
            {
                if (/*******************/ writePolicy == Const.WRITE_ALLOCATE /*************/)
                {//----->> read_Hit in L2
                    if (CacheL1.usedWays[setIndex] < CacheL1.waysNum)
                    {
                        putInFreeWay(address).isDirty = Const.DIRTY;
                        return;
                    }
                    else
                    {
                        evictAndPut(address).isDirty = Const.DIRTY;
                        return;
                    }
                }
                else  /**************  writePolicy == NO_WRITE_ALLOCATE ******************/
                {//----->> write_Hit in L2
                    CacheL2.updateLRU(address).isDirty = Const.DIRTY;
                    return;
                }
            }
        }
    




        private MemoryBlock evictAndPut(uint address)
        {
            CacheMemory targetCache = null;
            MemoryBlock evicted = null;
            uint tag;

            // miss in L1 and hit in L2
            if ((CacheL1.containsBlockOf(address) == false) && (CacheL2.containsBlockOf(address)))
            {
                targetCache = CacheL1;
                evicted = CacheL1.freeWayFor(address);
                CacheL2.updateLRU(address); //  <----- read L2
                evictFrom(CacheL1, address);
                tag = CacheL1.getTag(address);
            }
            // miss in L2 thus acsses Mem
            else
            {
                targetCache = CacheL2;
                evicted = CacheL2.leastRecentlyUsed(address);
                evictFrom(CacheL2, address);
                tag = CacheL2.getTag(address);
            }

            if (evicted.isValid || evicted.isDirty) throw new Exception("evacuated block is VALID and NOT_DIRTY");
            evicted.tag = tag;
            evicted.data = address;
            return targetCache.updateLRU(address); // <---- read target cache ( L1 or L2 )
        }








        internal void L1_and_L2_Miss(uint address, char operation)
        {
            if ((writePolicy == Const.NO_WRITE_ALLOCATE) && (operation == Const.WRITE))
            {
                /*      we "only" need to write the data into the
                        block that is allready in the main memory   */
                return;
            }

            if(Const.WRITE_ALLOCATE!=writePolicy &&  Const.READ!=operation)
            {
                throw new Exception("Oeration dont match to the Writing-Policy");
            }

            handle_L2_Miss(address);
            handle_L1_Miss(address, operation);
        }









        private void evictFrom(CacheMemory cacheLi, uint address)
        {
            var lruBlock = cacheLi.getSet(address)[0];
            if (lruBlock.isValid == Const.INVALID)
            {
                throw new Exception("LRU-block should always be VALID");
            }
            uint evictedAddress = cacheLi.getSet(address)[Const.LEAST_RECENTLY_USED].data;
            var evictedBlock = cacheLi.getSet(address)[Const.LEAST_RECENTLY_USED];

            if (evictedBlock.isDirty && cacheLi == cacheL1)
            {
                cacheL2.updateLRU(evictedAddress).isDirty = Const.DIRTY; // write L2
            }
            evictedBlock.isDirty = Const.NOT_DIRTY;
        }








        private MemoryBlock putInFreeWay(uint address)
        {
            CacheMemory targetCache = null;
            MemoryBlock freeBlock = null;
            uint tag;

            if ((CacheL1.containsBlockOf(address) == false) && (CacheL2.containsBlockOf(address)))
            {
                targetCache = CacheL1;
                freeBlock = CacheL1.freeWayFor(address);
                tag = CacheL1.getTag(address);
                CacheL2.updateLRU(address); // read-request sent to L2
            }
            else
            {
                targetCache = CacheL2;
                freeBlock = CacheL2.freeWayFor(address);
                tag = CacheL2.getTag(address);
                // no-need for LRU-policy managing. read-request sent to Mem
            }

            if(freeBlock.isValid || freeBlock.isDirty)  throw new Exception("free block is INVALID and NOT_DIRTY");
            freeBlock.isValid = true;
            int i = targetCache.getSetIndex(address);
            ++targetCache.usedWays[i];

            freeBlock.tag = tag;
            freeBlock.data = address;
            return targetCache.updateLRU(address); // <---- read target cache ( L1 or L2 )
        }
    }
}