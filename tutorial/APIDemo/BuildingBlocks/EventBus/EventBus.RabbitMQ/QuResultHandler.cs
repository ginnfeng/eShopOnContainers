////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/10/2020 5:10:59 PM 
// Description: QuResultHandler.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace EventBus.RabbitMQ
{
    
    internal class QuResultHandler : IQuCorrleation,IDisposable
    {
        public QuResultHandler(string cid)
        {            
            CorrleationId = cid;
        }        
        private object Rlt { get; set; }
        public string CorrleationId { get; set; }

        public void StartWatch()
        {
            returnEvent = new AutoResetEvent(false);
            returnEvent.Reset();
        }
        public void AssignResult(object rlt)
        {
            Rlt = rlt;
            returnEvent.Set();
        }
        
        public object Wait(TimeSpan timeOut)
        {            
            bool isTrue = returnEvent.WaitOne(timeOut);
            if (!isTrue)
                throw new TimeoutException(nameof(Wait));
            return Rlt;
        }
        
        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass of this type implements a finalizer.
            System.GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //TODO: Add resource.Dispose() logic here
                    returnEvent.Dispose();
                    returnEvent = null;
                }
            }
            //resource = null;
            disposed = true;
        }
        private bool disposed;
        private AutoResetEvent returnEvent;


    }
}
