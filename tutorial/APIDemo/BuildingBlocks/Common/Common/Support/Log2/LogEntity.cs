using System;
using Common.Support.ErrorHandling;
using System.Diagnostics;

namespace Common.Support.Log
{
    [Serializable]    
    public class LogEntity : LogEntityBase
    {
        static public explicit operator LogEntity(Exception exception)
        {            
            LogEntity log = new LogEntity();
            log.LogMode = TraceEventType.Error;
            
            log.Source = exception.Source;
            log.StackTrace = exception.StackTrace;           

            log.Message = exception.Message;
            
            log.HelpLink = exception.HelpLink;
            IExceptionExt execptionExt=exception as IExceptionExt;            
            log.Reason = (execptionExt == null || execptionExt.Detail.Reason == null) ? null : execptionExt.Detail.Reason.ToString();            
            log.Category = (execptionExt == null) ? exception.GetType().FullName : execptionExt.Detail.GetType().FullName;
            return log;
        }
        
        public LogEntity()
        {           
        }
        public LogEntity(TraceEventType type)
            :base(type)
        {
        }
        public LogEntity(TraceEventType type, ILogger logger)
            : base(type)
        {
            Logger = logger;
        }
    }
}
