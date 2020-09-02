////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 6/21/2011 6:02:30 PM 
// Description: AutoReaderLock.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Threading;
#if SILVERLIGHT
using Support.SilverLight.ThreadExt;
#endif

namespace Common.Support.ThreadExt
{

    public class AutoReaderLock : IDisposable
    {
#if SILVERLIGHT
        public AutoReaderLock(ReaderWriterLock readerWriterLock)         
        {
            this.readerWriterLock = readerWriterLock;
        }
#else
        public AutoReaderLock(ReaderWriterLock readerWriterLock)
            : this(readerWriterLock, Timeout.Infinite)
        {
        }

        public AutoReaderLock(ReaderWriterLock readerWriterLock, int timeout)
        {
            this.readerWriterLock = readerWriterLock;
            this.readerWriterLock.AcquireReaderLock(timeout);
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
                    readerWriterLock.ReleaseReaderLock();
                }
            }
            //resource = null;
            disposed = true;
        }
        private bool disposed;
        private ReaderWriterLock readerWriterLock;
    }
}
