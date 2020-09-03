using System;
using System.Diagnostics;
namespace Common.Support.Log
{

    public interface ILogEntity : IDisposable
    {
        int Idx { get; set; }
        string  Reason{get; set; }
    
        /// <summary>
        /// 提供Log間串聯資訊,或Url
        /// </summary>
        string HelpLink { get; set; }
        int ProcessId { get; set; }        
        int ThreadId { get; set; }
        TraceEventType LogMode { get; set; }
        

        DateTime Timestamp { get; set; }
        TimeSpan Interval { get; set; }        
        string Source { get; set; }
        string StackTrace { get; set; }
        string Category { get; set; }
        
        string Message { get; set; }
        object XmlMessage { set; }
        void AppendMessage(object message,bool autoFlush);
        void AppendMessage(object message);      
        void AppendFormatMessage(string format, params object[] args);                   
        ILogger Logger { get; set; }
        void Flush();
        void Bypass();

        
    }
}
