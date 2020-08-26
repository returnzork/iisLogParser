using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using returnzork.IIS_Log_Parser;

namespace returnzork.IIS_Log_Parser_Tests
{
    [TestClass]
    public class FailedReqTests
    {
        [TestMethod]
        public void LoadLogFile()
        {
            const string LOGFILE = "log.txt";
            //log file format:
            //path to log.xml
            //expected Url
            //expected host
            //expected status code
            //expected UserAgent
            //expected Action
            //expected Action Name
            //expected Remote Address
            string log, url, host, userAgent, action, actionname, remoteAddress;
            int statusCode;

            using (StreamReader sr = new StreamReader(LOGFILE))
            {
                log = sr.ReadLine();
                url = sr.ReadLine();
                host = sr.ReadLine();
                statusCode = int.Parse(sr.ReadLine());
                userAgent = sr.ReadLine();
                action = sr.ReadLine();
                actionname = sr.ReadLine();
                remoteAddress = sr.ReadLine();
            }

            IFailedReqLogItem req = FailedReqLogItem.LoadFailedReq(log);
            Assert.AreEqual(url, req.Url);
            Assert.AreEqual(host, req.Host);
            Assert.AreEqual(statusCode, req.StatusCode);
            Assert.AreEqual(userAgent, req.UserAgent);
            Assert.AreEqual(action, req.Action.ToString());
            Assert.AreEqual(actionname, req.ActionName);
            Assert.AreEqual(remoteAddress, req.RemoteAddress);
        }
    }
}
