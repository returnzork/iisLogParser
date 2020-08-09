using System;
using System.Collections.Generic;
using System.IO;

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
            ParseLines(lines);
        }

        static void ParseLines(List<string> lines)
        {
            //format is
            //#Fields: date time s-ip cs-method cs-uri-stem cs-uri-query s-port cs-username c-ip cs(User-Agent) cs(Referer) sc-status sc-substatus sc-win32-status time-taken
            //space delimiters

            foreach(string item in lines)
            {
                string[] split = item.Split(' ');
                LogItem li = new LogItem(split);
            }
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