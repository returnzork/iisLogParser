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

        [TestInitialize]
        public void TestInit()
        {
            helperType = Type.GetType("returnzork.IIS_Log_Parser.Helper, IIS Log Parser");
        }

        [TestMethod]
        public void IpSplitTest()
        {
            MethodInfo ipSplit = helperType.GetMethod("IpSplit", BindingFlags.NonPublic | BindingFlags.Static);

            //valid ip set is [127.0.0.1, 127.0.0.2]
            const string VALID1 = "[127.0.0.1, 127.0.0.2]";
            object[] args1 = new object[] { VALID1, null };
            Assert.IsTrue((bool)ipSplit.Invoke(null, args1));
            Assert.AreEqual(2, (args1[1] as string[]).Length);
            Assert.AreEqual("127.0.0.1", (args1[1] as string[])[0]);
            Assert.AreEqual("127.0.0.2", (args1[1] as string[])[1]);
        }
    }
}
