using System;
using System.Collections.Generic;
using System.IO;

using System.Linq;

namespace returnzork.IIS_Log_Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a IIS Log file to parse");
            string file = Console.ReadLine();

            if(!File.Exists(file))
            {
                Console.WriteLine("File does not exist");
            }
            else
            {
                FileWork(file);
            }
        }

        static void FileWork(string file)
        {
            var lines = ReadFile(file);
            Console.WriteLine($"There were a total of {lines.Count} log entries");

            var parsed = ParseLines(lines);
            Prompt(parsed);
        }

        static void Prompt(List<LogItem> logs)
        {
            bool shouldExit = false;
            LogDisplay display = new LogDisplay(logs);
            do
            {
                Console.WriteLine("1 - Show items by client ip address");
                Console.WriteLine("2 - Show items by HTTP verb");
                Console.WriteLine("3 - Match by status code");
                Console.WriteLine("-1 - Exit Program");

                if(!int.TryParse(Console.ReadLine(), out int result) || result < -1 || result == 0 || result > 3)
                {
                    Console.WriteLine("Invalid entry");
                    continue;
                }

                switch(result)
                {
                    case -1:
                        shouldExit = true;
                        break;

                    case 1:
                        display.ShowByClientIp();
                        break;
                    case 2:
                        display.ShowByHTTPVerb();
                        break;
                    case 3:
                        display.ShowByStatusCode();
                        break;
                }
            }
            while (!shouldExit);
        }

        static List<LogItem> ParseLines(List<string> lines)
        {
            //format is
            //#Fields: date time s-ip cs-method cs-uri-stem cs-uri-query s-port cs-username c-ip cs(User-Agent) cs(Referer) sc-status sc-substatus sc-win32-status time-taken
            //space delimiters

            List<LogItem> result = new List<LogItem>(lines.Count);
            foreach(string item in lines)
            {
                string[] split = item.Split(' ');
                result.Add(new LogItem(split));
            }

            return result;
        }

        static List<string> ReadFile(string file)
        {
            List<string> lines = new List<string>();
            using (StreamReader sr = new StreamReader(file))
            {
                while(!sr.EndOfStream)
                {
                    string l = sr.ReadLine();
                    if (l.StartsWith('#'))
                        continue;
                    lines.Add(l);
                }
            }

            return lines;
        }
    }
}