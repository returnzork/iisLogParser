using System;
using System.Collections.Generic;
using System.Text;

namespace returnzork.IIS_Log_Parser
{
    public enum FailedAction { NONE, ABORT_REQUEST_ACTION, URL_CHANGED, RULE_EVALUATION_END, MODULE_SET_RESPONSE_ERROR_STATUS }
    public interface IFailedReqLogItem : ILog
    {
        string FileName { get; }

        string Host { get; }
        FailedAction Action { get; }
        string ActionName { get; }
    }
}
