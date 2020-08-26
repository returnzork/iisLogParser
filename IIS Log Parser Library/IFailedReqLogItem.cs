using System;
using System.Collections.Generic;
using System.Text;

namespace returnzork.IIS_Log_Parser
{
    public enum FailedAction { NONE, ABORT_REQUEST_ACTION, GENERAL_RESPONSE_HEADERS, RULE_EVALUATION_END }
    public interface IFailedReqLogItem
    {
        string Url { get; }
        string Host { get; }
        int StatusCode { get; }
        string UserAgent { get; }
        FailedAction Action { get; }
        string ActionName { get; }
        string RemoteAddress { get; }
    }
}
