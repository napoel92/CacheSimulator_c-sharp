using System;
using System.Collections.Generic;

namespace CacheSimulator
{
    /*                     THE data-type for representing the whole memory structure.
                                this struct is the type that main calls                                    */
    //=========================================================================================================
    public class Memory
    {
       

        public CacheMemory CacheL1 { get; init; }
        public CacheMemory CacheL2 { get; init; }



        public bool writePolicy { get; init; }
        public uint ramCyclesNumber { get; init; }
        public uint totalAccessTime { get; set; } = 0;
        public uint accessNumber { get; set; } = 0;



        public Memory(ushort l1Assoc, ushort l1Size, ushort l1Cyc, ushort l2Assoc, ushort l2Size, ushort l2Cyc, bool wrAlloc, ushort bSize, ushort memCyc)
        {
            CacheL1 = new CacheMemory(l1Assoc, l1Size, bSize, l1Cyc);
            CacheL2 = new CacheMemory(l2Assoc, l2Size, bSize, l2Cyc);
            writePolicy = wrAlloc;
            ramCyclesNumber = memCyc;

        }


        


        // returns the right value of access-time per cache's level 
        public uint accessTimeOfCache(int level)
        {
            if (1 == level)
            {
                return CacheL1.cyclesNum;

            }
            else if (2 == level)
            {
                return CacheL2.cyclesNum;
            }
            throw new ArgumentException("there are only cache-L1 and cache-L2");
        }




        // increments the counter of the access-number per cache's level 
        public void increaseAccessToCache(int level)
        {
            if (1 == level)
            {
                ++CacheL1.accessNumber;
                return;

            }
            else if (2 == level)
            {
                ++CacheL2.accessNumber;
                return;
            }
            throw new ArgumentException("there are only cache-L1 and cache-L2");
        }



        // increments the counter of the miss-number per cache's level 
        public void increaseMissesOfCache(int cacheID)
        {
            if (1 == cacheID)
            {
                ++CacheL1.missNumber;
                return;

            }
            else if (2 == cacheID)
            {
                ++CacheL2.missNumber;
                return;
            }
            throw new ArgumentException("there are only cache-L1 and cache-L2");
        }





        // acssessing the data in L1 cache
        internal void L1_Hit(uint address, char operation)
        {
            var modified = CacheL1.updateLRU(address);
            modified.isDirty = (operation == Const.WRITE) ? Const.DIRTY : modified.isDirty;
        }





        // acssessing the data in L2 cache
        internal void L2_Hit(uint address, char operation)
        {
            var set = CacheL1.getSet(address);
            int setIndex = CacheL1.getSetIndex(address);
            char missInL1 = operation;


            if (/*************************/ Const.WRITE == missInL1 /***************************/)
            {
                if ( writePolicy == Const.WRITE_ALLOCATE)        //----->> read_Hit in L2
                {                                                   
                    if (CacheL1.usedWays[setIndex] < CacheL1.waysNum)
                            putInFreeWay(address).isDirty = Const.DIRTY;
                    else    evictAndPut(address).isDirty = Const.DIRTY;
                    return;
                }
                else /*  writePolicy == NO_WRITE_ALLOCATE */    //----->> write_Hit in L2
                {                                               
                    CacheL2.updateLRU(address).isDirty = Const.DIRTY;
                    return;
                }
            }

            else    /********************** Const.WRITE == missInL1 ****************************/
            {
                if(CacheL1.usedWays[setIndex] < CacheL1.waysNum)
                       putInFreeWay(address);
                else   evictAndPut(address);
                return;
            }
        }




        // writes a block to a full-set in the cache (assumes that Level_1 got miss)
        private MemoryBlock evictAndPut(uint address)
        {
            CacheMemory targetCache = null;
            MemoryBlock evicted = null;
            uint tag;

            // miss in L1 and hit in L2
            if ((CacheL1.containsBlockOf(address) == false) && (CacheL2.containsBlockOf(address)))
            {
                targetCache = CacheL1;
                evicted = CacheL1.leastRecentlyUsed(address);
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

            if ( !evicted.isValid || evicted.isDirty) throw new Exception("evacuated block is VALID and NOT_DIRTY");
            evicted.tag = tag;
            evicted.data = address;
            return targetCache.updateLRU(address); // <---- read target cache ( L1 or L2 )
        }







        // handles the "worst case" of accessing the ram-Memory
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





        // acssessing the data in L2 cache, and fetching it into L1 cache after bringing it from main-Memory
        private void handle_L1_Miss(uint address, char operation)
        {
            var setIndex = CacheL1.getSetIndex(address);
            
            if (CacheL1.usedWays[setIndex] < CacheL1.waysNum)
            {
                putInFreeWay(address);
            }
            else
            {
                evictAndPut(address);
            }
            var modified = CacheL1.blockOf(address);
            modified.isDirty = (operation==Const.WRITE) ? (Const.DIRTY) : (modified.isDirty);
        }





        // acssessing the data in the main Memory, and fetching it into L2 cache
        private void handle_L2_Miss(uint address)
        {
            List<MemoryBlock> set = CacheL2.getSet(address);
            //if (null == set) throw new NullReferenceException("sets shouldn't be null");

            int setIndex = CacheL2.getSetIndex(address);
            var usedWays = CacheL2.usedWays[setIndex];

            if (CacheL2.usedWays[setIndex] < CacheL2.waysNum)
            {
                putInFreeWay(address);
            }
            else
            {
                L2_snoops_L1(address);
                evictAndPut(address);
            }
        }





        // keeping coherent by asserting the evacuation of data from L1 when evicting from l2
        private void L2_snoops_L1(uint address)
        {
            uint evictedAddress_L2 = CacheL2.getSet(address)[Const.LEAST_RECENTLY_USED].data;
            if (CacheL1.containsBlockOf(evictedAddress_L2) == false) return;

            int setIndex = CacheL1.getSetIndex(address);
            CacheL1.blockOf(evictedAddress_L2).reset();
            --CacheL1.usedWays[setIndex];
            // if the data is dirty in L1  ===>>   "we assign" dirtyBit=DIRTY also in L2
            //                                        (but its meaningless in the code)
        }






        // vacates a block with 'address' due to 'cacheLi' got Miss for capacity or compulsary
        private void evictFrom(CacheMemory cacheLi, uint address)
        {
            var lruBlock = cacheLi.getSet(address)[0];
            if (lruBlock.isValid == Const.INVALID)
            {
                throw new Exception("LRU-block should always be VALID");
            }
            uint evictedAddress = cacheLi.getSet(address)[Const.LEAST_RECENTLY_USED].data;
            var evictedBlock = cacheLi.getSet(address)[Const.LEAST_RECENTLY_USED];

            if (evictedBlock.isDirty && cacheLi == CacheL1)
            {
                CacheL2.updateLRU(evictedAddress).isDirty = Const.DIRTY; // write L2
            }
            evictedBlock.isDirty = Const.NOT_DIRTY;
        }






        // writes a block to a non-full-set in cache (assumes that Level_1 got miss)
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