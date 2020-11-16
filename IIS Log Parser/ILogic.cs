using System;
using System.Collections.Generic;
using System.Text;

namespace returnzork.IIS_Log_Parser
{
    interface ILogic
    {
        public void Run();
        public event EventHandler<LogsChangedEventArgs> OnLogsChanged;
    }

    public class LogsChangedEventArgs : EventArgs
    { 
        public ILog[] NewLogs { get; }
        public LogsChangedEventArgs(ILog[] newLogs)
        {
            this.NewLogs = newLogs;
        }
    }
}
