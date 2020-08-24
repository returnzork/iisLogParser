using Microsoft.VisualStudio.TestTools.UnitTesting;
using returnzork.IIS_Log_Parser_Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace returnzork.IIS_Log_Parser_Tests
{
    [TestClass]
    public class LogDisplayTests
    {
        Type logDisplayType;
        object instance;
        List<object> logs;

        [TestInitialize]
        public void TestInit()
        {
            logDisplayType = Type.GetType("returnzork.IIS_Log_Parser.LogDisplay, IIS Log Parser");
            logs = TestInit_LoadLogs();
        }

        private List<object> TestInit_LoadLogs()
        {
            List<object> logs = new List<object>();
            for(int i = 0; i < 5; i++)
            {
                logs.Add(LogItemMock.GetGenericLog("127.0.0." + i, "GET"));
            }

            return logs;
        }

        [TestMethod]
        public void ShowByHTTPVerbTest()
        {
            //test loads 5 GET requests
            Assert.Inconclusive();
        }
    }
}
