using System;
using System.Collections.Generic;
using System.Text;

namespace returnzork.IIS_Log_Parser
{
    internal enum MenuEntry { NONE, Exit, AddLogFile, LoadFolder, GlobalIgnore, GlobalIgnoreFile, ShowClientIp,
        ShowNotClientIp,
        ShowHTTPVerb,
        ShowStatusCode
    }
    internal static class Menu
    {
        internal static void DisplayMenu()
        {
            Console.WriteLine("0 - Load a log file to what is already loaded");
            Console.WriteLine("5 - Load folder of log files, removing what is already loaded");
            Console.WriteLine("1 - Show items by client ip address");
            Console.WriteLine("10 - Show items not matching client ip address");
            Console.WriteLine("2 - Show items by HTTP verb");
            Console.WriteLine("3 - Match by status code");
            Console.WriteLine("4 - Add global ignore");
            Console.WriteLine("44 - Load global ignore file");
            Console.WriteLine("-1 - Exit Program");
        }

        internal static MenuEntry GetMenuEntry()
        {
            if (!int.TryParse(Console.ReadLine(), out int result) || result < -1 || (result > 5 && result != 10 && result != 100 && result != 1000 && result != 44))
            {
                Console.WriteLine("Invalid entry");
                return MenuEntry.NONE;
            }

            switch (result)
            {
                case -1:
                    return MenuEntry.Exit;


                case 0:
                    return MenuEntry.AddLogFile;

                case 5:
                    return MenuEntry.LoadFolder;


                case 4:
                    return MenuEntry.GlobalIgnore;

                case 44:
                    return MenuEntry.GlobalIgnoreFile;


                case 1:
                    return MenuEntry.ShowClientIp;
                case 10:
                    return MenuEntry.ShowNotClientIp;


                case 2:
                    return MenuEntry.ShowHTTPVerb;
                case 3:
                    return MenuEntry.ShowStatusCode;

                default:
                    return MenuEntry.NONE;
            }
        }
    }
}
