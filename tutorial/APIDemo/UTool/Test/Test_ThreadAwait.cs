////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 3/31/2020 10:49:20 AM 
// Description: Test_Await.cs  
// Revisions  :            		
// **************************************************************************** 
using Support.ThreadExt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UTDll;


namespace UTool.Test
{
    class Test_ThreadAwait : UTest
    {
        public Test_ThreadAwait()
        {
            //
            // TODO: Add constructor logic here
            //      
        }
        [UMethod]
        public void T_UI()
        {
            Debug.WriteLine($"➳ T_UI(➽)--Start!  ThreadId:{Thread.CurrentThread.ManagedThreadId}");
            AwaitSimple();
            Debug.WriteLine($"➳ T_UI(➽)--End!  ThreadId:{Thread.CurrentThread.ManagedThreadId}");
            /*
                ➳ T_UI(➽)--Start!  ThreadId:4
                ➳ AwaitSimple()--Start!  ThreadId:4
                ➳ GetInfo()--Start!  ThreadId:4
                ➳ AsyncGetInfo()--Start!  ThreadId:4
                ➳ AsyncGetInfo()--End!  ThreadId:4
                  ➥ Task_A  threadId:5
                ➳ T_UI(➽)--End!  ThreadId:4
                  ➥ GetInfo()--End!  ThreadId:5
                  ➥ AwaitSimple()--End!  ThreadId:5
            */
        }

        async public void AwaitSimple()
        {
            Debug.WriteLine($"➳ AwaitSimple()--Start!  ThreadId:{Thread.CurrentThread.ManagedThreadId}");
            string x = await GetInfo("A"); //Task<string> y= GetInfo("B");            
            Debug.WriteLine($"  ➥ AwaitSimple()--End!  ThreadId:{Thread.CurrentThread.ManagedThreadId}");            
        }
        static async Task<string> GetInfo(string id)
        {
            Debug.WriteLine($"➳ GetInfo()--Start!  ThreadId:{Thread.CurrentThread.ManagedThreadId}");
            string rlt = await AsyncGetInfo(id);
            Debug.WriteLine($"  ➥ GetInfo()--End!  ThreadId:{Thread.CurrentThread.ManagedThreadId}");
            return rlt;
        }
        static Task<string> AsyncGetInfo(string id)
        {
            Debug.WriteLine($"➳ AsyncGetInfo()--Start!  ThreadId:{Thread.CurrentThread.ManagedThreadId}");
            var task = new Task<string>(
                () => {
                    Debug.WriteLine($"  ➥ Task_{id}  threadId:{Thread.CurrentThread.ManagedThreadId}");
                    return ("OK " + id);
                }
                );
            task.Start(); // invoke another thread ➠
            Debug.WriteLine($"➳ AsyncGetInfo()--End!  ThreadId:{Thread.CurrentThread.ManagedThreadId}");
            return task;
        }

        static int tid = 0;
        [UMethod]
        async public void T_ThreadTask()
        {// TODO: Add Testing logic here

            Action act = () => new MyJob1(tid++).OnRun();
            await act.RunParallel(5);

            //Action<string> act2 = async (s) => { await Task.Run(() => "Hello"); };
            //Func<Task<string>> func2 = async () => await Task<string>.Run(() =>{return "Hello"; });            
            string rlt = await Task<string>.Run(() => "Hello");//.ConfigureAwait(true);似乎在.Net core Form無用
            print(rlt);
           
        }
        //**************************************
        static Task<string> AsyncEcho(string str)
        {
            var task = new Task<string>(
                    () => {
                        Debug.WriteLine($"  ➥ Task_{str}  threadId:{Thread.CurrentThread.ManagedThreadId}");
                        return ($"Echo{str}");
                    }
                );
            task.Start(); // invoke another thread ➠
            return task;
        }
        [UMethod]
        public async void T_AwaitEcho()
        {
            Debug.WriteLine($"  UI ➽ threadId:{Thread.CurrentThread.ManagedThreadId}");
            
            string rlt = await AsyncEcho("HELLO");
            print(rlt);

            Debug.WriteLine($"  ➥ rlt_{rlt}  threadId:{Thread.CurrentThread.ManagedThreadId}");
        }
        [UMethod]
        public void T_TaskWaitEcho()
        {
            Debug.WriteLine($"  UI ➽ threadId:{Thread.CurrentThread.ManagedThreadId}");

            Task<string> task = AsyncEcho("HELLO");            
            string rlt = task.Result;
            print(rlt);

            Debug.WriteLine($"  ➥ rlt_{rlt}  threadId:{Thread.CurrentThread.ManagedThreadId}");
        }
        //**************************************
        private static readonly SmartThreadPool threadPool = new SmartThreadPool(3);
        [UMethod]
        public void T_ThreadPool()
        {// TODO: Add Testing logic here
            var task1=threadPool.CreateTask<MyJob1>();
            task1.TaskContent.Id = 100;
            task1.Controller = new TaskControl(false, new Timecard(TimeSpan.FromSeconds(3)));

            var task2 = threadPool.CreateTask<MyJob2>();
            task2.CompleteEvent += OnTask2CompleteEvent;

            threadPool.PushTask(task1);
            threadPool.PushTask(task2);
        }

        private void OnTask2CompleteEvent(object sender, object taskContent)
        {
            MyJob2 job2 = taskContent as MyJob2;
            Debug.WriteLine($"OnTask2CompleteEvent {job2.RunAt}");
        }
    }
    public static class CommClassExt
    {
        public static async Task RunParallel(this Action act, int amount)
        {
            do
            {
                await Task.Run( () => act());
            } while (amount-- > 1);           
        }
    }

    public class MyJob1 : ISmartTaskContent
    {
        public MyJob1()
        {
        }
        public int Id { get; set; }
        public MyJob1(int id)
        {
            Id = id;
        }
       
        public void OnRun()
        {
            Debug.WriteLine($"Run TaskId={Id}");
        }
    }
    public class MyJob2 : ISmartTaskContent
    {
        public MyJob2()
        {
        }
        public DateTime RunAt { get; set; }
        public void OnRun()
        {
            RunAt = DateTime.Now;
            Debug.WriteLine($"Run MyTask2={RunAt}");
        }
    }
}
