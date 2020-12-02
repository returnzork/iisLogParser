using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

using System.Linq;

namespace returnzork.IIS_Log_Parser
{
    internal class FailedRequestDisplay : ILogDisplay
    {
        List<IFailedReqLogItem> loadedLogs;
        [Obsolete]
        IEnumerable<IFailedReqLogItem> currentLogFilter;


        public static IEnumerable<IFailedReqLogItem> LoadFailedReqLogDir(string dir)
        {
            //load all of the xml files
            var files = Directory.GetFiles(dir, "*.xml");
            for (int i = 0; i < files.Length; i++)
            {
                //make sure the file exists, the file could have been deleted since the last iteration happened
                if (File.Exists(files[i]))
                    yield return FailedReqLogItem.LoadFailedReq(files[i]);
            }
        }


        public FailedRequestDisplay(ILogic logic)
        {
            logic.OnLogsChanged += Logic_OnLogsChanged;
        }

        private void Logic_OnLogsChanged(object sender, LogsChangedEventArgs e)
        {
            if (e.NewLogs is IFailedReqLogItem[] ifrqliA)
            {
                this.loadedLogs = ifrqliA.ToList();
                //TODO removeme when finished
                currentLogFilter = loadedLogs.AsEnumerable();
            }
        }

        public void ConsumeMenuItem(MenuEntry item)
        {
            switch (item)
            {
                case MenuEntry.Exit:
                    return;

                case MenuEntry.FRQDisplay:
                    DisplayLogs(currentLogFilter);
                    break;

                case MenuEntry.FRQIgnoreByUrl:
                    IgnoreByUrl(ref currentLogFilter);
                    break;
                case MenuEntry.FRQIgnoreByUserAgent:
                    IgnoreByUserAgent(ref currentLogFilter);
                    break;

                case MenuEntry.FRQIgnoreByHost:
                    IgnoreByHost(ref currentLogFilter);
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
                    return MenuEntry.FRQDisplay;

                case 1:
                    return MenuEntry.FRQResetLogFilter;

                case 4:
                    return MenuEntry.FRQIgnoreByUrl;
                case 44:
                    return MenuEntry.FRQIgnoreByUserAgent;
                case 444:
                    return MenuEntry.FRQIgnoreByIP;
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




        public void Display() => DisplayLogs(currentLogFilter);
        private void DisplayLogs(IEnumerable<IFailedReqLogItem> logs)
        {
            foreach (var li in logs)
            {
                Console.WriteLine(li.ClientIpAddr + " - " + li.Time);
                Console.WriteLine($"\t{li.Url}");
                Console.WriteLine($"\t{li.UserAgent}");
                Console.WriteLine($"\t{li.ActionName}");
            }
        }
    }
}
