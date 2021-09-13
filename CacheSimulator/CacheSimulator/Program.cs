using System;
using System.IO;

namespace CacheSimulator
{
    public class Program
    {
        static int Main(string[] args)
        {

            // initialize the data structure:
            //*********************************************************************************************************
            var varsAreVerified = assignVariables(args, out ushort WrAlloc, out ushort MemCyc, out ushort BSize,
                                                         out ushort L1Size, out ushort L1Cyc, out ushort L1Assoc,
                                                         out ushort L2Size, out ushort L2Cyc, out ushort L2Assoc);
            if (!varsAreVerified)
            { 
                return 0;
            } 
            var memory = new Memory(L1Assoc, L1Size, L1Cyc, L2Assoc, L2Size, L2Cyc, WrAlloc, BSize, MemCyc);




            // parse the input file into the simulator:
            //*********************************************************************************************************
            string filePath = args[0];
            using (var file = File.OpenText(filePath))
            {
                if (!fileIsVerified(file))
                {
                    return 0;
                } 
                string commandLine;
                while ((commandLine = file.ReadLine()) != null)
                {
                    ++memory.acessNum;
                    string[] LineContent = commandLine.Split(' ');
                    if (LineContent.Length < 2)
                    {
                        Console.WriteLine("Command Format error");
                        return 0;
                    }
                    char operation = LineContent[0].ToCharArray()[0]; ;
                    string cutAddress = LineContent[1].Substring(2);
                    uint address = Convert.ToUInt32(cutAddress, 16);
                    handleMemoryRequest(operation, address);
                }

            }




            // calculate and display results:
            //*********************************************************************************************************
            //double L1MissRate = (double)memory.L1.missNum / memory.L1.acssesNum;
            //double L2MissRate = (double)memory.L2.missNum / memory.L2.acssesNum;
            //double avgAccTime = (double)memory.totalTime / memory.acessNum;

            //printf("L1miss=%.03f ", L1MissRate);
            //printf("L2miss=%.03f ", L2MissRate);
            //printf("AccTimeAvg=%.03f\n", avgAccTime);

            return 0;


            

        }



        private static void handleMemoryRequest(char operation, uint address)
        {
            //++memory.L1.acssesNum;
            //memory.totalTime += memory.L1.cyclesNum;
            ////--------------------------------------

            //if (memory.L1.containsBlockOf(address))
            //{
            //    memory.L1_Hit(address, operation);
            //    return;
            //}
            //else { ++memory.L1.missNum; /* move to search in L2-cache-Memory */ }


            //++memory.L2.acssesNum;
            //memory.totalTime += memory.L2.cyclesNum;
            ////--------------------------------------
            //if (memory.L2.containsBlockOf(address))
            //{
            //    memory.L2_Hit(address, operation);
            //    return;
            //}
            //else { ++memory.L2.missNum; /* move to search in ram-Memory */ }


            //memory.totalTime += memory.cyclesNum;
            ////--------------------------------------
            //memory.L1_and_L2_Miss(address, operation);
            //return;
        }




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




        private static bool assignVariables(string[] args, out ushort WrAlloc, out ushort MemCyc, out ushort BSize,
                                            out ushort L1Size, out ushort L1Cyc, out ushort L1Assoc,
                                            out ushort L2Size, out ushort L2Cyc, out ushort L2Assoc)
        {
            WrAlloc = 0;
            MemCyc = 0;
            BSize = 0;
            L1Size = 0;
            L1Cyc = 0;
            L1Assoc = 0;
            L2Size = 0;
            L2Cyc = 0;
            L2Assoc = 0;

            if (!argsAreVerified(args))
            {
                return false;
            }

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
                    WrAlloc = converted;
                }
                else
                {
                    TextWriter errorWriter = Console.Error;
                    errorWriter.WriteLine("Error in arguments");
                    return false;
                }
            }
            return true;
        }



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

