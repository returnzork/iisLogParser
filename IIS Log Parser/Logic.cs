using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using System.Linq;

namespace returnzork.IIS_Log_Parser
{
    internal class Logic<T> : ILogic where T : ILog
    {
        private List<T> allLogs;
        private IEnumerable<T> filteredLogs;
        private ILogDisplay display;
        public event EventHandler<LogsChangedEventArgs> OnLogsChanged;

        internal Logic(in IEnumerable<ILog> logs)
        {
            if(logs is IEnumerable<ILogItem> ili)
            {
                this.allLogs = ili.ToList() as List<T>;
                display = new LogDisplay(this);
            }
            else if(logs is IEnumerable<IFailedReqLogItem> frq)
            {
                this.allLogs = frq.ToList() as List<T>;
                display = new FailedRequestDisplay(this);
            }
            else
                throw new NotImplementedException();

            this.filteredLogs = this.allLogs;

            LogFilterChanged();
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

                    case MenuEntry.ResetLogFilters:
                        this.filteredLogs = this.allLogs.AsEnumerable();
                        LogFilterChanged();
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
                            int preCount = filteredLogs.Count();
                            //iterate over each item in the filtered logs, checking if the ClientIpAddr equals an item in the text document
                            filteredLogs = filteredLogs.Where(x =>
                            {
                                if (x is ILogItem ili)
                                {
                                    if (ili.IsValid)
                                    {
                                        if (res.Contains(ili.ClientIpAddr))
                                            return false;
                                        else
                                            return true;
                                    }
                                    else
                                        return false;
                                }
                                else
                                    throw new NotImplementedException();
                            });

                            if(preCount != filteredLogs.Count())
                                LogFilterChanged();
                        }
                    }
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
                int prevCount = filteredLogs.Count();
                //get all of the logs where the ClientIpAddr is not contained in the 'split' ip addresses to remove
                filteredLogs = filteredLogs.Where(x =>
                {
                    if (x is ILogItem ili)
                    {
                        if (split.Contains(ili.ClientIpAddr))
                            return false;
                    }
                    else
                        throw new NotImplementedException();

                    return true;
                });

                if (prevCount != filteredLogs.Count())
                    LogFilterChanged();
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
                    allLogs = Program.LoadDirectory(dir) as List<T>;
                }
                else
                    throw new NotImplementedException();

                LogFilterChanged();
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
                    (allLogs as List<ILogItem>).AddRange(Program.ParseLines(Program.ReadFile(newFile)));
                }
                else
                    throw new NotImplementedException();

                LogFilterChanged();
            }
        }


        private void LogFilterChanged()
        {
            Type t = filteredLogs.GetType();
            if (filteredLogs is IEnumerable<ILogItem> lili)
                OnLogsChanged(this, new LogsChangedEventArgs(lili.ToArray()));
            else if (filteredLogs is IEnumerable<IFailedReqLogItem> lifrqli)
                OnLogsChanged(this, new LogsChangedEventArgs(lifrqli.ToArray()));
            else
                throw new ArgumentException();
        }
    }
}
