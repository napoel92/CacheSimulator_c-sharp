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
        }




        public List<StreamWriter> createOutputs()
        {
            var commandFileNames = getCommandFiles();
            var outputs = new List<StreamWriter>();
            string filePrefix;
            string[] arguments;

            foreach(string fileName in commandFileNames)
            {             
                filePrefix = fileName.Split('.')[0];
                using (var writer = new StreamWriter(filePrefix + ".out"))
                {
                    arguments = prepareArgs(fileName);
                    Program.Main(arguments);
                    outputs.Add(writer);
                }
                
            }
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
            return outputs;
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
                Assert.AreEqual(arguments[0].ToCharArray()[0], ".");
                Assert.AreEqual(arguments[0].ToCharArray()[1], "/");
                arguments.Remove(arguments[0]);
                return arguments.ToArray();
            }

            return null;
        }
    }
}