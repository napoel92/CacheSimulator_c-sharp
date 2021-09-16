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
            //arrange - reach the tests directory
            string cwd = Directory.GetCurrentDirectory();
            for (int i = 0; i<3; ++i)
            {
                cwd = Path.GetDirectoryName(cwd);
            }
            string[] toCombine = { cwd, "tests" };
            string pdp = Path.Combine(toCombine);
            string[] files = Directory.GetFiles(pdp);











            //var prog = new Program();
            //string[] args = prepareArgs("file");


            //act

            //assert




            return;

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