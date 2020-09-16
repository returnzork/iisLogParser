using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace returnzork.IIS_Log_Parser
{
    internal enum MenuEntry
    {
        [Display(Name = "CANCEL", Order = -5)]
        NONE,
        [Display(Name = "Exit the program", Order = -1)]
        Exit,
        [Display(Name = "Load a log file to what is already loaded", Order = 0)]
        AddLogFile,
        [Display(Name = "Load a new log folder, replacing what is already loaded", Order = 5)]
        LoadFolder,
        [Display(Name = "Add an ip to the global ignore", Order = 4)]
        GlobalIgnore,
        [Display(Name = "Add an ip text document to the global ignore", Order = 44)]
        GlobalIgnoreFile,
        [Display(Name = "Show items that match a client ip address", Order = 1)]
        ShowClientIp,
        [Display(Name = "Show items that do not match a client ip address", Order = 10)]
        ShowNotClientIp,
        [Display(Name = "Show items that match an HTTP verb", Order = 2)]
        ShowHTTPVerb,
        [Display(Name = "Show items that match an HTTP status code", Order = 3)]
        ShowStatusCode
    }

    internal static class Menu
    {
        internal static void DisplayMenu()
        {
            foreach(MenuEntry e in Enum.GetValues(typeof(MenuEntry)))
            {
                Console.WriteLine(e.GetOrder() + " - "  + e.GetDisplayName());
            }
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


        public static string GetDisplayName(this MenuEntry me)
        {
            var attribute = typeof(MenuEntry).GetField(me.ToString()).GetCustomAttribute<DisplayAttribute>();
            if (attribute == null)
                return me.ToString();
            else
                return attribute.Name;
        }

        public static int GetOrder(this MenuEntry me)
        {
            var attribute = typeof(MenuEntry).GetField(me.ToString()).GetCustomAttribute<DisplayAttribute>();
            if (attribute == null)
                return Int32.MinValue;
            else
                return attribute.Order;
        }
    }
}
