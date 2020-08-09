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
            var clientIp = logs.Where(x => x.ClientIpAddr == ip);
            if (clientIp.Any())
            {
                foreach (var x in clientIp)
                {
                    Console.WriteLine(x);
                }
            }
            else
            {
                Console.WriteLine("No matching ip address");
            }
        }

        internal void ShowByHTTPVerb()
        {
            Console.WriteLine("Enter HTTP Verb to match:");
            string verb = Console.ReadLine().ToLower();
            var matches = logs.Where(x => x.HTTPVerb.ToLower() == verb);
            if(matches.Any())
            {
                foreach(var x in matches)
                {
                    Console.WriteLine(x);
                }
            }
            else
            {
                Console.WriteLine("No matching verbs");
            }
        }

        internal void ShowByStatusCode()
        {
            Console.WriteLine("Enter status code:");
            if(!int.TryParse(Console.ReadLine(), out int code))
            {
                Console.WriteLine("Invalid code");
                return;
            }
            var matches = logs.Where(x => x.HTTPStatus == code);
            if(matches.Any())
            {
                foreach(var x in matches)
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
