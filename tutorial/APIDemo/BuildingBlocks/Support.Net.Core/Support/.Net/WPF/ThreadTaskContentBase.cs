////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 11/25/2009 11:14:54 AM 
// Description: ThreadTaskContentBase.cs  
// Revisions  :    
// Reference :  Build More Responsive Apps With The Dispatcher
//                    http://msdn.microsoft.com/en-us/magazine/cc163328.aspx        		
// **************************************************************************** 
#if WINONLY


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Support.ThreadExt;
using System.Windows.Threading;

namespace Support.Net.WPF
{
   
    public abstract class ThreadTaskContentBase : ISmartTaskContent
    {
        public ThreadTaskContentBase()
        {
            DispatchPriority = DispatcherPriority.Normal;
        }

        #region ISmartTaskContent Members

        virtual public void OnRun()
        {
            DoRun();
            Delegate delegateMethod=new Action(DispathCall);
            if (IsAsyncDispath)
            {
                dispatcher.Invoke(delegateMethod, DispatchPriority, null);
                OnDispatchCompleted(this,null);
            }
            else
            {
                var dispatchOperator=dispatcher.BeginInvoke(delegateMethod, DispatchPriority, null);
                dispatchOperator.Completed += new EventHandler(OnDispatchCompleted);                
            }
        }

        private void OnDispatchCompleted(object sender, EventArgs e)
        {
            if (DispatchCompletedEvent != null)
            {
                DispatchCompletedEvent(this, e); 
            }
        }
        #endregion

        public event EventHandler DispatchCompletedEvent;

        /// <summary>
        /// SynsDispath is default
        /// </summary>
        public bool IsAsyncDispath { get; set; }
        public DispatcherPriority DispatchPriority { get; set; }
        
        /// <summary>
        /// Callback to WPF UI thread(意即DispathCall會回到WPF UI thread中執行)
        /// </summary>
        protected abstract void DispathCall();

        /// <summary>
        /// NonUI thread(執行背景工作,執行完畢才會呼叫DispathCall() )
        /// </summary>
        protected abstract void DoRun();

        private Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
    }
}

#endif