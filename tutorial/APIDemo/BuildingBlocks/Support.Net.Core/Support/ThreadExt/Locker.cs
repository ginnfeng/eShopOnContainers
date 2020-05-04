////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 10/1/2008 3:32:47 PM 
// Description: Locker.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Threading;
using System.Globalization;

namespace Support.ThreadExt
{
    public class Locker<T>:Locker
    {
        public Locker()
            :base(typeof(T).FullName)
        {
        }
    }
    public class Locker : IDisposable
    {  
        
        public Locker(string id)
        {
            CreateWaitHandle(id);
        }
        public Locker(object it)
        {
            string id = it.GetHashCode().ToString(CultureInfo.CurrentCulture);
            CreateWaitHandle(id);
        }

        public bool WaitOne()
        {
            return eventWaitHandle.WaitOne();
        }

        public bool WaitOne(TimeSpan timeout)
        {
            return eventWaitHandle.WaitOne(timeout, false);
        }
        private void CreateWaitHandle(string id)
        {
            bool initState = true;
            //false: 啟動時自動reset statue, EventResetMode.ManualReset: Set()後能進入WaitOne()其後會被自動reset 
            //true: 啟動時若為已有reset statue則不變 ,否則init to set status 
            //EventResetMode.AutoReset，進入WaitOne(),馬上被自動reset
            eventWaitHandle = new EventWaitHandle(initState, EventResetMode.AutoReset, id);            
        }
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
                if (disposing && eventWaitHandle != null)
                {
                    eventWaitHandle.Set();
                    eventWaitHandle.Dispose();
                }
            }
            eventWaitHandle = null;
            disposed = true;
        }
        private bool disposed; 
        
        #endregion
        private EventWaitHandle eventWaitHandle;
    }
}
