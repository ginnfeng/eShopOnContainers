////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 11/25/2009 2:43:34 PM 
// Description: ThreadEventTaskContent.cs  
// Revisions  :            		
// **************************************************************************** 

#if WINONLY


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Support.Net.WPF 
{
     public class ThreadEventTaskContent: ThreadEventTaskContent<object>
     {
     }
    public class ThreadEventTaskContent<T> : ThreadTaskContentBase
    {
        public ThreadEventTaskContent()
        {
        }
        public ThreadEventTaskContent(T owner)
        {
            this.Owner = owner;
        }
        protected override void DispathCall()
        {
            if (DispathCallEvent != null)
                DispathCallEvent(Owner);
        }
        protected override void DoRun()
        {
            if (DoRunEvent != null)
                DoRunEvent(Owner);
        }
        /// <summary>
        /// Callback to WPF UI thread(意即Fire DispathCallEvent會回到WPF UI thread中執行)
        /// </summary>
        public event Action<T> DispathCallEvent;

        /// <summary>
        /// NonUI thread(執行背景工作,執行完畢才會呼叫DispathCall() )
        /// </summary>
        public event Action<T>  DoRunEvent;
        public T Owner { get; set; }
    }
}

#endif