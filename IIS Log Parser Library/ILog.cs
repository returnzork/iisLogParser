using System;
using System.Collections.Generic;
using System.Text;

namespace returnzork.IIS_Log_Parser
{
    public interface ILog
    {
        bool IsValid { get; }
    }
}
