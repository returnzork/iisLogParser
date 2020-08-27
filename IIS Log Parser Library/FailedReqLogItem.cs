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
            StatusCode = int.Parse(doc.Root.Attribute("statusCode").Value);


            //Get the host and user agent from the Headers attribute
            var split = doc.Root.Descendants().First(x => x.HasAttributes && x.Attribute("Name")?.Value == "Headers").Value.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            this.Host = split[0].Substring(split[0].IndexOf(' ') + 1);
            if (split.Length != 1)
                UserAgent = split[1].Substring(split[1].IndexOf(' ') + 1);
            else
                UserAgent = "No User Agent Specified In Log File";

            //get the failed action
            //It is stored in an element named Opcode. Some Opcode data is numerical (which can be parsed incorrectly to the FailedAction enum), so we must ignore those values. Also ignore the Rule Evaluation End because that is not what we are looking for
            var failActionNode = doc.Root.Descendants().First(x => x.Name.LocalName == "Opcode" && Enum.IsDefined(typeof(FailedAction), x.Value) && Enum.TryParse(typeof(FailedAction), x.Value, out object result) && (FailedAction)result != FailedAction.RULE_EVALUATION_END);
            Action = (FailedAction)Enum.Parse(typeof(FailedAction), failActionNode.Value);

            //get the name of the failed action from the last RuleName attribute
            var t1 = doc.Root.Elements().ToList()[44].Elements().ToList()[1].Elements().ToList()[1];
            var t2 = doc.Root.Descendants().Last(x => x.HasAttributes && x.FirstAttribute.Value == "RuleName");
            ActionName = t2.Value;

            //get the remote address from the RemoteAddress attribute
            RemoteAddress = doc.Root.Descendants().First(x => x.HasAttributes && x.FirstAttribute.Value == "RemoteAddress").Value;
        }

        public static IFailedReqLogItem LoadFailedReq(string file)
        {
            return new FailedReqLogItem(file);
        }
    }
}
