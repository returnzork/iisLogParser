using System;
using System.Collections.Generic;
using System.Text;

namespace returnzork.IIS_Log_Parser
{
    public struct LogItem : ILogItem
    {
        /// <summary>
        /// Was this LogItem loaded successfully?
        /// </summary>
        public bool IsValid { get; private set; }


        /// <summary>
        /// Time that this log occurred (UTC Time)
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// IP Address of the server handling this log item
        /// </summary>
        public string ServerIpAddr { get; set; }
        /// <summary>
        /// Verb used to access server
        /// </summary>
        public string HTTPVerb { get; set; }
        /// <summary>
        /// Uri accessed on server
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// ?
        /// </summary>
        public string Query { get; set; }
        /// <summary>
        /// Port access on server
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Username (if set) used to access server
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Client IP address
        /// </summary>
        public string ClientIpAddr { get; set; }
        /// <summary>
        /// User agent sent when connecting to server
        /// </summary>
        public string UserAgent { get; set; }
        /// <summary>
        /// Uri set as Referer
        /// </summary>
        public string Referer { get; set; }
        /// <summary>
        /// HTTP Status code sent to client
        /// </summary>
        public int HTTPStatus { get; set; }
        /// <summary>
        /// HTTP Sub Status code sent to client
        /// </summary>
        public int HTTPSubStatus { get; set; }
        /// <summary>
        /// Windows status code
        /// </summary>
        public string WindowsStatus { get; set; }
        /// <summary>
        /// Time that the server spent processing this item
        /// </summary>
        public TimeSpan TimeTaken { get; set; }

        /// <summary>
        /// Create a new LogItem
        /// </summary>
        /// <param name="args">Array containing each piece of the log file</param>
        private LogItem(string[] args)
        {
            //format is
            //#Fields: date time s-ip cs-method cs-uri-stem cs-uri-query s-port cs-username c-ip cs(User-Agent) cs(Referer) sc-status sc-substatus sc-win32-status time-taken
            //space delimiters

            //[0] + [1] = this.Time
            this.Time = DateTime.Parse(args[0] + " " + args[1]);
            this.ServerIpAddr = args[2];
            this.HTTPVerb = args[3];
            this.Url = args[4];
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

            IsValid = true;
        }

        public static ILogItem Create(string[] args)
        {
            return new LogItem(args);
        }

        public override string ToString()
        {
            string output = "";

            output += Time + " " + ClientIpAddr + " " + HTTPVerb + " " + HTTPStatus + " " + Url;

            return output;
        }
    }
}
