using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics;
using System.Threading;


namespace Common.Support.Log
{
    public interface ILogAgent
    {
    }

    public class LogAgent : ILogAgent
    {

        static private LogAgent instance;
        static private LogAgent Instance
        {
            get
            {
                if (instance == null)
                    instance = new LogAgent();
                return instance;
            }
        }

        private LogAgent() { }
        static public ILogger Logger
        {
            get { return new Logger(Instance.CreateTraceSource()); } 
        }

        static public ActivityTrace NewActivity()
        {
            return Instance.CreateActivity(null);
        }

        static public ActivityTrace NewActivity(string name)
        {
            return Instance.CreateActivity(name) ;
        }
        /// <summary>
        /// Fatal error or application crash. 
        /// </summary>       
        static public ILogEntity NewCritical()
        {
            return Instance.CreateLogEntity(TraceEventType.Critical);
        }

        /// <summary>
        /// Recoverable error.
        /// </summary>        
        static public ILogEntity NewError()
        {
            return Instance.CreateLogEntity(TraceEventType.Error);
        }

        /// <summary>
        /// Auto dectecting the exception.
        /// </summary>
        static public ILogEntity NewSmartWarning()
        {
            return Instance.CreateLogEntity(TraceEventType.Warning);
        }

        /// <summary>
        /// Noncritical problem. 
        /// </summary>
        static public ILogEntity NewWarning()
        {
            ILogEntity logEntity = Instance.CreateLogEntity(TraceEventType.Warning);
            logEntity.Bypass();
            return logEntity;
        }

        /// <summary>
        /// Evidence/Informational message. 
        /// </summary>
        static public ILogEntity NewInformation()
        {
            return Instance.CreateLogEntity(TraceEventType.Information);
        }

        // <summary>
        /// Evidence/Informational message. 
        /// </summary>
        static public ILogEntity NewInformation(string category, string message, object xmlMessage)
        {
            ILogEntity logEntity = Instance.CreateLogEntity(TraceEventType.Information);
            logEntity.Category = category;
            logEntity.Message = message;
            logEntity.XmlMessage = xmlMessage;
            return logEntity;
        }

        /// <summary>
        /// Debugging trace.
        /// </summary>
        static public ILogEntity NewVerbose()
        {
            return Instance.CreateLogEntity(TraceEventType.Verbose);
        }
        static public void OutputFileTrace(Exception exception)
        {
#if DEBUG
            
            string stackTrace = exception.StackTrace;
            if (string.IsNullOrEmpty(stackTrace)) return;            
            Match match = regex.Match(stackTrace);
            if (match.Success)
            {
                string info = string.Format(CultureInfo.InvariantCulture, "{0}{1}({2},1)", match.Groups[2].Value, match.Groups[3].Value, match.Groups[6].Value);
                System.Diagnostics.Debug.WriteLine(info);
            }
#endif
        }
        
        
        private ActivityTrace CreateActivity(string name)
        {
            ActivityTrace activityTrace = new ActivityTrace(CreateTraceSource(), false){Name=name};
            if (string.IsNullOrEmpty(name))            
            {
                activityTrace.Name=CommonExtension.StringFormat
                    (
                    "{0}.{1}:activity{2}"
                    , Process.GetCurrentProcess().Id
                    , Thread.CurrentThread.ManagedThreadId
                    , activityTrace.GetHashCode()
                    );
            }
            activityTrace.Start();
            return activityTrace;
        }
        private ILogEntity CreateLogEntity(TraceEventType traceEventType)
        {
            Logger logger = new Logger(CreateTraceSource());
            return new LogEntity(traceEventType, logger);
        }
        private TraceSource[] CreateTraceSource()
        {
            var serviceModelSource= new LogTraceSource("System.ServiceModel", SourceLevels.All);
#if DEBUG
            return new TraceSource[] { serviceModelSource, new LogTraceSource("Support.Log", SourceLevels.All)};
#else
            return new TraceSource[] { serviceModelSource };
#endif        
        }

        //(in|在|於).{0,1}([A-Za-z]:)(.{1,})(:).{0,}(line|行)[^\d]{0,}(\d{1,})
        static Regex regex = new Regex("(in|在|於).{0,1}([A-Za-z]:)(.{1,})(:).{0,}(line|行)[^\\d]{0,}(\\d{1,})");
    }
}
