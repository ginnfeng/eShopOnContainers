using System;
using System.Threading;

namespace Support.ThreadExt
{
    public class TaskControl:IDisposable
    {
        private WaitHandle onRunEvent;
        private RegisteredWaitHandle registeredWaitHandle;
        private int timeOutInterval = Timeout.Infinite;
        private Timecard timecard;
        bool executeOnlyOnce = true;


        public TaskControl(bool isExecuteOnlyOnce, Timecard timecard)
        {
            //立即執行一次,但之後需再下SetSignal()才會再執行
            onRunEvent = new AutoResetEvent(true);
            Timecard = timecard;
            executeOnlyOnce = isExecuteOnlyOnce;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isAutoResetEvent">AutoResetEvent==true 時不會執行,除非下SetSignal</param>
        /// <param name="initSignaled"></param>
        /// <param name="isExecuteOnlyOnce"></param>
        public TaskControl(bool isAutoResetEvent, bool initSignaled, bool isExecuteOnlyOnce)
        {
            if (isAutoResetEvent) //AutoResetEvent 時不會執行,除非下set()
                onRunEvent = new AutoResetEvent(initSignaled);
            else//ManualResetEvent 時會不停執行,直到下reset()(目前只要一啟動就停不下,原因待查)
                onRunEvent = new ManualResetEvent(initSignaled);
            executeOnlyOnce = isExecuteOnlyOnce;
        }
        
        

        public TaskControl(WaitHandle runEvent, bool isExecuteOnlyOnce)
        {
            onRunEvent = runEvent;
            executeOnlyOnce = isExecuteOnlyOnce;
        }
        public bool Unregister()
        {
            if (onRunEvent != null && registeredWaitHandle != null)
                return registeredWaitHandle.Unregister(onRunEvent);
            return true;
        }
        public bool CheckIn()//檢查執行的時間到了沒
        {            
            if ((timecard == null || timecard.TryGoNextMilestone() ))
            {
                return true;
            }
            Thread.Sleep(200);
            SetSignal();
            return false;
        }

        public void Checkout()
        {
            if (!executeOnlyOnce)
                SetSignal();
        }

        public void SetSignal()
        {
            if (onRunEvent is Mutex)
                ((Mutex)onRunEvent).ReleaseMutex();
            else if (onRunEvent is AutoResetEvent)
                ((AutoResetEvent)onRunEvent).Set();
            else if (onRunEvent is ManualResetEvent)
                ((ManualResetEvent)onRunEvent).Set();

        }
        public void ResetSignal()
        {
            //目前無作用,原因待查
            if (onRunEvent is Mutex)
                ((Mutex)onRunEvent).WaitOne();
            else if (onRunEvent is AutoResetEvent)
                ((AutoResetEvent)onRunEvent).Reset();
            else if (onRunEvent is ManualResetEvent)
                ((ManualResetEvent)onRunEvent).Reset();

        }

        //*****************************************************************
        public WaitHandle OnRunEvent
        {
            get
            {
                return onRunEvent;
            }
        }
        public RegisteredWaitHandle RegisteredWaitHandle
        {
            set
            {
                registeredWaitHandle = value;
            }
        }
        public int TimeoutInterval
        {
            set
            {
                timeOutInterval = value;
            }
            get
            {
                return timeOutInterval;
            }
        }
        public bool ExecuteOnlyOnce
        {
            set
            {
                executeOnlyOnce = value;
            }
            get
            {
                return executeOnlyOnce;
            }
        }
        public Timecard Timecard
        {
            set
            {
                timecard = value;
            }
            get
            {
                return timecard;
            }
        }

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
                    onRunEvent.Close();
                }
            }
            onRunEvent = null;
            disposed = true;
        }
        private bool disposed; 
    }
}
