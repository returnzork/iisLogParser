using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

//using System.Xml;
using System.Xml.Linq;

namespace returnzork.IIS_Log_Parser
{
    public struct FailedReqLogItem : IFailedReqLogItem
    {
        public string Url { get; }
        public string Host { get; }
        public int StatusCode { get; }
        public string UserAgent { get; }
        public FailedAction Action { get; }
        public string ActionName { get; }
        public string RemoteAddress { get; }


        private FailedReqLogItem(string file)
        {
            //load the document and traverse it
            XDocument doc = XDocument.Load(file);
            Url = doc.Root.Attribute("url").Value;

            var z = doc.Root.Elements().ToList()[3].Elements().ToList()[1].Elements().ToList()[1].Value;
            var split = z.Split('\n');

            Host = split[2].Substring(split[2].IndexOf(' ') + 1);
            UserAgent = split[3].Substring(split[3].IndexOf(' ') + 1);

            StatusCode = int.Parse(doc.Root.Attribute("statusCode").Value);

            var c = doc.Root.Elements().ToList().Count();
            var zzz = doc.Root.Elements().ToList()[46].Elements().ToList()[2].Elements().ToList()[0].Value;
            Action = (FailedAction)Enum.Parse(typeof(FailedAction), zzz);
            ActionName = doc.Root.Elements().ToList()[44].Elements().ToList()[1].Elements().ToList()[1].Value;

            RemoteAddress = doc.Root.Elements().ToList()[2].Elements().ToList()[1].Elements().ToList()[1].Value;
        }

        public static IFailedReqLogItem LoadFailedReq(string file)
        {
            return new FailedReqLogItem(file);
        }
    }
}
