using Microsoft.VisualStudio.TestTools.UnitTesting;

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


        private T GetProperty<T>(object instance, string property)
        {
            PropertyInfo pi = logItemType.GetProperty(property, BindingFlags.Instance | BindingFlags.NonPublic);
            return (T)pi.GetValue(instance);
        }


        [TestMethod]
        public void CreateDefaultLogItem()
        {
            object instance = Activator.CreateInstance(logItemType);
            Assert.IsFalse(GetProperty<bool>(instance, "IsValid"));
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
            object instance = ctor.Invoke(new object[] { new string[] { $"{year}-{month}-{day}", $"{hour}:{minute}:{second}", ServerIpAddr, HTTPVerb, Uri, Query, Port.ToString(), Username, ClientIpAddr, UserAgent, Referer, HTTPStatus.ToString(), HTTPSubStatus.ToString(), WindowsStatus, TimeTaken.TotalMilliseconds.ToString() } });


            Assert.IsTrue(GetProperty<bool>(instance, "IsValid"));
            Assert.AreEqual(Time, GetProperty<DateTime>(instance, "Time"));
            Assert.AreEqual(ServerIpAddr, GetProperty<string>(instance, "ServerIpAddr"));
            Assert.AreEqual(HTTPVerb, GetProperty<string>(instance, "HTTPVerb"));
            Assert.AreEqual(Uri, GetProperty<string>(instance, "Uri"));
            Assert.AreEqual(Query, GetProperty<string>(instance, "Query"));
            Assert.AreEqual(Port, GetProperty<int>(instance, "Port"));
            Assert.AreEqual(Username, GetProperty<string>(instance, "Username"));
            Assert.AreEqual(ClientIpAddr, GetProperty<string>(instance, "ClientIpAddr"));
            Assert.AreEqual(UserAgent, GetProperty<string>(instance, "UserAgent"));
            Assert.AreEqual(Referer, GetProperty<string>(instance, "Referer"));
            Assert.AreEqual(HTTPStatus, GetProperty<int>(instance, "HTTPStatus"));
            Assert.AreEqual(HTTPSubStatus, GetProperty<int>(instance, "HTTPSubStatus"));
            Assert.AreEqual(WindowsStatus, GetProperty<string>(instance, "WindowsStatus"));
            Assert.AreEqual(TimeTaken, GetProperty<TimeSpan>(instance, "TimeTaken"));
        }
    }
}
