using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

using System.Xml;
using System.Xml.Linq;

namespace returnzork.IIS_Log_Parser
{
    public struct FailedReqLogItem : IFailedReqLogItem
    {
        public string Url => throw new NotImplementedException();

        public string Host => throw new NotImplementedException();

        public string UserAgent => throw new NotImplementedException();

        public FailedAction Action => throw new NotImplementedException();

        public string ActionName => throw new NotImplementedException();


        private FailedReqLogItem(string file)
        {
            XDocument doc = XDocument.Load(file);
        }
    }
}
