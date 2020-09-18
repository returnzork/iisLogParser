using Microsoft.VisualStudio.TestTools.UnitTesting;
using returnzork.IIS_Log_Parser;
using System;
using System.IO;

namespace returnzork.IIS_Log_Parser_Tests
{
    [TestClass]
    public class FailedReqTests
    {
        [TestMethod]
        public void LoadLogFile()
        {
            //get the log files to test
            string[] LOGFILES = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "TESTFILES"), "log*.txt");
            //log file format:
            //path to log.xml
            //expected Url
            //expected host
            //expected status code (eg 404.6 -> 404)
            //expected sub status code (eg 404.6 -> 6)
            //expected UserAgent
            //expected Action
            //expected Action Name
            //expected Remote Address
            //log time
            foreach (string lf in LOGFILES)
            {
                if (!File.Exists(lf))
                    Assert.Inconclusive("Test Log file not found");
                string log, url, host, userAgent, action, actionname, remoteAddress;
                int statusCode, subStatusCode;
                DateTime time;

                using (StreamReader sr = new StreamReader(lf))
                {
                    log = sr.ReadLine();
                    url = sr.ReadLine();
                    host = sr.ReadLine();
                    statusCode = int.Parse(sr.ReadLine());
                    subStatusCode = int.Parse(sr.ReadLine());
                    userAgent = sr.ReadLine();
                    action = sr.ReadLine();
                    actionname = sr.ReadLine();
                    remoteAddress = sr.ReadLine();
                    time = DateTime.Parse(sr.ReadLine());
                }

                if (!File.Exists(log))
                    Assert.Inconclusive("XML Log file not found");

                IFailedReqLogItem req = FailedReqLogItem.LoadFailedReq(log);
                Assert.AreEqual(url, req.Url);
                Assert.AreEqual(host, req.Host);
                Assert.AreEqual(statusCode, req.StatusCode);
                Assert.AreEqual(subStatusCode, req.StatusCodeSubCode);
                Assert.AreEqual(userAgent, req.UserAgent);
                Assert.AreEqual(action, req.Action.ToString());
                Assert.AreEqual(actionname, req.ActionName);
                Assert.AreEqual(remoteAddress, req.RemoteAddress);
                Assert.AreEqual(time, req.Time);
            }
        }
    }

}
