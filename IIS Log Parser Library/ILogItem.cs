using System;
using System.Collections.Generic;
using System.Text;

namespace returnzork.IIS_Log_Parser
{
    public interface ILogItem : ILog
    {
        string ServerIpAddr { get; }
        string HTTPVerb { get; }

        string Query { get; }
        int Port { get; }
        string Username { get; }
        string Referer { get; }
        string WindowsStatus { get; }
        TimeSpan TimeTaken { get; }
    }
}
