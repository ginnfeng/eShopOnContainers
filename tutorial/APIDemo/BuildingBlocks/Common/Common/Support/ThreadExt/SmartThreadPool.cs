using System;
using System.Threading;
using System.Collections.Generic;

namespace Common.Support.ThreadExt
{

    public interface ISmartThreadPool
	{        
		void PushTask(ISmartTask task);
		void PushTask(ISmartTaskContent taskContent);
		//TaskControl FireTask(ISmartTask task);
		void RegisterTask(ISmartTask smartTask);
		void UnregisterTask(ISmartTask smartTask);
	}
    public abstract class SmartThreadPoolBase : ISmartThreadPool
    {
        public SmartThreadPoolBase(int maxThreadCount)
        {
            this.MaxThreadCount = maxThreadCount;
            this.MaxActiveTaskCount = maxThreadCount;
        }
        abstract protected TaskControl DoFireTask(ISmartTask task);
        public TaskControl FireTask(ISmartTask task)
        {
            try 
	        {	        
		        return DoFireTask(task);
            }finally
            {
                Interlocked.Increment(ref busyTaskCount);
            }
        }
        virtual protected void DoTaskPlanning()
        {
            lock (this)
            {
                for (int i = BusyTaskCount; ((taskQueue.Count != 0) && (i < MaxActiveTaskCount)); i++)
                {
                    FireTask(taskQueue.Dequeue());
                }
            }
        }
        public void PushTask(ISmartTaskContent taskContent)
        {
            PushTask(CreateTask(taskContent));
        }
        public void PushTask(ISmartTask task)
        {
            RegisterTask(task);
            if (task != null)
                taskQueue.Enqueue(task);
            DoTaskPlanning();
        }
        public void CleanTasks()
        {
            taskQueue.Clear();
        }
        public SmartTask<T> CreateTask<T>() where T : new() //ISmartTaskContent, new()
        {
            SmartTask<T> task = new SmartTask<T>();
            RegisterTask(task);
            //PushTask(task); 需run task.Start()才會開始
            return task;
        }

        public SmartTask<T> CreateTask<T>(T taskContent) where T : new()//ISmartTaskContent, new()
        {
            SmartTask<T> task = new SmartTask<T>(taskContent);
            RegisterTask(task);
            //PushTask(task); 需run task.Start()才會開始
            return task;

        }

        public ISmartTask CreateTask(ISmartTaskContent taskContent)
        {
            Type templateType = typeof(SmartTask<>);
            Type[] tParamTypes = new Type[] { taskContent.GetType() };
            Type smartTaskType = templateType.MakeGenericType(tParamTypes);
            object[] constructorParam = new object[] { taskContent, null };
            ISmartTask smartTask = (ISmartTask)Activator.CreateInstance(smartTaskType, constructorParam);

            RegisterTask(smartTask);
            //PushTask(task); 需run task.Start()才會開始
            return smartTask;

        }
        public int MaxThreadCount { get; set; }
        public int MaxActiveTaskCount { get; protected set; }

        public int BusyTaskCount
        {
            get{ return busyTaskCount;}
        }
        public int TaskCount
        {
            get { return taskQueue.Count; }
        }
        public void RegisterTask(ISmartTask smartTask)
        {
            if (smartTask.ThreadPoolHost == this)
                return;
            if (smartTask.ThreadPoolHost != null)
            {
                smartTask.ThreadPoolHost.UnregisterTask(smartTask);
            }
            smartTask.ThreadPoolHost = this;
            smartTask.BeginEvent += new TaskRuntimeEventHandler(OnTaskBeginEvent);
            smartTask.CompleteEvent += new TaskRuntimeEventHandler(OnTaskCompleteEvent);
            smartTask.ErrorEvent += new TaskRuntimeEventHandler(OnTaskErrorEvent);
        }

        public void UnregisterTask(ISmartTask smartTask)
        {
            smartTask.ThreadPoolHost = null;
            smartTask.BeginEvent -= new TaskRuntimeEventHandler(OnTaskBeginEvent);
            smartTask.CompleteEvent -= new TaskRuntimeEventHandler(OnTaskCompleteEvent);
            smartTask.ErrorEvent -= new TaskRuntimeEventHandler(OnTaskErrorEvent);
        }
        virtual protected void OnTaskBeginEvent(object sender, object taskContent)
        {
            //
        }

        virtual protected void OnTaskCompleteEvent(object sender, object taskContent)
        {
            Interlocked.Decrement(ref busyTaskCount);
            DoTaskPlanning();
        }

        virtual protected void OnTaskErrorEvent(object sender, object taskContent)
        {
            Interlocked.Decrement(ref busyTaskCount);
            DoTaskPlanning();
        }

        

        //System.Threading.ThreadPool pool;
        private int busyTaskCount;        
        protected Queue<ISmartTask> taskQueue = new Queue<ISmartTask>();
    }
    public class SmartThreadPool : SmartThreadPoolBase
    {		
		public SmartThreadPool()
            :base(int.MaxValue)
		{			
		}
		public SmartThreadPool(int maxThreadCount)
            : base(maxThreadCount)
        {			
		}	
		override protected TaskControl DoFireTask(ISmartTask task)
		{
            if (task.Controller == null)
            {
                task.Controller = new TaskControl(true, null);
                ThreadPool.QueueUserWorkItem(new WaitCallback(task.Callback), null);
                return null;
            }
            else
            {
                TaskControl tc = task.Controller;
                RegisteredWaitHandle regHandle = ThreadPool.RegisterWaitForSingleObject(tc.OnRunEvent, new WaitOrTimerCallback(task.Callback), null, tc.TimeoutInterval, tc.ExecuteOnlyOnce);
                tc.RegisteredWaitHandle = regHandle;
                return tc;
            }
        }
		
        

    }
}
