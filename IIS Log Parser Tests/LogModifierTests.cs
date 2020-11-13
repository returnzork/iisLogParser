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

        [TestInitialize]
        public void TestInit_LoadLogs()
        {
            logs = new List<ILogItem>();
            //127.0.0.0 -> 127.0.0.4 GET 200
            for (int i = 0; i < 5; i++)
            {
                logs.Add(LogItemMock.GetGenericLog("127.0.0." + i, "GET", 200));
            }
            //127.0.0.251 -> 127.0.0.255 POST 200
            for(int i = 255; i > 250; i--)
            {
                logs.Add(LogItemMock.GetGenericLog("127.0.0." + i, "POST", 200));
            }

            //5 sets of 127.0.0.55 TEST 404
            for(int i = 50; i < 55; i++)
            {
                logs.Add(LogItemMock.GetGenericLog("127.0.0.55", "TEST", 404));
            }

            //5 sets of 127.0.0.99 OTHER 777, URI=/Test/Path/TestLogFile
            for(int i = 0; i < 5; i++)
            {
                logs.Add(LogItemMock.GetGenericLog("127.0.0.99", "OTHER", 777, "/Test/Path/"));
            }
        }



        [TestMethod]
        public void GetByClientIp()
        {
            //check the 5 sets of 127.0.0.55 we added
            IEnumerable<ILogItem> result = LogModifier.GetByClientIp(logs, "127.0.0.55");
            Assert.AreEqual(5, result.Count());
        }

        [TestMethod]
        public void GetByNotClientIp()
        {
            //get all results of NOT 127.0.0.55 -> 20 items added - 5 of 127.0.0.55 = 15 matches
            IEnumerable<ILogItem> result = LogModifier.GetByNotClientIp(logs, "127.0.0.55");
            Assert.AreEqual(15, result.Count());
        }

        [TestMethod]
        public void GetByMultipleClientIp()
        {
            //get matches of [127.0.0.1, 127.0.0.3, 127.0.0.252, 127.0.0.254] = 4 matches
            IEnumerable<ILogItem> result = LogModifier.GetByMultipleClientIp(logs, "[127.0.0.1, 127.0.0.3, 127.0.0.252, 127.0.0.254]");
            Assert.AreEqual(4, result.Count());

            //check the error handling
            Assert.ThrowsException<FormatException>(() => LogModifier.GetByMultipleClientIp(logs, "[127.0.0.1, 127.0.0.3"));
            Assert.ThrowsException<FormatException>(() => LogModifier.GetByMultipleClientIp(logs, "127.0.0.1, 127.0.0.3]"));
        }

        [TestMethod]
        public void GetByMultipleClientIpSingleEntry()
        {
            //call get multiple with only 1 ip specified, [127.0.0.1] = 1 match
            IEnumerable<ILogItem> result = LogModifier.GetByMultipleClientIp(logs, "[127.0.0.1]");
            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void GetByMultipleNotClientIp()
        {
            //get matches of not [127.0.0.1, 127.0.0.55] = 20 - 1 **[127.0.0.1]** - 5 **[127.0.0.55]** = 14 matches
            IEnumerable<ILogItem> result = LogModifier.GetByMultipleNotClientIp(logs, "[127.0.0.1, 127.0.0.55]");
            Assert.AreEqual(14, result.Count());

            //check the error handling
            Assert.ThrowsException<FormatException>(() => LogModifier.GetByMultipleNotClientIp(logs, "[127.0.0.1, 127.0.0.55"));
            Assert.ThrowsException<FormatException>(() => LogModifier.GetByMultipleNotClientIp(logs, "127.0.0.1, 127.0.0.55]"));
        }

        [TestMethod]
        public void GetByMultipleNotClientIpSingleEntry()
        {
            //call get multiple not with only 1 ip specified, [127.0.0.1] = 19 matches
            IEnumerable<ILogItem> result = LogModifier.GetByMultipleNotClientIp(logs, "[127.0.0.1]");
            Assert.AreEqual(19, result.Count());
        }

        [TestMethod]
        public void GetByHTTPVerb()
        {
            //5 GET methods, 5 POST methods
            IEnumerable<ILogItem> getResult = LogModifier.GetByHTTPVerb(logs, "GET");

            Assert.AreEqual(5, getResult.Count());
            for(int i = 0; i < 5; i++)
            {
                Assert.IsTrue(getResult.Any(x => x.ClientIpAddr == $"127.0.0.{i}"));
            }

            IEnumerable<ILogItem> postResult = LogModifier.GetByHTTPVerb(logs, "POST");
            Assert.AreEqual(5, postResult.Count());
            for(int i = 255; i > 250; i--)
            {
                Assert.IsTrue(postResult.Any(x => x.ClientIpAddr == $"127.0.0.{i}"));
            }
        }

        [TestMethod]
        public void GetByStatusCode()
        {
            //tests has 10 - 200's and 5 - 404's
            IEnumerable<ILogItem> result200 = LogModifier.GetByStatusCode(logs, 200);
            Assert.AreEqual(10, result200.Count());

            IEnumerable<ILogItem> result404 = LogModifier.GetByStatusCode(logs, 404);
            Assert.AreEqual(5, result404.Count());
        }

        [TestMethod]
        public void GetByPath()
        {
            //20 total items
            //15 items have uri of /TestLogFile.txt
            //5 have /Test/Path/TestLogFile.txt

            //match root only, gives 15 items
            IEnumerable<ILogItem> result = LogModifier.GetByPath(logs, "/");
            Assert.AreEqual(15, result.Count());

            //match /Test/Path/, gives 5 items
            result = LogModifier.GetByPath(logs, "/Test/Path/");
            Assert.AreEqual(5, result.Count());

            //match /Test/, gives 0 items
            result = LogModifier.GetByPath(logs, "/Test/");
            Assert.AreEqual(0, result.Count());
        }
    }
}
