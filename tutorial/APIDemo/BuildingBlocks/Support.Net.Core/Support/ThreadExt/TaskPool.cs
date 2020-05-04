////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 7/7/2016 5:13:54 PM 
// Description: TaskPool.cs  
// TaskPool2採比較簡單策略，Task執行無順序，且跨process控制cocurrent thread數(沒法在VS中debug觀察會全停住)，但thread比較多佔資源
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Support.ThreadExt
{

    public class TaskPool : SmartThreadPoolBase
    {
        public TaskPool(int maxThreadCount,string name=null)
            : base(maxThreadCount)
        {
            
        }
        override protected TaskControl DoFireTask(ISmartTask task)
        {
            task.Controller = (task.Controller ?? new TaskControl(true, null));
            var threadTask = new Task(() => task.Callback(null));            
            threadTask.Start();
            
            return task.Controller;            
        }
    }

    public class TaskPool2 : TaskPool
    {
        public TaskPool2(int maxThreadCount, string name = null)
            : base(maxThreadCount)
        {
            MaxActiveTaskCount = int.MaxValue;
            string semaphoreName = string.IsNullOrEmpty(name) ? Guid.NewGuid().ToString() : name;
            if (!Semaphore.TryOpenExisting(semaphoreName, out semaphore))
                semaphore = new Semaphore(maxThreadCount, maxThreadCount, semaphoreName);
        }

       
        override protected void OnTaskBeginEvent(object sender, object taskContent)
        {
            semaphore.WaitOne();
            base.OnTaskBeginEvent(sender, taskContent);
        }

        override protected void OnTaskCompleteEvent(object sender, object taskContent)
        {
            semaphore.Release();
            base.OnTaskCompleteEvent(sender, taskContent);
        }

        override protected void OnTaskErrorEvent(object sender, object taskContent)
        {            
            semaphore.Release();
            base.OnTaskErrorEvent(sender, taskContent);
        }      
      
        private Semaphore semaphore ;
    }
}
