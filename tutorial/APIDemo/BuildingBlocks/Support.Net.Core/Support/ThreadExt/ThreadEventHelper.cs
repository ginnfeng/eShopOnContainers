////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 10/13/2011 10:38:39 AM 
// Description: ThreadEventHelper.cs  
// Revisions  :            		
// **************************************************************************** 
using System.Threading;
using System.Security.AccessControl;

namespace Support.ThreadExt
{
    static public class ThreadEventHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="initState">
        /// false: 啟動時自動reset statue, EventResetMode.ManualReset: Set()後能進入WaitOne()其後會被自動reset 
        /// true: 啟動時若為已有reset statue則不變 ,否則init to set status  
        /// </param>        
        /// <returns></returns>
        static public EventWaitHandle CreateManualWaitHandle(string id, bool initState)
        {
            return CreateManualWaitHandle(id,initState,true);
        }
        static public EventWaitHandle CreateManualWaitHandle(string id, bool initState, bool allowEveryone)
        {
            return CreateWaitHandle(id, initState, EventResetMode.ManualReset, true);
        }
        static public EventWaitHandle CreateAutoWaitHandle(string id, bool initState)
        {
            return CreateAutoWaitHandle(id, initState, true);
        }
        static public EventWaitHandle CreateAutoWaitHandle(string id, bool initState, bool allowEveryone)
        {
            return CreateWaitHandle(id, initState, EventResetMode.AutoReset, true);
        }
        static public EventWaitHandle CreateWaitHandle(string id, bool initState,EventResetMode resetMode, bool allowEveryone)
        {
            
            //EventResetMode.AutoReset: Set()後,不管有無進入WaitOne(),馬上被自動reset
            EventWaitHandle eventWaitHandle;
            bool createNew;
            if (allowEveryone)
            {   //避免跨Process因權限不足產生WinIOError (Access to the path 'id' is denied.)
                // create a rule that allows anybody in the "Users" group to synchronise with us
                //var users = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);                

                EventWaitHandleSecurity evhSec = new EventWaitHandleSecurity();
                EventWaitHandleRights rights = EventWaitHandleRights.FullControl;// EventWaitHandleRights.Synchronize | EventWaitHandleRights.Modify;
                EventWaitHandleAccessRule evhAccessRule = new EventWaitHandleAccessRule("Everyone", rights, AccessControlType.Allow);
                evhSec.AddAccessRule(evhAccessRule);
                eventWaitHandle = new EventWaitHandle(initState, resetMode, id,out createNew);
                //eventWaitHandle.SetAccessControl(evhSec);
            }
            else
            {
                eventWaitHandle = new EventWaitHandle(initState, resetMode, id, out createNew);
            }           
            
            return eventWaitHandle;
        }
    }
}
