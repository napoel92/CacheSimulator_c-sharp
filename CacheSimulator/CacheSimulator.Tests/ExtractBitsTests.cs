using System;
using NUnit.Framework;

namespace CacheSimulator.Tests
{
    public class ExtractBitsTests
    {
        [SetUp]
        public void Setup()
        {
        }





        [Test]
        public void Extract32bitsTest()
        {
            //arrange
            uint number = 3196774569;

            //act
            uint res = CacheSimulator.Program.ExtractBits(number, 32, 1);

            //assert
            Assert.AreEqual(number, res);


        }

        [Test]
        public void Extract1bitsTest()
        {
            //arrange
            uint number = 3196774569;

            //act
            uint res1 = CacheSimulator.Program.ExtractBits(number, 1, 1);
            uint res2 = CacheSimulator.Program.ExtractBits(number, 1, 32);
            uint res3 = CacheSimulator.Program.ExtractBits(number, 1, 5);
            uint res4 = CacheSimulator.Program.ExtractBits(number, 1, 13);

            //assert
            Assert.AreEqual(1, res1);
            Assert.AreEqual(1, res2);
            Assert.AreEqual(0, res3);
            Assert.AreEqual(0, res4);


        }


        [Test]
        public void ExtractFewbitsTest()
        {
            //arrange
            uint number = 3196774569;

            //act
            uint res1 = CacheSimulator.Program.ExtractBits(number, 4, 1);
            uint res2 = CacheSimulator.Program.ExtractBits(number, 30, 3);
            uint res3 = CacheSimulator.Program.ExtractBits(number, 6, 5);
            uint res4 = CacheSimulator.Program.ExtractBits(number, 8, 13);

            //assert
            Assert.AreEqual(9, res1);
            Assert.AreEqual((3196774569-1)/4, res2);
            Assert.AreEqual(10, res3);
            Assert.AreEqual(174, res4);


        }





        [Test]
        public void Extract32bitsExceptionTest()
        {
            //arrange
            uint number = 2196774568;
            var res = false;
            //------------------------------------------------------
            //act
            try
            {
                CacheSimulator.Program.ExtractBits(number, 33, 1);
                
            }
            catch (ArgumentOutOfRangeException argExp)
            {
                res = true;
            }
            Assert.AreEqual(true, res);
            res = false;
            //------------------------------------------------------
            try
            {
                CacheSimulator.Program.ExtractBits(number, 32, 2);

            }
            catch (ArgumentOutOfRangeException argExp)
            {
                res = true;
            }
            Assert.AreEqual(true, res);
            res = false;
            //------------------------------------------------------
            try
            {
                CacheSimulator.Program.ExtractBits(number, 1, 33);

            }
            catch (ArgumentOutOfRangeException argExp)
            {
                res = true;
            }
            Assert.AreEqual(true, res);
            //------------------------------------------------------
            //assert
            res = true;


        }
    }
}