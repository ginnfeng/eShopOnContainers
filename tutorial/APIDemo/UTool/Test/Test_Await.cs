////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 3/31/2020 10:49:20 AM 
// Description: Test_Await.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UTDll;
namespace UTool.Test
{
    class Test_Await : UTest
    {
        public Test_Await()
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
    }
}
