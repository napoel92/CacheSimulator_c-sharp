using NUnit.Framework;
using System;
using System.Collections.Generic;


namespace CacheSimulator.Tests
{
    public static class AssertExtension
    {
        public static void TestPass(this Assert assert, List<int> resultsList)
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
            Assert.AreEqual("", results);
        }
    }
}