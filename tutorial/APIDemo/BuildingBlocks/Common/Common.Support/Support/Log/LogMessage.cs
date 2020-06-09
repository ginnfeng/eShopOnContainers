using System;
using System.Xml;
using System.Xml.Serialization;

namespace Support.Log
{
    [Serializable]
    public class LogMessage
    {
        public LogMessage()
        {
            TimestampXml=DateTime.Now.TimeOfDay.ToString();
        }
        [XmlAttribute(AttributeName = "TS")]
        public string TimestampXml 
        {
            get { return timestampXml; }
            set { timestampXml = value; }
        }
        [XmlText]
        public string Content 
        {
            get { return content; }
            set { content = value; }
        }

        private string timestampXml ;
        private string content;

    }
}
