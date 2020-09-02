////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 3/31/2020 5:32:52 PM 
// Description: AsyncCallWrapper.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Support.ThreadExt
{
    public class AsyncCallWrapper<OBJ>
    {
        public AsyncCallWrapper(OBJ it)
        {
            this.it = it;
        }
        public Task<T> AsyncCall<T>(Func<OBJ, T> syncMethod)
        {
            var task = new Task<T>(
                    () => { return (syncMethod(this.it)); }
                );
            task.Start();
            return task;
        }
        public Task AsyncCall(Action<OBJ> syncMethod)
        {
            var task = new Task(
                    () => { syncMethod(this.it); }
                );
            task.Start();
            return task;
        }
        private OBJ it;
    }
}
