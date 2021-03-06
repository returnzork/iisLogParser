﻿using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

//using System.Xml;
using System.Xml.Linq;

namespace returnzork.IIS_Log_Parser
{
    public struct FailedReqLogItem : IFailedReqLogItem
    {
        public string FileName { get; }

        public string Url { get; }
        public string Host { get; }
        public int HTTPStatus { get; }
        public int HTTPSubStatus { get; }
        public string UserAgent { get; }
        public FailedAction Action { get; }
        public string ActionName { get; }
        public string ClientIpAddr { get; }

        public DateTime Time { get; }
        public bool IsValid { get; }


        private FailedReqLogItem(string file)
        {
            FileName = file;
            //load the document and traverse it
            XDocument doc = XDocument.Load(file);
            Url = doc.Root.Attribute("url").Value;
            {
                var statusCodeNode = doc.Root.Attribute("statusCode").Value;
                var statusCodeSplit = statusCodeNode.Split('.');
                HTTPStatus = int.Parse(statusCodeSplit[0]);
                if (statusCodeSplit.Length == 2)
                    HTTPSubStatus = int.Parse(statusCodeSplit[1]);
                else
                    HTTPSubStatus = 0;
            }


            //Get the host and user agent from the Headers attribute
            var split = doc.Root.Descendants().First(x => x.HasAttributes && x.Attribute("Name")?.Value == "Headers").Value.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (split.Length == 0)
            {
                Host = "No Host Specified In Log File";
                UserAgent = "No User Agent Specified In Log File";
            }
            else
            {
                var s1 = split.FirstOrDefault(x => x.StartsWith("Host: "));
                Host = s1?.Substring(s1.IndexOf(' ') + 1) ?? "No Host Specified In Log File";
                var s2 = split.FirstOrDefault(x => x.StartsWith("User-Agent: "));
                UserAgent = s2?.Substring(s2.IndexOf(' ') + 1) ?? "No User Agent Specified In Log File";
            }

            //get the failed action
            //It is stored in an element named Opcode. Some Opcode data is numerical (which can be parsed incorrectly to the FailedAction enum), so we must ignore those values. Also ignore the Rule Evaluation End because that is not what we are looking for
            var failActionNode = doc.Root.Descendants().FirstOrDefault(x => x.Name.LocalName == "Opcode" && Enum.IsDefined(typeof(FailedAction), x.Value) && Enum.TryParse(x.Value, out FailedAction result) && result != FailedAction.RULE_EVALUATION_END);
            Action = Enum.Parse<FailedAction>(failActionNode?.Value ?? "NONE");

            //get the name of the failed action from the last RuleName attribute
            {
                var lastNode = doc.Root.Descendants().LastOrDefault(x => x.HasAttributes && x.Attributes().Any(z => z.Value == "StopProcessing"));
                var lastNodeParent = lastNode?.Parent;
                var ruleNameNode = doc.Root.Descendants().FirstOrDefault(x => x.HasAttributes && x.Attributes().Any(z => z.Value == "RuleName"));
                if (ruleNameNode != null)
                {
                    var succeededNode = lastNodeParent.Elements().FirstOrDefault(x => x.Attributes().Any(z => z.Value == "Succeeded"));
                    if (succeededNode != null && bool.TryParse(succeededNode.Value, out bool v) && v)
                    {
                        ActionName = lastNodeParent.Elements().First(x => x.Attributes().Any(z => z.Value == "RuleName")).Value;
                    }
                    else
                        ActionName = "No Action Name";
                }
                else
                    ActionName = "No Action Name";
            }

            //get the remote address from the RemoteAddress attribute
            ClientIpAddr = doc.Root.Descendants().First(x => x.HasAttributes && x.FirstAttribute.Value == "RemoteAddress").Value;

            //get the SystemTime that it started, with the SystemTime attribute
            Time = DateTime.Parse(doc.Root.Descendants().First(x => x.HasAttributes && x.Attributes().Any(z => z.Name.LocalName == "SystemTime")).Attribute("SystemTime").Value);


            IsValid = true;
        }

        public static IFailedReqLogItem LoadFailedReq(string file)
        {
            return new FailedReqLogItem(file);
        }
    }
}
