using System;
using System.Collections.Generic;
using System.Text;

namespace returnzork.IIS_Log_Parser
{
    public interface ILogItem : ILog
    {
        DateTime Time { get; }
        string ServerIpAddr { get; }
        string HTTPVerb { get; }
        string Uri { get; }
        string Query { get; }
        int Port { get; }
        string Username { get; }
        string ClientIpAddr { get; }
        string UserAgent { get; }
        string Referer { get; }
        int HTTPStatus { get; }
        int HTTPSubStatus { get; }
        string WindowsStatus { get; }
        TimeSpan TimeTaken { get; }
    }
}
