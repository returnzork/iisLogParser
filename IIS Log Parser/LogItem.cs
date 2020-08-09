using System;
using System.Collections.Generic;
using System.Text;

namespace returnzork.IIS_Log_Parser
{
    internal struct LogItem
    {
        internal DateTime Time { get; set; }
        internal string ServerIpAddr { get; set; }
        internal string HTTPVerb { get; set; }
        internal string Uri { get; set; }
        internal string Query { get; set; }
        internal int Port { get; set; }
        internal string Username { get; set; }
        internal string ClientIpAddr { get; set; }
        internal string UserAgent { get; set; }
        internal string Referer { get; set; }
        internal int HTTPStatus { get; set; }
        internal int HTTPSubStatus { get; set; }
        internal string WindowsStatus { get; set; }
        internal TimeSpan TimeTaken { get; set; }

        public LogItem(string[] args)
        {
            //format is
            //#Fields: date time s-ip cs-method cs-uri-stem cs-uri-query s-port cs-username c-ip cs(User-Agent) cs(Referer) sc-status sc-substatus sc-win32-status time-taken
            //space delimiters

            //[0] + [1] = this.Time
            this.Time = DateTime.Parse(args[0] + " " + args[1]);
            this.ServerIpAddr = args[2];
            this.HTTPVerb = args[3];
            this.Uri = args[4];
            this.Query = args[5];
            this.Port = int.Parse(args[6]);
            this.Username = args[7];
            this.ClientIpAddr = args[8];
            this.UserAgent = args[9];
            this.Referer = args[10];
            this.HTTPStatus = int.Parse(args[11]);
            this.HTTPSubStatus = int.Parse(args[12]);
            this.WindowsStatus = args[13];
            this.TimeTaken = new TimeSpan(0, 0, 0, 0, int.Parse(args[14]));
        }

        public override string ToString()
        {
            string output = "";

            output += Time + " " + HTTPVerb + " " + HTTPStatus + " " + Uri;

            return output;
        }
    }
}
