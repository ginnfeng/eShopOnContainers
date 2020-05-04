using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Globalization;

namespace Support.ErrorHandling
{
#if !SILVERLIGHT
    [Serializable]
#endif

    public class Exception<T> : Exception, IExceptionExt
        where T:IErrorInfo,new()
    {
       public Exception()
        {        
        }

       public Exception(T errorInfo)
       {
           ErrorInfo = errorInfo;
       }

       public Exception(T errorInfo, Exception innerException)
           : base("", innerException)
       {
#if !SILVERLIGHT
           HelpLink = innerException.HelpLink;
#endif
           ErrorInfo = errorInfo;
       }
      

       public Exception(Exception innerException)
           : base("", innerException)
       {           
#if !SILVERLIGHT
           HelpLink = innerException.HelpLink;
#endif
       }
        public Exception(string message, params object[] args)
           : base(CommonExtension.StringFormat(message, args))
        {            
            ErrorInfo.AppendedMessage = base.Message;
        }

        public Exception(string message, Exception innerException)
            :base(message,innerException)
        {
#if !SILVERLIGHT
            HelpLink = innerException.HelpLink;
#endif
            ErrorInfo.AppendedMessage = message;
        }
#if !SILVERLIGHT
        protected Exception(SerializationInfo info, StreamingContext context)
            :base(info,context)
        {
            if (info == null) throw new ArgumentNullException("info");
            ErrorInfo=(T)info.GetValue(typeof(T).Name,typeof(T));            
        }        

        /// <summary>
        /// ISerializable interface are not automatically included in the serialization process. 
        /// To include the fields, the type must implement the GetObjectData method and the serialization constructor.        
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        override   public void GetObjectData(SerializationInfo info, StreamingContext context) 
        {
            base.GetObjectData(info, context);
            info.AddValue(typeof(T).Name, ErrorInfo);            
        }
        public override string HelpLink
       {
           get
           {
               if (base.HelpLink == null)
                   base.HelpLink = Guid.NewGuid().ToString();
               return base.HelpLink;
           }
           set
           {
               base.HelpLink = value;
           }
       }
#endif       


        public T ErrorInfo 
        {
            get{return errorInfo;}
            set{errorInfo=value;}
        }

         public override string Message
        {
            get
            {
                return string.Format(CultureInfo.CurrentCulture,"{0}  {1}",ErrorInfo.Message,(InnerException==null)?"": ("# "+InnerException.Message) );  
            }
        }

         public Exception CloneSimple()
         {
             string msg = string.Format(CultureInfo.CurrentCulture, "{0}", ErrorInfo.AppendedMessage);
#if !SILVERLIGHT
             return new Exception<T>(msg) { HelpLink = HelpLink, HResult = HResult, Reason = Reason };
#else
             return new Exception<T>(msg) { HResult = HResult, Reason = Reason };
#endif

         }

       #region IErrorInfo Members

         public object Reason
       {
           get
           {
               return ErrorInfo.Reason;
           }
           set
           {
               ErrorInfo.Reason = value;
           }
       }

       public object Reference
       {
           get
           {
               return ErrorInfo.Reference;
           }
           set
           {
               ErrorInfo.Reference=value;
           }
       }

       public List<string> DumpingFieldList
       {
           get
           {
               return ErrorInfo.DumpingFieldList;
           }
           set
           {
               ErrorInfo.DumpingFieldList=value;
           }
       }
       
       public string AppendedMessage
       {
           get
           {
               return ErrorInfo.AppendedMessage;
           }
           set
           {
               ErrorInfo.AppendedMessage=value;
           }
       }

       public IErrorInfo Detail 
       {
           get { return ErrorInfo; }
       }
       
       #endregion      
       
       private T errorInfo=new T() ;
      
    }

    
}
