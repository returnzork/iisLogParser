using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace returnzork.IIS_Log_Parser
{
    internal static class LogModifier
    {
        internal static List<ILogItem> GetByClientIp() => throw new NotImplementedException();
        internal static List<ILogItem> GetByNotClientIp() => throw new NotImplementedException();
        internal static List<ILogItem> GetByMultipleClientIp() => throw new NotImplementedException();
        internal static List<ILogItem> GetByMultipleNotClientIp() => throw new NotImplementedException();
        internal static IEnumerable<ILogItem> GetByHTTPVerb(List<ILogItem> logs, string verb)
        {
            return logs.Where(x => x.HTTPVerb.ToLower() == verb.ToLower());
        }
        internal static List<ILogItem> GetByStatusCode() => throw new NotImplementedException();
    }
}
