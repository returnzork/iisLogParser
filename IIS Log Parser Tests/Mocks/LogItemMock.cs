using returnzork.IIS_Log_Parser;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace returnzork.IIS_Log_Parser_Tests.Mocks
{
    static class LogItemMock
    {
        static Type logItemType;

        /// <summary>
        /// Create a ILogItem entry
        /// </summary>
        /// <param name="clientIp">Ip address to set (ex 127.0.0.1)</param>
        /// <param name="verb">Http verb (ex Get)</param>
        /// <param name="status">Http status code (ex 500)</param>
        /// <param name="path">Path client requested (ex /folder1/folder2/)</param>
        /// <param name="file">File client requested (ex myfile.txt)</param>
        /// <returns>ILogItem with specified parameters</returns>
        internal static ILogItem GetGenericLog(string clientIp, string verb, int status, string path = "", string file = "TestLogFile.txt")
        {
            //reflection loading
            if(logItemType == null)
                logItemType = Type.GetType("returnzork.IIS_Log_Parser.LogItem, IIS Log Parser");

            //if the path the user specified ends with a forwardslash, remove it
            if (path.EndsWith('/'))
                path = path.Substring(0, path.Length - 1);

            //set the values of the ILogItem with what the user specified
            DateTime Time = DateTime.Now;
            string ServerIpAddr = "127.0.0.1";
            string HTTPVerb = verb;
            string Uri = path + "/" + file;
            string Query = "X-ARR-CACHE-HIT=";
            int Port = 80;
            string Username = "testuser";
            string ClientIpAddr = clientIp;
            string UserAgent = "FakeUserAgent";
            string Referer = "none";
            int HTTPStatus = status;
            int HTTPSubStatus = 0;
            string WindowsStatus = "0";
            TimeSpan TimeTaken = new TimeSpan(0);

            //reflection instantiation of item
            ILogItem instance = LogItem.Create(new string[] { $"{Time.Year}-{Time.Month}-{Time.Day}", $"{Time.Hour}:{Time.Minute}:{Time.Second}", ServerIpAddr, HTTPVerb, Uri, Query, Port.ToString(), Username,
                ClientIpAddr, UserAgent, Referer, HTTPStatus.ToString(), HTTPSubStatus.ToString(), WindowsStatus, TimeTaken.TotalMilliseconds.ToString() });
            return instance;
        }
    }
}
