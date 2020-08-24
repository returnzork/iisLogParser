using Microsoft.VisualStudio.TestTools.UnitTesting;
using returnzork.IIS_Log_Parser;
using System;
using System.Reflection;

namespace returnzork.IIS_Log_Parser_Tests
{
    [TestClass]
    public class LogItemTests
    {
        Type logItemType;
        [TestInitialize]
        public void TestInit()
        {
            logItemType = Type.GetType("returnzork.IIS_Log_Parser.LogItem, IIS Log Parser");
        }

        [TestMethod]
        public void CreateDefaultLogItem()
        {
            ILogItem instance = Activator.CreateInstance(logItemType) as ILogItem;
            Assert.IsFalse(instance.IsValid);
        }

        [TestMethod]
        public void CreateLogItem()
        {
            //values we want set

            //format used for Time
            int year = 2020, month = 8, day = 23, hour = 19, minute = 45, second = 5;
            DateTime Time = new DateTime(year, month, day, hour, minute, second);
            string ServerIpAddr = "127.0.0.1";
            string HTTPVerb = "GET";
            string Uri = "/TestLogFile.txt";
            string Query = "X-ARR-CACHE-HIT=";
            int Port = 80;
            string Username = "testuser";
            string ClientIpAddr = "127.0.0.255";
            string UserAgent = "FakeUserAgent";
            string Referer = "none";
            int HTTPStatus = 200;
            int HTTPSubStatus = 0;
            string WindowsStatus = "0";
            TimeSpan TimeTaken = new TimeSpan(0);




            ConstructorInfo ctor = logItemType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(string[]) }, null);
            ILogItem instance = ctor.Invoke(new object[] { new string[] { $"{year}-{month}-{day}", $"{hour}:{minute}:{second}", ServerIpAddr, HTTPVerb, Uri, Query, Port.ToString(), Username, ClientIpAddr, UserAgent, Referer, HTTPStatus.ToString(), HTTPSubStatus.ToString(), WindowsStatus, TimeTaken.TotalMilliseconds.ToString() } }) as ILogItem;


            Assert.IsTrue(instance.IsValid);
            Assert.AreEqual(Time, instance.Time);
            Assert.AreEqual(ServerIpAddr, instance.ServerIpAddr);
            Assert.AreEqual(HTTPVerb, instance.HTTPVerb);
            Assert.AreEqual(Uri, instance.Uri);
            Assert.AreEqual(Query, instance.Query);
            Assert.AreEqual(Port, instance.Port);
            Assert.AreEqual(Username, instance.Username);
            Assert.AreEqual(ClientIpAddr, instance.ClientIpAddr);
            Assert.AreEqual(UserAgent, instance.UserAgent);
            Assert.AreEqual(Referer, instance.Referer);
            Assert.AreEqual(HTTPStatus, instance.HTTPStatus);
            Assert.AreEqual(HTTPSubStatus, instance.HTTPSubStatus);
            Assert.AreEqual(WindowsStatus, instance.WindowsStatus);
            Assert.AreEqual(TimeTaken, instance.TimeTaken);
        }
    }
}
