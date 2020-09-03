using System;
using System.Collections.Generic;

using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.Xml.Serialization;
using System.Xml;
using System.Globalization;

namespace Common.Support.Log
{
    [Serializable]    
    public class LogEntityBase:ILogEntity
    {
        public LogEntityBase()
        {
            MessageList = new List<LogMessage>();
        }
        public LogEntityBase(TraceEventType type)
        {
            MessageList = new List<LogMessage>();
            LogMode = type;
        }

        #region ILogEntity Members
        [XmlAttribute(AttributeName = "Idx")]
        public int Idx 
        {
            get {return idx ;}
            set {idx = value;} 
        }
        
        [XmlAttribute(AttributeName="Mode")]
        public TraceEventType LogMode 
        {
            get { return logEntityType; }
            set { Begin(value); }
        }
        
        
        [XmlAttribute(AttributeName = "PID")]
        public int ProcessId
        {
            get { return processId; }
            set { processId = value; }
        }

        [XmlAttribute(AttributeName = "TID")]
        public int ThreadId
        {
            get { return threadId; }
            set { threadId = value; }
        }

        [XmlAttribute]
        public string Category
        {
            get { return category; }
            set { category = value; }
        }
        [XmlAttribute(AttributeName = "Reason")]
        public string Reason 
        {
            get { return reason; }
            set { reason = value; }
        }

        [XmlAttribute(AttributeName = "TS")]
        public DateTime Timestamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }

        [XmlAttribute("Interval", DataType = "duration")]
        public string IntervalXml
        {
            get
            {
                return (Interval < TimeSpan.FromTicks(1)) ? null : XmlConvert.ToString(Interval);
            }
            set
            {
                Interval = (value == null) ? TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
            }
        }

        [XmlAttribute(AttributeName = "Bypass")]
        public bool IsBypass
        {
            get { return isBypass; }
            set { isBypass = value; }
        }

        public void Bypass()
        {
            IsBypass = true;
        }
        [XmlAttribute]
        public string HelpLink
        {
            get { return helpLink; }
            set { helpLink = value; }
        }

        public string Source
        {
            get { return source; }
            set { source = value; }
        }
        
        public string StackTrace
        {
            get { return stackTrace; }
            set { stackTrace = value; }
        }

