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
                Console.WriteLine("0 - Load a log file to what is already loaded");
                Console.WriteLine("5 - Load folder of log files, removing what is already loaded");
                Console.WriteLine("1 - Show items by client ip address");
                Console.WriteLine("10 - Show items by multiple client ip address");
                Console.WriteLine("100 - Show items not matching client ip address");
                Console.WriteLine("1000 - Show items not maching multiple client ip address");
                Console.WriteLine("2 - Show items by HTTP verb");
                Console.WriteLine("3 - Match by status code");
                Console.WriteLine("4 - Add global ignore");
                Console.WriteLine("-1 - Exit Program");

                if(!int.TryParse(Console.ReadLine(), out int result) || result < -1 || (result > 5 && result != 10 && result != 100 && result != 1000))
                {
                    Console.WriteLine("Invalid entry");
                    continue;
                }

                switch(result)
                {
                    case -1:
                        shouldExit = true;
                        break;

                    case 0:
                        Console.WriteLine("Enter new log file to add");
                        string newFile = Console.ReadLine();
                        if(!File.Exists(newFile))
                        {
                            Console.WriteLine("File does not exist");
                        }
                        else
                        {
                            logs.AddRange(ParseLines(ReadFile(newFile)));
                            display = new LogDisplay(logs);
                        }
                        break;
                    case 5:
                        Console.WriteLine("Enter folder to load from");
                        string dir = Console.ReadLine();
                        if(!Directory.Exists(dir))
                        {
                            Console.WriteLine("Directory does not exist");
                        }
                        else
                        {
                            logs = new List<LogItem>();
                            string[] allLogFiles = Directory.GetFiles(dir);
                            List<LogItem>[] temp = new List<LogItem>[allLogFiles.Length];
                            System.Threading.Tasks.Parallel.For(0, allLogFiles.Length, (i) =>
                            {
                                temp[i] = ParseLines(ReadFile(allLogFiles[i]));
                            });

                            foreach(var li in temp)
                            {
                                logs.Concat(li);
                            }

                            display = new LogDisplay(logs);
                        }
                        break;

                    case 4:
                        //global ignore
                        Console.WriteLine("Enter [ip, address] to ignore (MINIMUM 2 ENTRIES):");
                        string arr = Console.ReadLine();
                        if(!Helper.IpSplit(arr, out string[] split))
                        {
                            Console.WriteLine("Invalid format");
                        }
                        else
                        {
                            for(int i = logs.Count - 1; i >= 0; i--)
                            {
                                if (split.Contains(logs[i].ClientIpAddr))
                                    logs.RemoveAt(i);
                            }
                        }
                        break;

                    case 1:
                        display.ShowByClientIp();
                        break;
                    case 10:
                        display.ShowByMultipleClientIp();
                        break;
                    case 100:
                        display.ShowByNotClientIp();
                        break;
                    case 1000:
                        display.ShowByMultipleNotClientIp();
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