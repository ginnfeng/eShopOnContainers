using System;
using System.Diagnostics;

namespace Common.Support.Log
{
    class Logger:ILogger,IDisposable
    {
     
        public Logger(TraceSource[] tracesource)
        {
            this.Source = tracesource;
        }

        #region ILogAgent Members

        public void Flush(Exception exception,params  string[] appendMessages)
        {
            LogAgent.OutputFileTrace(exception);
            LogEntity logEntity=(LogEntity)exception;
            foreach(string message in appendMessages)
            {
                logEntity.Message=message;
            }
            Flush(logEntity);
        }
      
        public void Flush(ILogEntity log)
        {
            try
            {
                foreach (var src in Source)
                {
                    src.TraceData(log.LogMode, log.Idx, log);    
                }                
            }
            catch(Exception err) 
            {
                Debug.Write("Support.Log.Flush() " + err.Message);
            };
        }

        public TraceSource[] Source 
        {
            get { return tracesource; }
            set {tracesource=value;}
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
                }
            }
            //resource = null;
            disposed = true;
        }
        private bool disposed; 
        #endregion

        private TraceSource[] tracesource;
        

        //static ReaderWriterLock rwLock = new ReaderWriterLock();
    }
}
