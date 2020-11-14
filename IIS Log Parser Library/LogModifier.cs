using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace returnzork.IIS_Log_Parser
{
    public static class LogModifier
    {
        public static IEnumerable<ILogItem> GetByClientIp(List<ILogItem> logs, string ip)
        {
            return logs.Where(x => x.ClientIpAddr == ip);
        }

        public static IEnumerable<ILogItem> GetByNotClientIp(List<ILogItem> logs, string ip)
        {
            return logs.Where(x => x.ClientIpAddr != ip);
        }

        public static IEnumerable<ILogItem> GetByMultipleClientIp(List<ILogItem> logs, string ips)
        {
            if (!Helper.IpSplit(ips, out string[] split))
                throw new FormatException("ips is malformed");
            return logs.Where(x => split.Contains(x.ClientIpAddr));
        }

        public static IEnumerable<ILogItem> GetByMultipleNotClientIp(List<ILogItem> logs, string ips)
        {
            if (!Helper.IpSplit(ips, out string[] split))
                throw new FormatException("ips is malformed");
            return logs.Where(x => !split.Contains(x.ClientIpAddr));
        }

        public static IEnumerable<ILogItem> GetByHTTPVerb(List<ILogItem> logs, string verb)
        {
            return logs.Where(x => x.HTTPVerb.ToLower() == verb.ToLower());
        }

        public static IEnumerable<ILogItem> GetByStatusCode(List<ILogItem> logs, int statusCode)
        {
            return logs.Where(x => x.HTTPStatus == statusCode);
        }

        public static IEnumerable<ILogItem> GetByPath(List<ILogItem> logs, string path)
        {
            foreach(ILogItem item in logs)
            {
                //get the various components of the path
                string uri = item.Uri;
                string[] split = uri.Split('/', StringSplitOptions.RemoveEmptyEntries);

                //if split length is 0, the root directory with no file was accessed
                //if we are not matching the root directory (with or without wildcards), we can skip this
                if(split.Length == 0)
                {
                    if (path != "/" && path != "/*")
                        continue;
                }
                //each index is a part of the path (hopefully we never have a non-html-escaped '/' character)
                //final index is always the file that was accessed? (TODO CHECK THIS)

                //iterate over the path that was specified to see if this matches
                bool matches = false;
                if (split.Length == 1)
                {
                    //we are in the root dir
                    //did we specify root?
                    if (path == "/" || path == "/*")
                        matches = true;
                }
                else
                {
                    //split the path we specified to match
                    string[] matchIn = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

                    //if they do not have the same amount of directory components, they cannot match
                    //if the length does not match, but the last item is a wildcard, it can match
                    //if the matchIn length is 0, that means we ONLY want items in the root
                    if (matchIn.Length == split.Length - 1 || (matchIn.Length > 0 ? matchIn[matchIn.Length - 1] == "*" : false))
                    {
                        //set that it probably is a match
                        matches = true;
                        //iterate over each portion, if it doesn't match, set so, and exit
                        for (int i = 0; i < split.Length - 1; i++)
                        {
                            if(i == matchIn.Length - 1 && matchIn[i] == "*")
                            {
                                //wildcard match, it does match so exit
                                break;
                            }
                            if (split[i] != matchIn[i])
                            {
                                matches = false;
                                break;
                            }
                        }
                    }
                }

                //if this was a match, return it
                if(matches)
                {
                    yield return item;
                }    
            }
        }
    }
}
