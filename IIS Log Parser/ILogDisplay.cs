using System;
using System.Collections.Generic;
using System.Text;

namespace returnzork.IIS_Log_Parser
{
    interface ILogDisplay
    {
        void ShowMenu();
        MenuEntry GetMenuItem();
        void ConsumeMenuItem(MenuEntry item);
    }
}
