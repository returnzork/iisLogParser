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


        internal LogDisplay(List<ILogItem> logs)
        {
            this.logs = logs;
        }

        internal void ShowByClientIp()
        {
            Console.WriteLine("Enter ip address to match:");
            string ip = Console.ReadLine();
            Display(LogModifier.GetByClientIp(logs, ip));
        }

        internal void ShowByNotClientIp()
        {
            Console.WriteLine("Enter IP to negate its lookup:");
            string ip = Console.ReadLine();
            Display(LogModifier.GetByNotClientIp(logs, ip));
        }

        internal void ShowByMultipleClientIp()
        {
            Console.WriteLine("Enter an [ip, array] of ip address to match: (ex [127.0.0.1, 127.0.0.2])");
            string arr = Console.ReadLine();
            Display(LogModifier.GetByMultipleClientIp(logs, arr));
        }

        internal void ShowByMultipleNotClientIp()
        {
            Console.WriteLine("Enter an [ip, array] of ip address to negative match: (ex [127.0.0.1, 127.0.0.2])");
            string arr = Console.ReadLine();
            Display(LogModifier.GetByMultipleNotClientIp(logs, arr));
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
                    Console.WriteLine(x);
                }
            }
            else
            {
                Console.WriteLine("No matches");
            }

            Console.WriteLine();
        }
    }
}
