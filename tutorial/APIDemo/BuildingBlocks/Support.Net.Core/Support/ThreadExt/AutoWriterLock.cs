////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 6/21/2011 6:02:51 PM 
// Description: AutoWriterLock.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Threading;
#if SILVERLIGHT
using Support.SilverLight.ThreadExt;
#endif

namespace Support.ThreadExt
{
    public class AutoWriterLock:IDisposable
    {
#if SILVERLIGHT
        public AutoWriterLock(ReaderWriterLock readerWriterLock)           
        {
            this.readerWriterLock = readerWriterLock;            
        }
#else
        public AutoWriterLock(ReaderWriterLock readerWriterLock)
            : this(readerWriterLock, Timeout.Infinite)
        {
        }
        public AutoWriterLock(ReaderWriterLock readerWriterLock,int timeout)
        {
            this.readerWriterLock = readerWriterLock;
            this.readerWriterLock.AcquireWriterLock(timeout);
        }
#endif

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
                    readerWriterLock.ReleaseWriterLock();
                }
            }
            //resource = null;
            disposed = true;
        }
        private bool disposed; 
        private ReaderWriterLock readerWriterLock;
    }
}
