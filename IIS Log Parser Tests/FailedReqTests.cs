using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace returnzork.IIS_Log_Parser_Tests
{
    [TestClass]
    public class FailedReqTests
    {
        [TestInitialize]
        public void Test_Init()
        {
            const string LOGFILE = "log.txt";
            //log file format:
            //path to log.xml
            //expected Url
            //expected host
            //expected UserAgent
            //expected Action
            //expected Action Name
            Assert.Fail();
        }
    }
}
