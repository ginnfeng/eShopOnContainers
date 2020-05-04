using System;
using System.Threading;

namespace Support.ThreadExt
{
    //public delegate void TaskRuntimeHandler<T>(object sender,T result);
    public delegate void TaskRuntimeEventHandler(object sender, object taskContent);
    public delegate void TaskErrorEventHandler(object sender, Exception exception);

    public interface ISmartTaskContent
    {
        void OnRun();
    }
    public class SmartTaskContent<T> : ISmartTaskContent
    {
        public SmartTaskContent() { }            

        public void OnRun()
        {
            Action(Ower);
        }
        public Action<T> Action { get; set; }
        public T Ower { get; set; }      
    }

    public interface ISmartTask
    {
        void Callback(Object state);
        void Callback(Object state, bool isTimeout);

        TaskControl Controller { set; get; }

        bool Wait();
        bool Wait(TimeSpan timeSpan);
        void Start();
        int ErrorSerialCount { get;}
        ISmartThreadPool ThreadPoolHost { get; set; }
        Exception ErrorResult { get; }
        event TaskRuntimeEventHandler BeginEvent;
        event TaskRuntimeEventHandler CompleteEvent;
        event TaskRuntimeEventHandler ErrorEvent;
        object GetTaskContent();
        void ReactiveAfterException();
    }

    public interface ISmartTask<T> : ISmartTask
        where T :new()
    {       
        T TaskContent { set; get; }
    }

    public class SmartTask<T> : ISmartTask<T>
        where T : new()
    {
        virtual public void OnBegin()
        {
            if (BeginEvent != null)
                BeginEvent(this, TaskContent);
            exception = null;
        }
        virtual public void OnEnd()
        {
            if (CompleteEvent != null)
                CompleteEvent(this, TaskContent);
            waitEvent.Set();
        }


        public SmartTask()
        {   //只執行一次
            control = null;
            TaskContent = new T();
        }

        public SmartTask(TaskControl taskControl)
        {
            control = taskControl;
            TaskContent = new T();
        }

        public SmartTask(T task)
        {
            control = null;
            TaskContent = task;
        }

        public SmartTask(T task, TaskControl taskControl)
        {  
            control = taskControl;
            TaskContent = task;
        }

        public void Start()
        {
            ThreadPoolHost.PushTask(this);
        }
        public bool Wait(TimeSpan timeSpan)
        {
            bool exitContext = true;
            return waitEvent.WaitOne(timeSpan, exitContext);
        }
        public bool Wait()
        {
            return waitEvent.WaitOne();
        }
        public void ReactiveAfterException()
        {
            this.Start();
            this.Controller.SetSignal();
        }
        public int ErrorSerialCount { get; private set; }

        public ISmartThreadPool ThreadPoolHost { get; set; }
        

        public void Callback(Object state)
        {
            try
            {               
                this.theState = state;
                if (Controller.CheckIn())
                {
                    OnBegin();
                    OnRun();
                    OnEnd();
                    Controller.Checkout();
                    ErrorSerialCount = 0;
                }
            }
            catch (Exception e)
            {
                ErrorSerialCount++;
                exception = e;
                if (ErrorEvent != null)
                {
                    ErrorEvent(this, TaskContent);
                }

            }
        }
        public void Callback(Object state, bool isTimeout)
        {
            this.theIsTimeout = isTimeout;
            Callback(state);
        }
        public T TaskContent
        {
            get
            {
                return content;
            }
            set
            {
                lock (this)
                {
                    content = value;
                }
            }
        }
        public object GetTaskContent()
        {
            return TaskContent;
        }

        public TaskControl Controller
        {
            set
            {
                if (control != null && control != value)
                    control.Dispose();
                control = value;
            }
            get
            {
                return control;
            }
        }
        public Exception ErrorResult
        {
            get
            {
                return exception;
            }
        }
        public event TaskRuntimeEventHandler BeginEvent;
        public event TaskRuntimeEventHandler CompleteEvent;
        public event TaskRuntimeEventHandler ErrorEvent;

        private void OnRun()
        {
            var it = TaskContent as ISmartTaskContent;
            if(it!=null)
                it.OnRun();
            else
            {
                dynamic the = TaskContent;
                the.OnRun();
            }

        }

        #region Member Data

        protected TaskControl control;

        //private ISmartThreadPool ownerPool = null;
        protected object theState = null;
        protected bool theIsTimeout = false;
        protected ManualResetEvent waitEvent = new ManualResetEvent(false);
        protected Exception exception = null;
        protected T content;


        #endregion
    }

}
