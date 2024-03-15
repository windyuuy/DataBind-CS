﻿using NUnit.Framework;
using EngineAdapter.StringExt;

namespace TestDataBind.DataObserver.Interperter
{
    public class StringTest
    {
        [Test]
        public void TestStringSplit1()
        {
            var a = "abcd cedcdc";
            var b = "cd ";
            var c=a.Splits(b);
            Assert.AreEqual(c.Length, 2);
            Assert.AreEqual(c[0], "ab");
            Assert.AreEqual(c[1], "cedcdc");
        }
        [Test]
        public void TestStringSplit2()
        {
            var a = "xaabcabxcwfbcweffewbc";
            var b = "bc";
            var c = a.Splits(b);
            Assert.AreEqual(c.Length, 4);
            Assert.AreEqual(c[0], "xaa");
            Assert.AreEqual(c[1], "abxcwf");
            Assert.AreEqual(c[2], "weffew");
            Assert.AreEqual(c[3], "");
        }
    }
}
