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


        MemoryBlock(bool isValid=false, bool isDirty=false, uint tag=0, uint data=0, uint statusLRU=0)
        {
            this.isValid = isValid;
            this.isDirty =  isDirty;
            this.tag =  tag;
            this.data =  data;
            this.statusLRU =  statusLRU;
        }
    }
}

