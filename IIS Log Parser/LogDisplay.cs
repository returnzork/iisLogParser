using returnzork.IIS_Log_Parser;
using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace returnzork.IIS_Log_Parser
{
    internal class LogDisplay
    {
        List<LogItem> logs;


        internal LogDisplay(List<LogItem> logs)
        {
            this.logs = logs;
        }

        internal void ShowByClientIp()
        {
            Console.WriteLine("Enter ip address to match:");
            string ip = Console.ReadLine();
            Display(logs.Where(x => x.ClientIpAddr == ip));
        }

        internal void ShowByHTTPVerb()
        {
            Console.WriteLine("Enter HTTP Verb to match:");
            string verb = Console.ReadLine().ToLower();
            Display(logs.Where(x => x.HTTPVerb.ToLower() == verb));
        }

        internal void ShowByStatusCode()
        {
            Console.WriteLine("Enter status code:");
            if(!int.TryParse(Console.ReadLine(), out int code))
            {
                Console.WriteLine("Invalid code");
                return;
            }
            Display(logs.Where(x => x.HTTPStatus == code));
        }


        private void Display(IEnumerable<LogItem> results)
        {
            if (results.Any())
            {
                foreach (var x in results)
                {
                    Console.WriteLine(x);
                }
            }
            else
            {
                Console.WriteLine("No matches");
            }
        }
    }
}
