using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

using System.Linq;

namespace returnzork.IIS_Log_Parser
{
    enum FailedRequestMenuItem { NONE, Exit, IgnoreByUrl, IgnoreByUserAgent, Display }
    internal class FailedRequestDisplay
    {
        IFailedReqLogItem[] logs;
        internal FailedRequestDisplay(string dir)
        {
            //load all of the xml files
            var files = Directory.GetFiles(dir, "*.xml");
            logs = new IFailedReqLogItem[files.Length];
            for(int i = 0; i < files.Length; i++)
            {
                logs[i] = FailedReqLogItem.LoadFailedReq(files[i]);
            }
        }

        internal void Display()
        {
            while (true)
            {
                var option = DisplayMenu();
                switch (option)
                {
                    case FailedRequestMenuItem.Exit:
                        return;

                    case FailedRequestMenuItem.Display:
                        DisplayLogs();
                        break;

                    case FailedRequestMenuItem.IgnoreByUrl:
                        IgnoreByUrl();
                        break;
                    case FailedRequestMenuItem.IgnoreByUserAgent:
                        IgnoreByUserAgent();
                        break;
                }
            }
        }

        private void DisplayLogs()
        {
            foreach(var li in logs)
            {
                Console.WriteLine(li.RemoteAddress + " - " + li.Time);
                Console.WriteLine($"\t{li.Url}");
                Console.WriteLine($"\t{li.UserAgent}");
                Console.WriteLine($"\t{li.ActionName}");
            }
        }


        private void IgnoreByUrl()
        {
            Console.WriteLine("Enter url to ignore: ");
            string url = Console.ReadLine();

            logs = logs.Where(x => x.Url != url).ToArray();
        }

        private void IgnoreByUserAgent()
        {
            Console.WriteLine("Enter user agent to ignore: ");
            string agent = Console.ReadLine();

            logs = logs.Where(x => x.UserAgent != agent).ToArray();
        }


        private FailedRequestMenuItem DisplayMenu()
        {
            Console.WriteLine("-1 - Exit");
            Console.WriteLine("0 - Display");
            Console.WriteLine("4 - Add ignore by url");
            Console.WriteLine("44 - Add ignore by user agent");

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

                case 4:
                    return FailedRequestMenuItem.IgnoreByUrl;
                case 44:
                    return FailedRequestMenuItem.IgnoreByUserAgent;

                default:
                    return FailedRequestMenuItem.NONE;
            }
        }
    }
}
