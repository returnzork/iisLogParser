using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

using System.Linq;

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
        ShowStatusCode,
        [Display(Name = "Change display formatting", Order = 7)]
        ChangeDisplayFormat,
        [Display(Name = "Show items matching a path", Order = 8)]
        ShowByPath,
        [Display(Name = "Reset Log Filters", Order = 9)]
        ResetLogFilters,
        [Display(Name = "Display Logs", Order = 6)]
        DisplayLogs,

        //failed req
        FRQIgnoreByUrl, FRQIgnoreByUserAgent, FRQIgnoreByIP, FRQIgnoreByHost

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
            //check the user put a number in
            if(!int.TryParse(Console.ReadLine(), out int result))
            {
                Console.WriteLine("Invalid entry");
                return MenuEntry.NONE;
            }
            
            //check if the result that the user entered is an option labeled in the Order field of the MenuEntry enum
            MenuEntry item = ((MenuEntry[])Enum.GetValues(typeof(MenuEntry))).First(x => x.GetOrder() == result);

            //if the user entered the default option (display CANCEL/enum name NONE), that's not a valid option
            if(item == default)
            {
                Console.WriteLine("Invalid Entry");
                return MenuEntry.NONE;
            }

            //the user entered a valid menu option, return what it was
            return item;            
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
