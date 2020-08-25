using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace returnzork.IIS_Log_Parser_Tests
{
    [TestClass]
    public class HelperTests
    {
        Type helperType;
        MethodInfo ipSplit;

        [TestInitialize]
        public void TestInit()
        {
            helperType = Type.GetType("returnzork.IIS_Log_Parser.Helper, IIS Log Parser");
            ipSplit = helperType.GetMethod("IpSplit", BindingFlags.NonPublic | BindingFlags.Static);
        }

        [TestMethod]
        public void IpSplitSucessTest()
        {
            //valid ip set is [127.0.0.1, 127.0.0.2]
            const string VALID1 = "[127.0.0.1, 127.0.0.2]";
            object[] args1 = new object[] { VALID1, null };
            Assert.IsTrue((bool)ipSplit.Invoke(null, args1));
            Assert.AreEqual(2, (args1[1] as string[]).Length);
            Assert.AreEqual("127.0.0.1", (args1[1] as string[])[0]);
            Assert.AreEqual("127.0.0.2", (args1[1] as string[])[1]);
        }

        [TestMethod]
        public void IpSplitMissingBracketTest()
        {
            //missing bracket, returns false with a null out param
            const string INVALID1 = "127.0.0.1, 127.0.0.2]";
            object[] args1 = new object[] { INVALID1, null };
            Assert.IsFalse((bool)ipSplit.Invoke(null, args1));
           

            const string INVALID2 = "[127.0.0.1, 127.0.0.2";
            object[] args2 = new object[] { INVALID2, null };
            Assert.IsFalse((bool)ipSplit.Invoke(null, args2));
        }

        [TestMethod]
        public void IpSplitMIssingItemTest()
        {
            //only has 1 ip listed instead of the multiple required
            const string INVALID = "[127.0.0.1]";
            object[] args = new object[] { INVALID, null };
            Assert.IsFalse((bool)ipSplit.Invoke(null, args));
        }
    }
}
