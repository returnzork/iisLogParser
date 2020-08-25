using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using returnzork.IIS_Log_Parser;

namespace returnzork.IIS_Log_Parser_Tests
{
    [TestClass]
    public class HelperTests
    {
        [TestMethod]
        public void IpSplitSucessTest()
        {
            //valid ip set is [127.0.0.1, 127.0.0.2]
            const string VALID1 = "[127.0.0.1, 127.0.0.2]";
            Assert.IsTrue(Helper.IpSplit(VALID1, out string[] result));
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("127.0.0.1", result[0]);
            Assert.AreEqual("127.0.0.2", result[1]);
        }

        [TestMethod]
        public void IpSplitMissingBracketTest()
        {
            //missing bracket, returns false with a null out param
            const string INVALID1 = "127.0.0.1, 127.0.0.2]";
            Assert.IsFalse(Helper.IpSplit(INVALID1, out _));

            const string INVALID2 = "[127.0.0.1, 127.0.0.2";
            Assert.IsFalse(Helper.IpSplit(INVALID2, out _));
        }

        [TestMethod]
        public void IpSplitMIssingItemTest()
        {
            //only has 1 ip listed instead of the multiple required
            const string INVALID = "[127.0.0.1]";
            Assert.IsFalse(Helper.IpSplit(INVALID, out _));
        }
    }
}
