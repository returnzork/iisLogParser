using returnzork.IIS_Log_Parser;
using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace returnzork.IIS_Log_Parser
{
    internal class LogDisplay : ILogDisplay
    {
        List<ILogItem> logs;
        const string DEFAULTFORMAT = "%date% %ip% %method% %status% %path%";
        string currentFormat = DEFAULTFORMAT;

        internal LogDisplay(List<ILogItem> logs)
        {
            this.logs = logs;
        }



        public void ShowMenu()
        {
            Menu.DisplayMenu();
        }
        
        public MenuEntry GetMenuItem()
        {
            return Menu.GetMenuEntry();
        }

        public void ConsumeMenuItem(MenuEntry item)
        {
            switch (item)
            {
                case MenuEntry.ShowClientIp:
                    ShowByClientIp();
                    break;
                case MenuEntry.ShowNotClientIp:
                    ShowByNotClientIp();
                    break;


                case MenuEntry.ShowHTTPVerb:
                    ShowByHTTPVerb();
                    break;
                case MenuEntry.ShowStatusCode:
                    ShowByStatusCode();
                    break;

                case MenuEntry.ChangeDisplayFormat:
                    ChangeFormat();
                    break;

                case MenuEntry.ShowByPath:
                    ShowByPath();
                    break;
            }
        }





        internal void ShowByClientIp()
        {
            Console.WriteLine("Enter ip address to match:");
            string ip = Console.ReadLine();
            if (string.IsNullOrEmpty(ip))
                Console.WriteLine("Invalid Format");
            else if (!ip.Contains(',') && ip[0] != '[')
                Display(LogModifier.GetByClientIp(logs, ip));
            else
                Display(LogModifier.GetByMultipleClientIp(logs, ip));
        }

        internal void ShowByNotClientIp()
        {
            Console.WriteLine("Enter IP to negate its lookup:");
            string ip = Console.ReadLine();
            if (string.IsNullOrEmpty(ip))
                Console.WriteLine("Invalid Format");
            else if (!ip.Contains(',') && ip[0] != '[')
                Display(LogModifier.GetByNotClientIp(logs, ip));
            else
                Display(LogModifier.GetByMultipleNotClientIp(logs, ip));
        }

        internal void ShowByHTTPVerb()
        {
            Console.WriteLine("Enter HTTP Verb to match:");
            string verb = Console.ReadLine().ToLower();
            Display(LogModifier.GetByHTTPVerb(logs, verb));
        }

        internal void ShowByStatusCode()
        {
            Console.WriteLine("Enter status code:");
            if(!int.TryParse(Console.ReadLine(), out int code))
            {
                Console.WriteLine("Invalid code");
                return;
            }
            Display(LogModifier.GetByStatusCode(logs, code));
        }

        internal void ShowByPath()
        {
            Console.WriteLine("Enter path to match: ");
            string path = Console.ReadLine();
            Display(LogModifier.GetByPath(logs, path));
        }


        private void Display(IEnumerable<ILogItem> results)
        {
            Console.WriteLine();

            if (results.Any())
            {
                foreach (var x in results)
                {
                    Console.WriteLine(GetFormatting(x));
                }
            }
            else
            {
                Console.WriteLine("No matches");
            }

            Console.WriteLine();
        }

        
        private string GetFormatting(ILogItem item)
        {
            //current replacement is
            //%date% %ip% %method% %status% %path% %serverip% %query% %port% %username% %useragent% %referer% %substatus% %winstatus% %taken%

            string output = currentFormat;

            output = output.Replace("%date%", item.Time.ToString());
            output = output.Replace("%serverip%", item.ServerIpAddr);
            output = output.Replace("%method%", item.HTTPVerb);
            output = output.Replace("%path%", item.Uri);
            output = output.Replace("%query%", item.Query);
            output = output.Replace("%port%", item.Port.ToString());
            output = output.Replace("%username%", item.Username);
            output = output.Replace("%ip%", item.ClientIpAddr);
            output = output.Replace("%useragent%", item.UserAgent);
            output = output.Replace("%referer%", item.Referer);
            output = output.Replace("%status%", item.HTTPStatus.ToString());
            output = output.Replace("%substatus%", item.HTTPSubStatus.ToString());
            output = output.Replace("%winstatus%", item.WindowsStatus);
            output = output.Replace("%taken%", item.TimeTaken.ToString());

            return output;
        }



        internal void ChangeFormat()
        {
            Console.WriteLine("Current Format: ");
            Console.WriteLine(currentFormat);
            Console.WriteLine("Enter a new format: ");

            string newFormat = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(newFormat))
                Console.WriteLine("Bad format");
            else
                currentFormat = newFormat;
        }
    }
}
