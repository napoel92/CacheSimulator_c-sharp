namespace CacheSimulator
{
    /*      Auxiliary struct for representing a block from the main-Memory,
            that should be (or already) stored inside the cache-Memory          */
    //==========================================================================================
    public class MemoryBlock
    {
        public bool isValid { get; set; }
        public bool isDirty { get; set; }
        public uint tag { get; set; }
        public uint data { get; set; }
        public uint statusLRU { get; set; }




        public MemoryBlock()
        {
            isValid = Const.INVALID;
            isDirty = Const.NOT_DIRTY;
            tag =  0;
            data =  0;
            statusLRU = 0;
        }




        internal void reset()
        {
            isValid = Const.INVALID;
            isDirty = Const.NOT_DIRTY;
            tag = 0;
            data = 0;
            statusLRU = 0;
        }
    }
}

