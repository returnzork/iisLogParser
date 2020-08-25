﻿using returnzork.IIS_Log_Parser;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace returnzork.IIS_Log_Parser_Tests.Mocks
{
    static class LogItemMock
    {
        static Type logItemType;

        internal static ILogItem GetGenericLog(string clientIp, string verb, int status)
        {
            if(logItemType == null)
                logItemType = Type.GetType("returnzork.IIS_Log_Parser.LogItem, IIS Log Parser");


            DateTime Time = DateTime.Now;
            string ServerIpAddr = "127.0.0.1";
            string HTTPVerb = verb;
            string Uri = "/TestLogFile.txt";
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

            ILogItem instance = new LogItem(new string[] { $"{Time.Year}-{Time.Month}-{Time.Day}", $"{Time.Hour}:{Time.Minute}:{Time.Second}", ServerIpAddr, HTTPVerb, Uri, Query, Port.ToString(), Username, 
                ClientIpAddr, UserAgent, Referer, HTTPStatus.ToString(), HTTPSubStatus.ToString(), WindowsStatus, TimeTaken.TotalMilliseconds.ToString() });
            return (ILogItem)instance;
        }
    }
}
