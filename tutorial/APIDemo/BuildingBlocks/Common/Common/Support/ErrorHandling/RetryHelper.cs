////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 7/8/2020 2:59:51 PM 
// Description: RetryHelper.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Support.ErrorHandling
{
    public class RetryHelper
    {
        static public readonly TimeSpan defaultRetryTimeSpan = TimeSpan.FromSeconds(2);
        static public T AutoRetry<T>(Func<T> func, int retryCount = 1)
        {
            return AutoRetry<T>(func, defaultRetryTimeSpan, retryCount);
        }
        static public T AutoRetry<T>(Func<T> func, TimeSpan ts, int retryCount = 1)
        {
            try
            {
                return func();
            }
            catch (Exception err)
            {
                if (retryCount <= 0) throw err;
                System.Threading.Thread.Sleep(ts);
                return AutoRetry<T>(func, ts, retryCount - 1);
            }
        }
        static public void AutoRetry(Action act, int retryCount = 1)
        {
            AutoRetry(act, defaultRetryTimeSpan, retryCount);
        }
        static public void AutoRetry(Action act, TimeSpan ts, int retryCount = 1)
        {
            AutoRetry<bool>(() => { act(); return true; }, defaultRetryTimeSpan, retryCount);;
        }
    }
}
