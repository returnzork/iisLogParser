using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using System.Linq;

namespace returnzork.IIS_Log_Parser
{
    /// <summary>
    /// Class that holds the code that links the individual log items together
    /// </summary>
    /// <typeparam name="T">ILog type of what logs are to be used</typeparam>
    internal class Logic<T> : ILogic where T : ILog
    {
        /// <summary>
        /// All unfiltered logs that have been loaded <see cref="filteredLogs"/>
        /// </summary>
        private List<T> allLogs;

        /// <summary>
        /// Logs that have been filtered from <see cref="allLogs"/>
        /// </summary>
        private IEnumerable<T> filteredLogs;

        /// <summary>
        /// LogDisplay class that is used for displaying the log items
        /// </summary>
        private ILogDisplay display;

        /// <summary>
        /// Event that is fired whenever <see cref="filteredLogs"/> is updated
        /// </summary>
        public event EventHandler<LogsChangedEventArgs> OnLogsChanged;



        /// <param name="logs">All of the logs files that have been loaded</param>
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
                display = new FailedRequestDisplay(this as Logic<IFailedReqLogItem>);
            }
            else
                throw new NotImplementedException();

            this.filteredLogs = this.allLogs;

            LogFilterChanged();
        }

        /// <summary>
        /// Run the Logic execution code
        /// </summary>
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

                    case MenuEntry.DisplayLogs:
                        display.Display();
                        break;


                    default:
                        display.ConsumeMenuItem(entry);
                        break;
                }
            }
            while (entry != MenuEntry.Exit && !shouldExit);
        }

        /// <summary>
        /// From a text document, take the specified ip address' from the filtered logs
        /// </summary>
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

        /// <summary>
        /// From user entry, take the specified ip address (or multiple) from the filtered logs
        /// </summary>
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
                    else if(x is IFailedReqLogItem frqli)
                    {
                        if (split.Contains(frqli.ClientIpAddr))
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

        /// <summary>
        /// Load a new log folder, removing any log files that are already loaded
        /// </summary>
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

        /// <summary>
        /// Add a single log file, adding it to the log files already loaded
        /// </summary>
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


        /// <summary>
        /// Glue code to interact with <see cref="OnLogsChanged"/>
        /// </summary>
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

        /// <summary>
        /// Take the specified items out of the filtered logs
        /// </summary>
        /// <param name="item">All of the logs that should be filtered from <see cref="filteredLogs"/></param>
        internal void AddFilteredItem(IEnumerable<T> item)
        {
            int prevCount = filteredLogs.Count();

            filteredLogs = filteredLogs.Where(x => item.Contains(x) == false);

            if (prevCount != filteredLogs.Count())
                LogFilterChanged();
        }
    }
}
