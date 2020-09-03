using System;
using System.Diagnostics;

namespace Common.Support.Logger
{
    public interface ILogger
    {
        void Flush(Exception exception,params  string[] appendMessages);
        void Flush(ILogEntity log);    
       
        TraceSource[] Source { get; set; }
    }
}
