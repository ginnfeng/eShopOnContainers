using System;
using System.Reflection;

namespace Support.ErrorHandling
{
    public interface IExceptionExt
    {        
        Exception InnerException { get; }
        string Message { get; }       
        string StackTrace { get; }
#if !SILVERLIGHT
        MethodBase TargetSite { get; }
        string HelpLink { get; set; }
        string Source { get; set; }
#endif
        Exception CloneSimple();
        
        IErrorInfo Detail { get; }
    }
}
