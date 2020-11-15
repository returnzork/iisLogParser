using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

using System.Linq;

namespace returnzork.IIS_Log_Parser
{
    enum FailedRequestMenuItem
    {
        NONE, Exit, ResetLogFilter,
        Display,
        IgnoreByUrl, IgnoreByUserAgent, IgnoreByIP, IgnoreByHost
    }


    internal class FailedRequestDisplay
    {
        IFailedReqLogItem[] loadedLogs;
        internal FailedRequestDisplay(string dir)
        {
            //load all of the xml files
            var files = Directory.GetFiles(dir, "*.xml");
            loadedLogs = new IFailedReqLogItem[files.Length];
            for(int i = 0; i < files.Length; i++)
            {
                loadedLogs[i] = FailedReqLogItem.LoadFailedReq(files[i]);
            }
        }

        internal void Display()
        {
            IEnumerable<IFailedReqLogItem> currentLogFilter = loadedLogs.AsEnumerable();
            while (true)
            {
                var option = DisplayMenu();
                switch (option)
                {
                    case FailedRequestMenuItem.Exit:
                        return;

                    case FailedRequestMenuItem.ResetLogFilter:
                        currentLogFilter = loadedLogs.AsEnumerable();
                        break;

                    case FailedRequestMenuItem.Display:
                        DisplayLogs(currentLogFilter);
                        break;

                    case FailedRequestMenuItem.IgnoreByUrl:
                        IgnoreByUrl(ref currentLogFilter);
                        break;
                    case FailedRequestMenuItem.IgnoreByUserAgent:
                        IgnoreByUserAgent(ref currentLogFilter);
                        break;

                    case FailedRequestMenuItem.IgnoreByIP:
                        IgnoreByIP(ref currentLogFilter);
                        break;

                    case FailedRequestMenuItem.IgnoreByHost:
                        IgnoreByHost(ref currentLogFilter);
                        break;
                }
            }
        }

        private void DisplayLogs(IEnumerable<IFailedReqLogItem> logs)
        {
            foreach(var li in logs)
            {
                Console.WriteLine(li.ClientIpAddr + " - " + li.Time);
                Console.WriteLine($"\t{li.Url}");
                Console.WriteLine($"\t{li.UserAgent}");
                Console.WriteLine($"\t{li.ActionName}");
            }
        }


        private void IgnoreByUrl(ref IEnumerable<IFailedReqLogItem> logs)
        {
            Console.WriteLine("Enter url to ignore: ");
            string url = Console.ReadLine();

            logs = logs.Where(x => x.Url != url);
        }

        private void IgnoreByUserAgent(ref IEnumerable<IFailedReqLogItem> logs)
        {
            Console.WriteLine("Enter user agent to ignore: ");
            string agent = Console.ReadLine();

            logs = logs.Where(x => x.UserAgent != agent);
        }

        private void IgnoreByIP(ref IEnumerable<IFailedReqLogItem> logs)
        {
            Console.WriteLine("Enter IP address to ignore: ");
            string ip = Console.ReadLine();

            logs = logs.Where(x => x.ClientIpAddr != ip);
        }

        private void IgnoreByHost(ref IEnumerable<IFailedReqLogItem> logs)
        {
            Console.WriteLine("Enter host to ignore: ");
            string host = Console.ReadLine();

            logs = logs.Where(x => x.Host != host);
        }


        private FailedRequestMenuItem DisplayMenu()
        {
            Console.WriteLine("-1 - Exit");
            Console.WriteLine("0 - Display");
            Console.WriteLine("1 - Reset log filters");
            Console.WriteLine("4 - Add ignore by url");
            Console.WriteLine("44 - Add ignore by user agent");
            Console.WriteLine("444 - Add ignore by IP address");
            Console.WriteLine("4444 - Add ignore by host");

            if(!int.TryParse(Console.ReadLine(), out int result))
            {
                Console.WriteLine("Invalid entry");
                return FailedRequestMenuItem.NONE;
            }
            switch(result)
            {
                case -1:
                    return FailedRequestMenuItem.Exit;

                case 0:
                    return FailedRequestMenuItem.Display;

                case 1:
                    return FailedRequestMenuItem.ResetLogFilter;

                case 4:
                    return FailedRequestMenuItem.IgnoreByUrl;
                case 44:
                    return FailedRequestMenuItem.IgnoreByUserAgent;
                case 444:
                    return FailedRequestMenuItem.IgnoreByIP;
                case 4444:
                    return FailedRequestMenuItem.IgnoreByHost;

                default:
                    return FailedRequestMenuItem.NONE;
            }
        }
    }
}
