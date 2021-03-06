﻿using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

using System.Linq;

namespace returnzork.IIS_Log_Parser
{
    internal class FailedRequestDisplay : ILogDisplay
    {
        List<IFailedReqLogItem> loadedLogs;
        Logic<IFailedReqLogItem> logic;


        public static IEnumerable<IFailedReqLogItem> LoadFailedReqLogDir(string dir)
        {
            //load all of the xml files
            var files = Directory.GetFiles(dir, "*.xml");
            List<IFailedReqLogItem> logs = new List<IFailedReqLogItem>(files.Length);

            System.Threading.Tasks.Parallel.For(0, files.Length, (i) =>
            {
                if (File.Exists(files[i]))
                {
                    var frqli = FailedReqLogItem.LoadFailedReq(files[i]);
                    lock (logs)
                        logs.Add(frqli);
                }
            });

            return logs.OrderBy(x => x.Time);
        }


        public FailedRequestDisplay(Logic<IFailedReqLogItem> logic)
        {
            this.logic = logic;
            logic.OnLogsChanged += Logic_OnLogsChanged;
        }

        private void Logic_OnLogsChanged(object sender, LogsChangedEventArgs e)
        {
            if (e.NewLogs is IFailedReqLogItem[] ifrqliA)
            {
                this.loadedLogs = ifrqliA.ToList();
            }
        }

        public void ConsumeMenuItem(MenuEntry item)
        {
            switch (item)
            {
                case MenuEntry.Exit:
                    return;

                case MenuEntry.FRQIgnoreByUrl:
                    IgnoreByUrl();
                    break;
                case MenuEntry.FRQIgnoreByUserAgent:
                    IgnoreByUserAgent();
                    break;

                case MenuEntry.FRQIgnoreByHost:
                    IgnoreByHost();
                    break;
            }
        }

        public MenuEntry GetMenuItem()
        {
            if (!int.TryParse(Console.ReadLine(), out int result))
            {
                Console.WriteLine("Invalid entry");
                return MenuEntry.NONE;
            }
            switch (result)
            {
                case -1:
                    return MenuEntry.Exit;

                case 0:
                    return MenuEntry.DisplayLogs;

                case 1:
                    return MenuEntry.ResetLogFilters;

                case 4:
                    return MenuEntry.FRQIgnoreByUrl;
                case 44:
                    return MenuEntry.FRQIgnoreByUserAgent;
                case 444:
                    return MenuEntry.GlobalIgnore;
                case 4444:
                    return MenuEntry.FRQIgnoreByHost;

                default:
                    return MenuEntry.NONE;
            }
        }

        public void ShowMenu()
        {
            Console.WriteLine("-1 - Exit");
            Console.WriteLine("0 - Display");
            Console.WriteLine("1 - Reset log filters");
            Console.WriteLine("4 - Add ignore by url");
            Console.WriteLine("44 - Add ignore by user agent");
            Console.WriteLine("444 - Add ignore by IP address");
            Console.WriteLine("4444 - Add ignore by host");
        }



        private void IgnoreByUrl()
        {
            Console.WriteLine("Enter url to ignore: ");
            string url = Console.ReadLine();

            var toFilter = loadedLogs.Where(x => x.Url == url);
            logic.AddFilteredItem(toFilter);
        }

        private void IgnoreByUserAgent()
        {
            Console.WriteLine("Enter user agent to ignore: ");
            string agent = Console.ReadLine();

            var toFilter = loadedLogs.Where(x => x.UserAgent == agent);
            logic.AddFilteredItem(toFilter);
        }

        private void IgnoreByHost()
        {
            Console.WriteLine("Enter host to ignore: ");
            string host = Console.ReadLine();

            var toFilter = loadedLogs.Where(x => x.Host == host);
            logic.AddFilteredItem(toFilter);
        }


        public void Display()
        {
            foreach (var li in this.loadedLogs)
            {
                Console.WriteLine(li.ClientIpAddr + " - " + li.Time);
                Console.WriteLine($"\t{li.Url}");
                Console.WriteLine($"\t{li.UserAgent}");
                Console.WriteLine($"\t{li.ActionName}");
            }
        }
    }
}
