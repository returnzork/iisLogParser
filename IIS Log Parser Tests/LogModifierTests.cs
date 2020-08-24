using Microsoft.VisualStudio.TestTools.UnitTesting;
using returnzork.IIS_Log_Parser_Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using returnzork.IIS_Log_Parser;

namespace returnzork.IIS_Log_Parser_Tests
{
    [TestClass]
    public class LogModifierTests
    {
        List<ILogItem> logs;
        Type logModifierType;
        Type logItemType;

        [TestInitialize]
        public void TestInit()
        {
            logModifierType = Type.GetType("returnzork.IIS_Log_Parser.LogModifier, IIS Log Parser");
            logItemType = Type.GetType("returnzork.IIS_Log_Parser.LogItem, IIS Log Parser");
            logs = TestInit_LoadLogs();
        }

        private List<ILogItem> TestInit_LoadLogs()
        {
            List<ILogItem> logs = new List<ILogItem>();
            for (int i = 0; i < 5; i++)
            {
                logs.Add(LogItemMock.GetGenericLog("127.0.0." + i, "GET"));
            }
            for(int i = 255; i > 250; i--)
            {
                logs.Add(LogItemMock.GetGenericLog("127.0.0." + i, "POST"));
            }

            return logs;
        }



        [TestMethod]
        public void GetByHTTPVerb()
        {
            //5 GET methods, 5 POST methods
            MethodInfo gethttp = logModifierType.GetMethod("GetByHTTPVerb", BindingFlags.Static | BindingFlags.NonPublic);
            IEnumerable<ILogItem> getResult = gethttp.Invoke(null, new object[] { logs, "GET" }) as IEnumerable<ILogItem>;

            Assert.AreEqual(5, getResult.Count());
            for(int i = 0; i < 5; i++)
            {
                Assert.IsTrue(getResult.Any(x => x.ClientIpAddr == $"127.0.0.{i}"));
            }

            IEnumerable<ILogItem> postResult = gethttp.Invoke(null, new object[] { logs, "POST" }) as IEnumerable<ILogItem>;
            Assert.AreEqual(5, postResult.Count());
            for(int i = 255; i > 250; i--)
            {
                Assert.IsTrue(postResult.Any(x => x.ClientIpAddr == $"127.0.0.{i}"));
            }
        }
    }
}
