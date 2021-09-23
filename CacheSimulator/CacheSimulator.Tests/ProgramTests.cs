using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CacheSimulator.Tests
{
    public class ProgramTests
    {
        
        
        [Test]
        public static void testCache()
        {
            var resultsList = loopAllTests();
            var result = TestPass(resultsList);
            Assert.AreEqual("", result);

        }



        private static string TestPass(List<int> resultsList)
        {
            string results = "";
            string temp = "";
            foreach (int testFaild in resultsList)
            {
                temp += $"{testFaild}, ";
            }
            if (temp != "")
            {
                results = "Failed tests: " + temp.Substring(0, temp.Length - 2);
            }
            return results;
        }






        private static List<int> loopAllTests()
        {
            var outputs = createOutputs();
            var expectedOutputs = getExpectedFiles();
            List<int> failed = new();

            for(int i=0; i< outputs.Length; ++i)
            {
                if (FilesAreEquals(outputs[i], expectedOutputs[i]))  continue;
                failed.Add(i);
            }
            return failed;
        }




        static bool FilesAreEquals(string path1, string path2)
        {
            Dos2Unix(path1);
            byte[] file1 = File.ReadAllBytes(path1);
            byte[] file2 = File.ReadAllBytes(path2);
            
            if (file1.Length == file2.Length)
            {
                for (int i = 0; i < file1.Length; i++)
                {
                    if (file1[i] != file2[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }





        private static string[] createOutputs()
        {
            var commandFileNames = getCommandFiles();
            string filePrefix;
            string[] arguments;
            List<string> outputs = new();

            removeOldFiles();
            foreach (string fileName in commandFileNames)
            {             
                filePrefix = fileName.Split(".command")[0];
                using (var writer = new StreamWriter(filePrefix + ".out"))
                {
                    // Redirect standard output from the console to the output file.
                    Console.SetOut(writer);

                    arguments = prepareArgs(fileName);
                    Program.Main(arguments);
                    outputs.Add(filePrefix + ".out");
                }
                
            }
            // Recover the standard output stream so that a
            // completion message can be displayed.
            var standardOutput = new StreamWriter(Console.OpenStandardOutput());
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
            return outputs.ToArray();
        }





        private static string[] getCommandFiles()
        {
            string[] subDirectory = { Directory.GetCurrentDirectory(), "tests" };
            string testsDirectory = Path.Combine(subDirectory);
            return Directory.GetFiles(testsDirectory, "*.command");
        }



        private static string[] getExpectedFiles()
        {
            string[] subDirectory = { Directory.GetCurrentDirectory(), "tests" };
            string testsDirectory = Path.Combine(subDirectory);
            return Directory.GetFiles(testsDirectory, "*.OURS");
        }



        private static void removeOldFiles()
        {
            string[] subDirectory = { Directory.GetCurrentDirectory(), "tests" };
            string testsDirectory = Path.Combine(subDirectory);
            var folder =  Directory.GetFiles(testsDirectory, "*.out");
            // remove the next line if debbuging...
            foreach (string file in folder) File.Delete(file);

            var newFolder = Directory.GetFiles(testsDirectory, "*.out");
            if (newFolder.Length != 0) throw new Exception("out files arent deleted");

        }




        private static string[] prepareArgs(string filePath)
        {
            List<string> arguments = null;
            using (var file = File.OpenText(filePath))
            {
                string content;
                if ((content = file.ReadLine()) != null)
                {
                    arguments = content.Split(' ').ToList();
                }

            }

            if(arguments!=null)
            {
                Assert.AreEqual(arguments[0].ToCharArray()[0], '.');
                Assert.AreEqual(arguments[0].ToCharArray()[1], '/');
                arguments.Remove(arguments[0]);
                return arguments.ToArray();
            }

            return null;
        }


        private static void Dos2Unix(string fileName)
        {
            const byte CR = 0x0D;
            const byte LF = 0x0A;
            byte[] data = File.ReadAllBytes(fileName);
            using (FileStream fileStream = File.OpenWrite(fileName))
            {
                BinaryWriter bw = new BinaryWriter(fileStream);
                int position = 0;
                int index = 0;
                do
                {
                    index = Array.IndexOf<byte>(data, CR, position);
                    if ((index >= 0) && (data[index + 1] == LF))
                    {
                        // Write before the CR
                        bw.Write(data, position, index - position);
                        // from LF
                        position = index + 1;
                    }
                }
                while (index >= 0);
                bw.Write(data, position, data.Length - position);
                fileStream.SetLength(fileStream.Position);
            }
        }
    }
}