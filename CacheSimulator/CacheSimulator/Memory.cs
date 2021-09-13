namespace CacheSimulator
{
    public class Memory
    {
        private ushort l1Assoc;
        private ushort l1Size;
        private ushort l1Cyc;
        private ushort l2Assoc;
        private ushort l2Size;
        private ushort l2Cyc;
        private ushort wrAlloc;
        private ushort bSize;
        private ushort memCyc;

        public Memory(ushort l1Assoc, ushort l1Size, ushort l1Cyc, ushort l2Assoc, ushort l2Size, ushort l2Cyc, ushort wrAlloc, ushort bSize, ushort memCyc)
        {
            this.l1Assoc = l1Assoc;
            this.l1Size = l1Size;
            this.l1Cyc = l1Cyc;
            this.l2Assoc = l2Assoc;
            this.l2Size = l2Size;
            this.l2Cyc = l2Cyc;
            this.wrAlloc = wrAlloc;
            this.bSize = bSize;
            this.memCyc = memCyc;
        }

        public int acessNum { get; set; }
    }
}