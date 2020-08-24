using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace returnzork.IIS_Log_Parser
{
    internal static class LogModifier
    {
        internal static IEnumerable<ILogItem> GetByClientIp(List<ILogItem> logs, string ip)
        {
            return logs.Where(x => x.ClientIpAddr == ip);
        }

        internal static IEnumerable<ILogItem> GetByNotClientIp(List<ILogItem> logs, string ip)
        {
            return logs.Where(x => x.ClientIpAddr != ip);
        }
        internal static IEnumerable<ILogItem> GetByMultipleClientIp(List<ILogItem> logs, string ips)
        {
            if (!Helper.IpSplit(ips, out string[] split))
                throw new FormatException("ips is malformed");
            return logs.Where(x => split.Contains(x.ClientIpAddr));
        }
        internal static IEnumerable<ILogItem> GetByMultipleNotClientIp(List<ILogItem> logs, string ips)
        {
            if (!Helper.IpSplit(ips, out string[] split))
                throw new FormatException("ips is malformed");
            return logs.Where(x => !split.Contains(x.ClientIpAddr));
        }

        internal static IEnumerable<ILogItem> GetByHTTPVerb(List<ILogItem> logs, string verb)
        {
            return logs.Where(x => x.HTTPVerb.ToLower() == verb.ToLower());
        }

        internal static IEnumerable<ILogItem> GetByStatusCode(List<ILogItem> logs, int statusCode)
        {
            return logs.Where(x => x.HTTPStatus == statusCode);
        }
    }
}
