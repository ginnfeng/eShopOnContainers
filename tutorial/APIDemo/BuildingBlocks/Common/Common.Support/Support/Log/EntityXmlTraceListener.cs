////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 6/4/2009 11:15:02 AM 
// Description: EntityXmlTraceListener.cs  
// Revisions  :            		
// **************************************************************************** 
using System.Diagnostics;
using System.IO;

namespace Support.Log
{
    public class EntityXmlTraceListener : XmlWriterTraceListener
    {
        public EntityXmlTraceListener(Stream stream)
            : base(stream)
        {
            
        }
        
        public EntityXmlTraceListener(string filename)
            : base(GetFilePath(filename))
        {
            
        }
        public EntityXmlTraceListener(TextWriter writer)
        : base(writer)
        {
        }
        public EntityXmlTraceListener(Stream stream, string name)
        : base(stream)
        {
        }
        public EntityXmlTraceListener(string filename, string name)
        : base(filename,name)
        {
        }
        public EntityXmlTraceListener(TextWriter writer, string name)
        : base(writer,name)
        {
        }
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            LogEntity logEntity = data as LogEntity;
            string deccript=(logEntity == null)
                ?source
                : CommonExtension.StringFormat("{0}-{1}"
                    , string.IsNullOrEmpty(logEntity.Category) ? source : logEntity.Category
                    , (logEntity.MessageList.Count > 0) ? logEntity.MessageList[0].Content : ""
                    );
            if(logEntity == null)
            {
                base.TraceData(eventCache, deccript, eventType, id, data);
            }else
            {
                int size=logEntity.MessageList.Count;                
                object[] msgs=new object[size+1];
                for(int i=0;i<size;i++)
                {
                    LogMessage msg = logEntity.MessageList[i];
                    string msgString=CommonExtension.StringFormat("[{0}] {1}", msg.TimestampXml, msg.Content);
                    msgs.SetValue(msgString, i);
                }     
                // this.TraceOutputOptions
                msgs.SetValue(logEntity.ExtInfo(), size);
                base.TraceData(eventCache, deccript, eventType, id, msgs);
            }
           
        }

        
        static private string GetFilePath(string file)
        {
            if (file.Contains("\\"))
                return file;
            return CommonExtension.StringFormat("{0}\\{1}", SystemSetting.Instance.StorageDirectory,file);
        }
    }
}
