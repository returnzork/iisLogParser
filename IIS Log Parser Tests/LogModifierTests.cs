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

        [TestInitialize]
        public void TestInit()
        {
            logModifierType = Type.GetType("returnzork.IIS_Log_Parser.LogModifier, IIS Log Parser");
            logs = TestInit_LoadLogs();
        }

        private List<ILogItem> TestInit_LoadLogs()
        {
            List<ILogItem> logs = new List<ILogItem>();
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

            return logs;
        }



        [TestMethod]
        public void GetByClientIp()
        {
            //check the 5 sets of 127.0.0.55 we added
            MethodInfo method = logModifierType.GetMethod("GetByClientIp", BindingFlags.Static | BindingFlags.NonPublic);
            IEnumerable<ILogItem> result = method.Invoke(null, new object[] { logs, "127.0.0.55" }) as IEnumerable<ILogItem>;

            Assert.AreEqual(5, result.Count());
        }

        [TestMethod]
        public void GetByNotClientIp()
        {
            //get all results of NOT 127.0.0.55 -> 15 items added - 5 of 127.0.0.55 = 10 matches
            MethodInfo method = logModifierType.GetMethod("GetByNotClientIp", BindingFlags.Static | BindingFlags.NonPublic);
            IEnumerable<ILogItem> result = method.Invoke(null, new object[] { logs, "127.0.0.55" }) as IEnumerable<ILogItem>;
            Assert.AreEqual(10, result.Count());
        }

        [TestMethod]
        public void GetByMultipleClientIp()
        {
            //get matches of [127.0.0.1, 127.0.0.3, 127.0.0.252, 127.0.0.254] = 4 matches
            MethodInfo method = logModifierType.GetMethod("GetByMultipleClientIp", BindingFlags.Static | BindingFlags.NonPublic);
            IEnumerable<ILogItem> result = method.Invoke(null, new object[] { logs, "[127.0.0.1, 127.0.0.3, 127.0.0.252, 127.0.0.254]" }) as IEnumerable<ILogItem>;
            Assert.AreEqual(4, result.Count());

            //check the error handling
            Assert.ThrowsException<FormatException>(() =>
            {
                try
                {
                    method.Invoke(null, new object[] { logs, "[bad format" });
                }
                catch(TargetInvocationException tie)
                {
                    throw tie.InnerException;
                }
            });
        }

        [TestMethod]
        public void GetByMultipleNotClientIp()
        {
            //get matches of not [127.0.0.1, 127.0.0.55] = 15 - 1 [127.0.0.1] - 5 [127.0.0.55] = 9 matches
            MethodInfo method = logModifierType.GetMethod("GetByMultipleNotClientIp", BindingFlags.Static | BindingFlags.NonPublic);
            IEnumerable<ILogItem> result = method.Invoke(null, new object[] { logs, "[127.0.0.1, 127.0.0.55]" }) as IEnumerable<ILogItem>;
            Assert.AreEqual(9, result.Count());

            //check the error handling
            Assert.ThrowsException<FormatException>(() =>
            {
                try
                {
                    method.Invoke(null, new object[] { logs, "[bad format" });
                }
                catch (TargetInvocationException tie)
                {
                    throw tie.InnerException;
                }
            });
        }

        [TestMethod]
        public void GetByHTTPVerb()
        {
            //5 GET methods, 5 POST methods
            MethodInfo method = logModifierType.GetMethod("GetByHTTPVerb", BindingFlags.Static | BindingFlags.NonPublic);
            IEnumerable<ILogItem> getResult = method.Invoke(null, new object[] { logs, "GET" }) as IEnumerable<ILogItem>;

            Assert.AreEqual(5, getResult.Count());
            for(int i = 0; i < 5; i++)
            {
                Assert.IsTrue(getResult.Any(x => x.ClientIpAddr == $"127.0.0.{i}"));
            }

            IEnumerable<ILogItem> postResult = method.Invoke(null, new object[] { logs, "POST" }) as IEnumerable<ILogItem>;
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
            MethodInfo method = logModifierType.GetMethod("GetByStatusCode", BindingFlags.Static | BindingFlags.NonPublic);

            IEnumerable<ILogItem> result200 = method.Invoke(null, new object[] { logs, 200 }) as IEnumerable<ILogItem>;
            Assert.AreEqual(10, result200.Count());

            IEnumerable<ILogItem> result404 = method.Invoke(null, new object[] { logs, 404 }) as IEnumerable<ILogItem>;
            Assert.AreEqual(5, result404.Count());
        }
    }
}
