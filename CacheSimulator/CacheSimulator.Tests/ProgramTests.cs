using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CacheSimulator.Tests
{
    public class ProgramTests
    {
        [SetUp]
        public void Setup()
        {
        }



        [Test]
        public void ProgramTests1()
        {
            //arrange
            var prog = new Program();
            //string[] files = Directory.GetFiles("path");
            string[] args = prepareArgs("file");


            //act

            //assert






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