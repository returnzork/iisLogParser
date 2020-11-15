using System;
using System.Collections.Generic;
using System.Text;

namespace returnzork.IIS_Log_Parser
{
    public interface ILog
    {
        bool IsValid { get; }

        DateTime Time { get; }
        string Url { get; }
        string UserAgent { get; }
        int HTTPStatus { get; }
        int HTTPSubStatus { get; }
        string ClientIpAddr { get; }
    }
}
