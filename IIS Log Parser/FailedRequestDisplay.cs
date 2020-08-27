using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace returnzork.IIS_Log_Parser
{
    internal class FailedRequestDisplay
    {
        IFailedReqLogItem[] logs;
        internal FailedRequestDisplay(string dir)
        {
            //load all of the xml files
            var files = Directory.GetFiles(dir, "*.xml");
            logs = new IFailedReqLogItem[files.Length];
            for(int i = 0; i < files.Length; i++)
            {
                logs[i] = FailedReqLogItem.LoadFailedReq(files[i]);
            }
        }

        internal void Display()
        { }
    }
}
