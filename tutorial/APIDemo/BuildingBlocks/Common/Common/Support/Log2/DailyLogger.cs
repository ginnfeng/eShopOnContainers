////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 8/16/2013 10:42:14 AM 
// Description: DailyLogger.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Principal;
using System.Text;

namespace Common.Support.Log
{
    public enum ActionType
    {
        Info,
        Startup,
        End,
        Restart,
        Init,
        Monitor,
        Manage,
        Engine,
        Flow,
        Insert,
        Update,
        Delete,
        Read,
        Write,
        Undo,
        Processing
    }
    public class TraceInfo : TraceInfo<ActionType>
    {
        public TraceInfo(object input)
            : base(input)
        {
        }
        public TraceInfo(object input, ActionType type)
            : base(input, type)
        {
        }
        public TraceInfo(object input, object result, ActionType type)
            : base(input, result, type)
        {
        }
    }
    public class DailyLogger : DailyLogger<ActionType>
    {

        static public DailyLogger Instance { get { return Support.Singleton0<DailyLogger>.Instance; } }
        public void WriteStartupLog(string info)
        {
            var proc = Process.GetCurrentProcess();
            info = string.Format("PID#{0}:{1},{2}, {3}", proc.Id, proc.ProcessName, WindowsIdentity.GetCurrent().Name, info);
            Write(new TraceInfo<ActionType>(info, ActionType.Startup));
        }
        public void WriteEndLog(string info)
        {
            var proc = Process.GetCurrentProcess();
            info = string.Format("PID#{0}:{1},{2}, {3}", proc.Id, proc.ProcessName, WindowsIdentity.GetCurrent().Name, info);
            Write(new TraceInfo<ActionType>(info, ActionType.End));
        }
    }

    public class TraceInfo<TActionType>
    {
        public TraceInfo(object input)
            : this(input, null, DefaultActionType)
        {
        }
        public TraceInfo(object input, TActionType type)
            : this(input, null, type)
        {
        }
        public TraceInfo(object input, object result, TActionType type)
        {
            At = DateTime.Now;
            Action = type;
            Inputs = new List<object>();
            Inputs.Add(input);
            Result = result;
        }

        public TActionType Action { get; set; }
        public object Input
        {
            get
            {
                return Inputs.Count == 0 ? null : Inputs[0];
            }
            set
            {
                Inputs.Clear();
                Inputs.Add(value);
            }
        }
        public List<object> Inputs { get; set; }
        public object Result
        {
            get { return result; }
            set { result = value; Success = value as Exception == null; }
        }
        public DateTime At { get; set; }
        public bool Success { get; set; }
        public string SourceTypeName { get; set; }

        public override string ToString()
        {
            StringBuilder inps = new StringBuilder();
            foreach (var str in Inputs)
            {
                if (inps.Length != 0) inps.Append(",");
                inps.Append(str);
            }
            var rltString = ResultString();
            var tmStr = At.ToString("yyyy/MM/dd HH:mm:ss");

            if (string.IsNullOrEmpty(rltString))
                return string.Format("{0}, {1,-11},{2}, {3,-16},Inp=[{4}]", tmStr, Action, Success, Truncate(SourceTypeName, truncateLenght), inps);
            return string.Format("{0}, {1,-11},{2}, {3,-16},Inp=[{4}],Result={5}", tmStr, Action, Success, Truncate(SourceTypeName, truncateLenght), inps, rltString);

        }

        public string ResultString(bool detail = true)
        {
            if (Success) return Result == null ? null : Result.ToString();
            var err = Result as Exception;
            return string.Format("ExceptionType:{0}\r\n{1}.{2}\r\n{3}", err.GetType().Name, err.Message, err.InnerException == null ? "" : err.InnerException.Message, detail ? err.StackTrace : "");
        }
        private string Truncate(string it, int length)
        {
            if (string.IsNullOrEmpty(it) || it.Length <= length) return it;

            return it.Substring(0, length);
        }
        private object result;
        static public TActionType DefaultActionType { get { return defaultActionType; } }
        static private TActionType defaultActionType = Activator.CreateInstance<TActionType>();
        internal readonly static int truncateLenght = 16;
    }
    public interface IDailyLogger<TActionType>
    {
        //void WriteStartupLog(string info);
        void Write(string info);
        void Write(TraceInfo<TActionType> traceInfo);
    }
    public class DailyLogger<TActionType> : IDailyLogger<TActionType>
    {
        internal DailyLogger()
        {

            defaultLogDir = Support.CommonExtension.StringFormat("{0}/Log/{1}"
                , Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                , AppDomain.CurrentDomain.FriendlyName);

        }
        public void Write<T>(object info)
        {
            Write<T>(info.ToString());
        }
        public void Write<T>(string info)
        {
            Write<T>(new TraceInfo<TActionType>(info));
        }
        public void Write<T>(string format, params object[] args)
        {
            Write<T>(string.Format(CultureInfo.CurrentCulture, format, args));
        }
        public void Write<T>(Exception err, object extInfo = null)
        {
            Write<T>(new TraceInfo<TActionType>(extInfo) { Result = err });
        }
        public void Write<T>(TraceInfo<TActionType> traceInfo)
        {
            traceInfo.SourceTypeName = typeof(T).Name;
            Write(traceInfo);
        }
        public void Write(object info)
        {
            Write(info.ToString());
        }
        public void Write(string format, params object[] args)
        {
            Write(string.Format(CultureInfo.CurrentCulture, format, args));
        }
        public void Write(string info)
        {
            Write(new TraceInfo<TActionType>(info));
        }
        public void Write(Exception err, object extInfo = null)
        {
            var traceInfo = new TraceInfo<TActionType>(extInfo) { Result = err };
            Write(traceInfo);
        }
        public void Write(TraceInfo<TActionType> traceInfo)
        {
            try
            {
                if (SourceType != null && string.IsNullOrEmpty(traceInfo.SourceTypeName))
                    traceInfo.SourceTypeName = SourceType.Name;
                lock (this)
                {
                    using (var stream = CreateLogFile())
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        var tm = DateTime.Now;
                        writer.WriteLine("{0}", traceInfo.ToString());
                        writer.Close();
                        stream.Close();
                    }
                }
            }
            catch
            {//避免Log反而造成當掉
            }
        }
        public string LogDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(logDirectory))
                    LogDirectory = defaultLogDir;
                return logDirectory;
            }
            set
            {
                Directory.CreateDirectory(value);
                logDirectory = value;
            }
        }
        private FileStream CreateLogFile()
        {
            //delete last month log
            File.Delete(GenLogPath(DateTime.Today.AddMonths(-1)));
            string path = GenLogPath(DateTime.Today);
            bool isExist = File.Exists(path);
            return !isExist ? File.Open(path, FileMode.CreateNew) : File.Open(path, FileMode.Append);
        }
        private string GenLogPath(DateTime date)
        {
            return string.Format("{0}/{1}.{2}.{3}.txt", LogDirectory, date.Day, date.Month, date.Year);
        }

        public Type SourceType { get; set; }
        static private string defaultLogDir;
        private string logDirectory;

    }


}
