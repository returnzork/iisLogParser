using System;
using System.Collections.Generic;
using System.Text;

namespace returnzork.IIS_Log_Parser
{
    public enum FailedAction { NONE, ABORT_REQUEST_ACTION, URL_CHANGED, RULE_EVALUATION_END }
    public interface IFailedReqLogItem
    {
        string FileName { get; }

        string Url { get; }
        string Host { get; }
        int StatusCode { get; }
        int StatusCodeSubCode { get; }
        string UserAgent { get; }
        FailedAction Action { get; }
        string ActionName { get; }
        string RemoteAddress { get; }
        DateTime Time { get; }
    }
}
