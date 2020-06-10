////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/10/2020 10:25:32 AM 
// Description: QuResponseService.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using Newtonsoft.Json.Linq;
using Support;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.RabbitMQ
{
    internal class QuResponseService : IQuResponseService,IDisposable
    {
        static public QuResponseService Instance => Singleton<QuResponseService>.Instance;
        
        public QuResponseService()
        {

        }
        public void ReceiveResponse(string corrleationId, object rlt)
        {
            QuResultHandler rltHlper;
            lock (this)
            {
                if (rltmap.TryGetValue(corrleationId, out rltHlper))
                {
                    rltHlper.AssignResult(rlt);
                }
            }
        }
        public void Register(IQuCorrleation rlt)
        {
            QuResultHandler rltHlper;
            lock (this)
            {
                if (!rltmap.TryGetValue(rlt.CorrleationId, out rltHlper))
                {
                    rltHlper = new QuResultHandler(rlt.CorrleationId);
                    rltmap[rlt.CorrleationId] = rltHlper;
                }
            }
            rltHlper.StartWatch();
        }
        //public T Wait<T>(QuResult<T> rltStamp)
        //{
        //    return Wait(rltStamp, TimeSpan.MaxValue);
        //}
        public T Wait<T>(QuResult<T> rltStamp, TimeSpan timeOut)
        {
            QuResultHandler rltHlper;
            lock (this)
            {
                if (!rltmap.TryGetValue(rltStamp.CorrleationId, out rltHlper))
                    throw new NullReferenceException(nameof(Wait));
            }
            var obj = rltHlper.Wait(timeOut);
            var jObj = obj as JObject;
            var rlt = (jObj!=null)? jObj.ToObject<T>():(T)obj;
            lock (this)
                rltmap.Remove(rltHlper.CorrleationId);
            rltHlper.Dispose();
            return rlt;
            
        }
        public object Wait(Type retType,IQuCorrleation rltStamp, TimeSpan timeOut)
        {
            QuResultHandler rltHlper;
            lock (this)
            {
                if (!rltmap.TryGetValue(rltStamp.CorrleationId, out rltHlper))
                    throw new NullReferenceException(nameof(Wait));
            }
            var obj = rltHlper.Wait(timeOut);
            var jObj = obj as JObject;
            var rlt = (jObj != null) ? jObj.ToObject(retType) : obj;
            lock (this)
                rltmap.Remove(rltHlper.CorrleationId);
            rltHlper.Dispose();
            return rlt;
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
                    foreach (var rltHelper in rltmap.Values)
                    {
                        rltHelper.Dispose();
                    }
                    rltmap.Clear();
                }
            }
            //resource = null;
            disposed = true;
        }
        private bool disposed;
        private Dictionary<string, QuResultHandler> rltmap=new Dictionary<string, QuResultHandler>();
    }
}
