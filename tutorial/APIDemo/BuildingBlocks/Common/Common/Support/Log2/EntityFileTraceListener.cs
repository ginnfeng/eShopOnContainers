////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 6/2/2009 10:03:31 AM 
// Description: EntityTraceListener.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace Common.Support.Log
{
    public class EntityFileTraceListener : TraceListener//XmlWriterTraceListener //TraceListener//TextWriterTraceListener//TraceListener
    {
        public EntityFileTraceListener()
        {
            
            StorageDirectory = SystemSetting.Instance.StorageDirectory;
        }
        public EntityFileTraceListener(string initializeData)            
        {
            StorageDirectory = initializeData;
        }
        public override bool IsThreadSafe { get { return true; } }
        
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            LogEntity logEntity = data as LogEntity;
            if (logEntity == null)
            {
                base.TraceData(eventCache, source, eventType, id, data);
                return;
            }
            if (string.IsNullOrEmpty(logEntity.StackTrace))
                logEntity.StackTrace = eventCache.Callstack;
            XmlWriterSettings xmlWriterSetting = new XmlWriterSettings();
            xmlWriterSetting.OmitXmlDeclaration = true;

            using (MemoryStream memoryStream = new MemoryStream())
            using (TextWriter textWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, xmlWriterSetting))
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                //without namespaces
                ns.Add("", "");
                XmlSerializer serializer = new XmlSerializer(logEntity.GetType());
                serializer.Serialize(xmlWriter, logEntity, ns);
                string msg = Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
                WriteLine(msg);
            }
        }
        public override void Write(string message)
        {
           
        }

        public override void WriteLine(string message)
        {
            const string beginXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<Logs>\n";
            const string endXml = "</Logs>";
            string path = GenLogFilePath();
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
            using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.UTF8))
            {
                try
                {
                    var fileSize = fileStream.Length;
                    if (fileSize == 0)
                    {
                        textWriter.Write(beginXml);
                    }
                    else
                    {
                        fileStream.Position = fileSize - endXml.Length;
                    }

                    textWriter.Write(message);
                    textWriter.Write(endXml);
                } finally
                {                    
                    textWriter.Close();
                    fileStream.Close();                                       
                }
            }
        }

        public string Source
        {
            get
            {
                var source = this.Attributes["Source"];
                return (source != null) ? source.ToString() : null;

            }
        }
        protected override string[] GetSupportedAttributes()
        {
            return new string[] { "Source" };
        }


        private string GenLogFilePath()
        {
            char[] forbidChars = new char[] { '\\', '/', ' ' };
            string appName = AppDomain.CurrentDomain.FriendlyName.Trim(forbidChars);
            DateTime today = DateTime.Now.Date;
            return string.Format(CultureInfo.InvariantCulture, @"{0}\{1}_{2}.{3}.{4}_pid{5}.xml", StorageDirectory, appName, today.Year, today.Month, today.Day, Process.GetCurrentProcess().Id);
        }
        public string StorageDirectory
        {
            get
            {
                return storageDirectory;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Directory.CreateDirectory(value);
                    storageDirectory = value;
                }
            }
        }
        string storageDirectory;
    }
}
