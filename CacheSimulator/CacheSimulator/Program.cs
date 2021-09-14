using System;
using System.IO;


namespace CacheSimulator
{
    public class Program
    {
        static int Main(string[] args)
        {
 
            try
            {
                var varsAreVerified = assignVariables(args, out bool WrAlloc, out ushort MemCyc, out ushort BSize,
                                                         out ushort L1Size, out ushort L1Cyc, out ushort L1Assoc,
                                                         out ushort L2Size, out ushort L2Cyc, out ushort L2Assoc);
                var memory = new Memory(L1Assoc, L1Size, L1Cyc, L2Assoc, L2Size, L2Cyc, WrAlloc, BSize, MemCyc);
                parseTheInput(args, memory);
                DisplayResults(memory);

            }

            catch(Exception exception)
            {
                return 0;
            }
            return 0;
        }



        private static void DisplayResults(Memory memory)
        {
            //double L1MissRate = (double)memory.L1.missNum / memory.L1.acssesNum;
            //double L2MissRate = (double)memory.L2.missNum / memory.L2.acssesNum;
            //double avgAccTime = (double)memory.totalTime / memory.acessNum;

            //printf("L1miss=%.03f ", L1MissRate);
            //printf("L2miss=%.03f ", L2MissRate);
            //printf("AccTimeAvg=%.03f\n", avgAccTime);
        }

        private static void parseTheInput(string[] args, Memory memory)
        {
            string filePath = args[0];
            using (var file = File.OpenText(filePath))
            {
                if (!fileIsVerified(file))
                {
                    throw new FieldAccessException();
                }
                string commandLine;
                while ((commandLine = file.ReadLine()) != null)
                {
                    ++memory.accessNumber;
                    string[] LineContent = commandLine.Split(' ');
                    if (LineContent.Length < 2)
                    {
                        Console.WriteLine("Command Format error");
                        throw new FormatException(); ;
                    }
                    char operation = LineContent[0].ToCharArray()[0]; ;
                    string cutAddress = LineContent[1].Substring(2);
                    uint address = Convert.ToUInt32(cutAddress, 16);
                    handleMemoryRequest(memory, operation, address);
                }

            }
        }



        /*      a function to gradually access the layers
         *      of the Memory-System to find some data                                      */
        //=====================================================================================
        private static void handleMemoryRequest(Memory memory, char operation, uint address)
        {
            memory.increaseAccessToCache(1);
            memory.totalAccessTime += memory.accessTimeOfCache(1);
            //----------------------------------------------------
            if (memory.cacheL1.containsBlockOf(address))
            {
                memory.L1_Hit(address, operation);
                return;
            }
            else { memory.increaseMissesOfCache(1); /* move to search in L2-cache-Memory */ }


            memory.increaseAccessToCache(2);
            memory.totalAccessTime += memory.accessTimeOfCache(2);
            //-----------------------------------------------------
            if (memory.cacheL2.containsBlockOf(address))
            {
                memory.L2_Hit(address, operation);
                return;
            }
            else { memory.increaseMissesOfCache(2); /* move to search in ram-Memory */ }


            memory.totalAccessTime += memory.ramCyclesNumber;
            //-------------------------------------------------
            memory.L1_and_L2_Miss(address, operation);
        }



        /*      a method for verifying a proper
         *      opening of the input file                                       */
        //=========================================================================
        private static bool fileIsVerified(StreamReader file)
        {
            if (file == null)
            {
                // File doesn't exist or some other error
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine("File not found");
                return false;
            }
            return true;
        }



        /*      a method to assign all the values into
         *      Main's variables - foe feeding the data-structure.                                              */
        //=========================================================================================================
        private static bool assignVariables(string[] args, out bool WrAlloc, out ushort MemCyc, out ushort BSize,
                                            out ushort L1Size, out ushort L1Cyc, out ushort L1Assoc,
                                            out ushort L2Size, out ushort L2Cyc, out ushort L2Assoc)
        {
            WrAlloc = false;
            MemCyc = 0;
            BSize = 0;
            L1Size = 0;
            L1Cyc = 0;
            L1Assoc = 0;
            L2Size = 0;
            L2Cyc = 0;
            L2Assoc = 0;
            if (!argsAreVerified(args))  throw new ArgumentException() ;


            for (int i = 1; i < 18; i += 2)
            {
                string s = args[i];
                var converted = Convert.ToUInt16(args[i + 1]);

                if (s == "--mem-cyc")
                {
                    MemCyc = converted;
                }
                else if (s == "--bsize")
                {
                    BSize = converted;
                }
                else if (s == "--l1-size")
                {
                    L1Size = converted;
                }
                else if (s == "--l2-size")
                {
                    L2Size = converted;
                }
                else if (s == "--l1-cyc")
                {
                    L1Cyc = converted;
                }
                else if (s == "--l2-cyc")
                {
                    L2Cyc = converted;
                }
                else if (s == "--l1-assoc")
                {
                    L1Assoc = converted;
                }
                else if (s == "--l2-assoc")
                {
                    L2Assoc = converted;
                }
                else if (s == "--wr-alloc")
                {
                    WrAlloc = (converted==0) ?(Const.NO_WRITE_ALLOCATE):(Const.WRITE_ALLOCATE);
                }
                else
                {
                    TextWriter errorWriter = Console.Error;
                    errorWriter.WriteLine("Error in arguments");
                    throw new ArgumentException();
                }
            }
            return true;
        }




        /*      a method to verify the acceptance of all
         *      18 parameters that should have passed.                                      */
        //=====================================================================================
        private static bool argsAreVerified(string[] args)
        {
            if (args.Length < 18)
            {
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine("Not enough arguments");
                return false;
            }
            return true;
        }




        /*      helper function for extracting decimal-integers 
                out of boulian-representation.                                                                  */
        //=========================================================================================================
        public static uint ExtractBits(uint number, int k, int p)
        {
            if ( (p<1) || (p>32) ) throw new ArgumentOutOfRangeException("position must be in range of 32-bits");
            if ( (k<0) || (k>32) ) throw new ArgumentOutOfRangeException("subAddress must be in range of 32-bits");
            if ( (p-1+k) > 32 ) throw new ArgumentOutOfRangeException("Longst address should be 32-bits");

            var mask = ((uint)(((long)1 << k) - 1));
            var shiftedNumber = (number >> (p - 1));
            return mask & shiftedNumber;

        }
    }
}

