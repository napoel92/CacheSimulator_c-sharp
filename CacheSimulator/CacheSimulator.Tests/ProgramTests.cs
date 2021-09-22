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
        public void loopAllTests()
        {
            var outputs = createOutputs();
            var expectedOutputs = getExpectedFiles();
            List<string> fails = new();

            string good;
            for(int i=0; i< outputs.Length; ++i)
            {
                fails.Add($"test {i} pass");
                if (FileEquals(outputs[i], expectedOutputs[i]))  continue;
                fails...
            }
            Assert.AreEqual("", fails);
            
        }




        static bool FileEquals(string path1, string path2)
        {
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



        public string[] createOutputs()
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



        public string[] getCommandFiles()
        {
            string[] subDirectory = { Directory.GetCurrentDirectory(), "tests" };
            string testsDirectory = Path.Combine(subDirectory);
            return Directory.GetFiles(testsDirectory, "*.command");
        }


        public string[] getExpectedFiles()
        {
            string[] subDirectory = { Directory.GetCurrentDirectory(), "tests" };
            string testsDirectory = Path.Combine(subDirectory);
            return Directory.GetFiles(testsDirectory, "*.OURS");
        }

        public void removeOldFiles()
        {
            string[] subDirectory = { Directory.GetCurrentDirectory(), "tests" };
            string testsDirectory = Path.Combine(subDirectory);
            var folder =  Directory.GetFiles(testsDirectory, "*.out");
            foreach (string file in folder) File.Delete(file);

            var newFolder = Directory.GetFiles(testsDirectory, "*.out");
            if (newFolder.Length != 0) throw new Exception("out files arent deleted");

        }




        public string[] prepareArgs(string filePath)
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
    }
}