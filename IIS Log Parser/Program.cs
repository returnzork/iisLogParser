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
        }

        static List<string> ReadFile(string file)
        {
            List<string> lines = new List<string>();
            using (StreamReader sr = new StreamReader(file))
            {
                while(!sr.EndOfStream)
                {
                    lines.Add(sr.ReadLine());
                }
            }

            return lines;
        }
    }
}