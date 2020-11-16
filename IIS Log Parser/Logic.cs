using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using System.Linq;

namespace returnzork.IIS_Log_Parser
{
    internal class Logic<T> : ILogic where T : ILog
    {
        private List<T> logs;
        private ILogDisplay display;

        internal Logic(IEnumerable<ILog> logs)
        {
            if(logs is IEnumerable<ILogItem> enIli)
            {
                List<ILogItem> ili = enIli.ToList();
                this.logs = ili as List<T>;
                display = new LogDisplay(ili);
            }
            else if(logs is IEnumerable<IFailedReqLogItem> frq)
            {
                this.logs = frq as List<T>;
                display = new FailedRequestDisplay(frq.ToList());
            }
            else
                throw new NotImplementedException();
        }

        public void Run()
        {
            //setup requirements
            MenuEntry entry;
            bool shouldExit = false;


            //run the menu
            do
            {
                display.ShowMenu();
                entry = display.GetMenuItem();
                switch (entry)
                {
                    case MenuEntry.Exit:
                        shouldExit = true;
                        break;

                    case MenuEntry.AddLogFile:
                        AddLogFile();
                        break;
                    case MenuEntry.LoadFolder:
                        LoadLogFolder();
                        break;

                    case MenuEntry.GlobalIgnore:
                        AddGlobalIgnore();
                        break;
                    case MenuEntry.GlobalIgnoreFile:
                        AddGlobalIgnoreFile();
                        break;



                    default:
                        display.ConsumeMenuItem(entry);
                        break;
                }
            }
            while (entry != MenuEntry.Exit && !shouldExit);
        }

        private void AddGlobalIgnoreFile()
        {
            Console.WriteLine("Enter file containing [ip, address] to ignore:");
            string file = Console.ReadLine();
            if (!File.Exists(file))
            {
                Console.WriteLine("File does not exist");
            }
            else
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    //cast the list to an array so we can use a parallel for loop
                    var logArray = logs.ToArray();

                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (string.IsNullOrEmpty(line))
                            continue;

                        if (!Helper.IpSplit(line, out string[] res))
                        {
                            Console.WriteLine("Invalid format");
                        }
                        else
                        {
                            System.Threading.Tasks.Parallel.For(0, logArray.Length, (i) =>
                            {
                                if (typeof(T) == typeof(ILogItem))
                                {
                                    if (logArray[i] != null)
                                    {
                                        if (res.Contains((logArray[i] as ILogItem).ClientIpAddr))
                                            logArray[i] = default;
                                    }
                                }
                                else
                                    throw new NotImplementedException();
                            });
                        }
                    }

                    logs = logArray.Where(x => x != null && x.IsValid).ToList();
                }
            }
        }

        private void AddGlobalIgnore()
        {
            Console.WriteLine("Enter [ip, address] to ignore:");
            string arr = Console.ReadLine();
            if (!Helper.IpSplit(arr, out string[] split))
            {
                Console.WriteLine("Invalid format");
            }
            else
            {
                for (int i = logs.Count - 1; i >= 0; i--)
                {
                    if (typeof(T) == typeof(ILogItem))
                    {
                        if (split.Contains((logs[i] as ILogItem).ClientIpAddr))
                            logs.RemoveAt(i);
                    }
                    else
                        throw new NotImplementedException();
                }
            }
        }

        private void LoadLogFolder()
        {
            Console.WriteLine("Enter folder to load from");
            string dir = Console.ReadLine();
            if (!Directory.Exists(dir))
            {
                Console.WriteLine("Directory does not exist");
            }
            else
            {
                if (typeof(T) == typeof(ILogItem))
                {
                    logs = Program.LoadDirectory(dir) as List<T>;
                }
                else
                    throw new NotImplementedException();
            }
        }

        private void AddLogFile()
        {
            Console.WriteLine("Enter new log file to add");
            string newFile = Console.ReadLine();
            if (!File.Exists(newFile))
            {
                Console.WriteLine("File does not exist");
            }
            else
            {
                if (typeof(T) == typeof(ILogItem))
                {
                    //we shouldn't need to reinitialize the Display, as a change to the List should be reflected
                    (logs as List<ILogItem>).AddRange(Program.ParseLines(Program.ReadFile(newFile)));
                }
                else
                    throw new NotImplementedException();
            }
        }
    }
}
