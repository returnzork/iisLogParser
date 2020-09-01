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
            Console.WriteLine("Enter a IIS Log file/directory to parse");
            string file = Console.ReadLine();
            file = file.Replace("\"", "");

            if(!(File.Exists(file) || Directory.Exists(file)))
            {
                Console.WriteLine("No log was found");
            }
            else
            {
                FileWork(file);
            }
            Console.ReadLine();
        }

        static void FileWork(string file)
        {
            List<ILogItem> parsed;
            if(File.Exists(file))
            {
                parsed = ParseLines(ReadFile(file));
            }
            else
            {
                parsed = LoadDirectory(file);
            }

            Console.WriteLine($"There were a total of {parsed.Count} log entries");
            Logic l = new Logic(parsed);
            l.Run();
        }


        internal static List<ILogItem> LoadDirectory(string dir)
        {
            List<ILogItem> logs = new List<ILogItem>();
            string[] allLogFiles = Directory.GetFiles(dir);
            List<ILogItem>[] temp = new List<ILogItem>[allLogFiles.Length];
            System.Threading.Tasks.Parallel.For(0, allLogFiles.Length, (i) =>
            {
                temp[i] = ParseLines(ReadFile(allLogFiles[i]));
            });

            foreach (var li in temp)
            {
                logs.AddRange(li);
            }

            return logs;
        }

        internal static List<ILogItem> ParseLines(List<string> lines)
        {
            //format is
            //#Fields: date time s-ip cs-method cs-uri-stem cs-uri-query s-port cs-username c-ip cs(User-Agent) cs(Referer) sc-status sc-substatus sc-win32-status time-taken
            //space delimiters

            List<ILogItem> result = new List<ILogItem>(lines.Count);
            foreach(string item in lines)
            {
                string[] split = item.Split(' ');
                result.Add(LogItem.Create(split));
            }

            return result;
        }

        internal static List<string> ReadFile(string file)
        {
            List<string> lines = new List<string>();
            using (FileStream openedFile = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(openedFile))
                {
                    while (!sr.EndOfStream)
                    {
                        string l = sr.ReadLine();
                        if (l.StartsWith('#'))
                            continue;
                        lines.Add(l);
                    }
                }
            }

            return lines;
        }
    }
}