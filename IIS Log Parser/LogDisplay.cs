using returnzork.IIS_Log_Parser;
using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace returnzork.IIS_Log_Parser
{
    internal class LogDisplay
    {
        List<ILogItem> logs;
        const string DEFAULTFORMAT = "%date% %ip% %method% %status% %path%";
        string currentFormat = DEFAULTFORMAT;


        internal LogDisplay(List<ILogItem> logs)
        {
            this.logs = logs;
        }

        internal void ShowByClientIp()
        {
            Console.WriteLine("Enter ip address to match:");
            string ip = Console.ReadLine();
            if (!ip.Contains(',') && ip[0] != '[')
                Display(LogModifier.GetByClientIp(logs, ip));
            else
                Display(LogModifier.GetByMultipleClientIp(logs, ip));
        }

        internal void ShowByNotClientIp()
        {
            Console.WriteLine("Enter IP to negate its lookup:");
            string ip = Console.ReadLine();
            if (!ip.Contains(',') && ip[0] != '[')
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
            //%date% %ip% %method% %status% %path%

            string output = currentFormat;

            output = output.Replace("%date%", item.Time.ToString());
            output = output.Replace("%ip%", item.ClientIpAddr);
            output = output.Replace("%method%", item.HTTPVerb);
            output = output.Replace("%status%", item.HTTPStatus.ToString());
            output = output.Replace("%path%", item.Uri);

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