        [XmlIgnore]
        public string Message
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (LogMessage msg in MessageList)
                {
                    stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture,"[{0}]   {1}",msg.TimestampXml,msg.Content));
                }
                return stringBuilder.ToString();
            }
            set { AppendMessage(value); } 
        }
        [XmlIgnore]
        public object XmlMessage 
        {
            set 
            {
                try
                {
                    Message = (value == null) ? "null" : MsgTransfer.Object2Xml(value).OuterXml;
                }
                catch (Exception)
                {
                    Message = value.ToString();
                }
            }
        }

        [XmlElement(ElementName="Msg")]        
        public List<LogMessage> MessageList 
        {
            get { return messageList; }
            set { messageList = value; }
        }
        public void AppendMessage(object message, bool autoFlush)
        {
            AppendFormatMessage("{0}", message);
            if (autoFlush)
                Flush();
        }

        public void AppendMessage(object message)
        {
            AppendMessage(message, false);
        }
        public void AppendFormatMessage(string format, params object[] args)
        {
            LogMessage msg = new LogMessage();
            msg.Content=string.Format(CultureInfo.InvariantCulture, format, args);
            MessageList.Add(msg);

            //System.Diagnostics.Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "[{0}] {1}", msg.TimestampXml, msg.Content));

        }
        [XmlIgnore]
        public TimeSpan Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        [XmlIgnore]
        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }
        
        public void Flush()
        {          
            if (LogMode == TraceEventType.Warning && !IsBypass)
            {
                LogMode = TraceEventType.Error;
                Message = "Detecting an exception be throwed!";
            }
            if (MessageList.Count == 0 && LogMode == TraceEventType.Warning)
                return;
            Interval = DateTime.Now - Timestamp;
            Logger.Flush(this);
            messageList.Clear();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass of this type implements a finalizer.
            System.GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //TODO: Add resource.Dispose() logic here
                    Flush();
                }
            }
            //resource = null;
            disposed = true;
        }
        private bool disposed; 
        #endregion

        

        private void Begin(System.Diagnostics.TraceEventType type)
        {
            logEntityType = type;
            
            Timestamp = DateTime.Now;
            ProcessId = Process.GetCurrentProcess().Id;
            ThreadId = Thread.CurrentThread.ManagedThreadId;
            switch (type)
	        {
		        case TraceEventType.Error:
                 break;                 
                //case LogMode.Trace:                    
                //    Format(true);
                // break;
                default:
                    Format(false);
                 break;
	        }
        }
        public override string ToString()
        {
            StringBuilder infos = new StringBuilder("\r\n");
            infos.Append(Message);
            Format(ref infos, null, "-----------------------------------------------------");
            Format(ref infos, null, ExtInfo());
            return infos.ToString();
        }
        public string ExtInfo()
        {
            StringBuilder infos = new StringBuilder();
            Format(ref infos, "Interval", Interval);
            Format(ref infos, "Source", Source);            
            Format(ref infos, "StackTrace", this.StackTrace);
            Format(ref infos, "HelpLink", HelpLink);
            Format(ref infos, "Reason", Reason);
            return infos.ToString();
        }
        private void Format(bool wantStackTraceInfo)
        {            
            StackTrace trace = new StackTrace(true);
            StringBuilder stackTraceBuilder = new StringBuilder();
            foreach (StackFrame frame in trace.GetFrames())
            {
                MethodBase methodInfo = frame.GetMethod();
                if (methodInfo != null)
                {
                    if (methodInfo.DeclaringType==null)
                        continue;
                    if( methodInfo.DeclaringType.GetInterface(typeof(ILogEntity).FullName) != null
                        || methodInfo.DeclaringType.GetInterface(typeof(ILogAgent).FullName) != null)
                        continue;
                    
                    if (Source == null)
                    {
                        //Source = stackTraceBuilder.ToString();
                        string traceInfo = string.Format(CultureInfo.InvariantCulture,"{0}({1},{2}): {3}.{4};   "
                            , frame.GetFileName()
                            , frame.GetFileLineNumber()
                            , frame.GetFileColumnNumber()
                            , frame.GetMethod().DeclaringType.Name
                            , frame.GetMethod());
                        stackTraceBuilder.Append(traceInfo);
                        Source = traceInfo;
                        stackTraceBuilder.Append("\n");
                        //System.Diagnostics.Debug.Write(stackTraceBuilder);
                        if (!wantStackTraceInfo) break;
                    }
                    else
                    {
                        stackTraceBuilder.AppendFormat("{0}.{1};   \n", frame.GetMethod().DeclaringType.Name, frame.GetMethod());
                    }
                }               
            }
            if(wantStackTraceInfo)
                StackTrace =stackTraceBuilder.ToString();
        }
        
        private void Format(ref StringBuilder infos, string name, object value)
        {
            Format(ref infos, name, "= ", value);
        }
        private void Format(ref StringBuilder infos, string name, string delimit, object value)
        {
            if (value == null) return;
            if (!string.IsNullOrEmpty(name))
                infos.Append(name).Append(delimit);
            infos.Append(value).Append("\r\n");
        }
        private int idx;
        private System.Diagnostics.TraceEventType logEntityType;
        private TimeSpan interval;
        private ILogger logger;
        private string stackTrace;
        private string source;
        private bool isBypass; //false is the default
        private DateTime timestamp;
        private string category;
        private int threadId;
        private int processId;
        private string reason;
        private string helpLink;        
        private List<LogMessage> messageList;

    }    
}
