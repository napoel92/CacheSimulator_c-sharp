using NUnit.Framework;
using System;

namespace CacheSimulator.Tests
{
    public class MemoryTests
    {
        [SetUp]
        public void Setup()
        {
        }



        [Test]
        public void cacheSizeTest()
        {
            //arrange
            CacheSimulator.CacheMemory c = new CacheMemory(1, 5, 0, 4);

            //act
            var l1Size = c.cacheSize;

            //assert
            Assert.AreEqual((int)Math.Pow(2, 5 - 0 - 1), l1Size);


        }



        [Test]
        public void waysNumeTest()
        {
            //arrange
            CacheSimulator.CacheMemory c = new CacheMemory(1, 5, 0, 4);

            //act
            var waysNum = c.waysNum;

            //assert
            Assert.AreEqual((int)Math.Pow(2, 1), waysNum);


        }
    }
}