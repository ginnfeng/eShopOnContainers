using Common.DataContract;
using Common.Support.Net.Proxy;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Common.Support.Net.Container
{

    public class SmartPool<TKey, TResource> : IDisposable
        where TResource : class,IDisposable
    {
        class RcInfo
        {
            public TResource Resource{ get; set; }
            //public int OwnerThreadId { get; set; }
            public int RefCount { get; set; }
            public DateTime LastUsing { get; set; }
            public string ResourceId { get; set; }
            public TKey OriginKey { get; set; }
        }
       
        public SmartPool(Func<TKey,TResource> method,Func<TKey,string> rcKeyFunc=null,bool threadShareResource=false)
        {
            this.PoolMaxCount = 10;
            this.PoolTimeOut = TimeSpan.FromSeconds(60);//秒
            this.createMethod = method;
            this.rcKeyFunc = rcKeyFunc;
            this.threadShareResource = threadShareResource;
            timer = new Timer(OnTimerElapsed, null, this.PoolTimeOut, this.PoolTimeOut);            
        }
        private Func<TKey, string> rcKeyFunc;
        void OnTimerElapsed(object state)
        {           
            lock (this)
            {
                foreach (var rcs in poolMap)
                {
                    if (rcs.Value.Count == 0) continue;
                    var rc=rcs.Value.Peek();
                    if ((DateTime.Now - rc.LastUsing) < PoolTimeOut)
                        return;
                    ReleaseOneResource(rcs.Value);                    
                }
            }            
        }
        public DisposableAdapter<TResource> Create(TKey oringalKey,bool forceNew=false)
        {
            RcInfo info;
            string usingId = (forceNew) ? Guid.NewGuid().ToString() : TakeUsingId(oringalKey);
                            
            lock (this)
            {
                if (!workingMap.TryGetValue(usingId, out info))
                {
                    info = TakeOneResource(oringalKey);
                    workingMap[usingId] = info;
                }
                info.RefCount += 1;
            }
            
            var adapter=new DisposableAdapter<TResource>(info.Resource);
            adapter.UsingId = usingId;
            adapter.ResourceId = info.ResourceId;
            adapter.DisposeEvent += OnDisposeEvent;
            adapter.RenewEntityEvent += OnRenewEntityEvent;
            return adapter;                       
        }

        void OnRenewEntityEvent(IDisposableAdapter<TResource> it)
        {
            RcInfo info;
            lock (this)
            {
                if (!workingMap.TryGetValue(it.ResourceId, out info))
                {
                    throw new KeyNotFoundException("SmartPool.OnDisposeEvent()");
                }
                it.Entity.Dispose();
                info = TakeOneResource(info.OriginKey);
                workingMap[it.ResourceId] = info;
                it.AssignEntity(info.Resource);
            }
        }

        void OnDisposeEvent(IDisposableAdapter<TResource> it)
        {
            lock (this)
            {
                RcInfo info;
                if (!workingMap.TryGetValue(it.UsingId, out info)) return;
                if (it.IsDamaged)
                {
                    try
                    {   //強制移除
                        workingMap.Remove(it.UsingId);
                        info.Resource.Dispose();
                    }
                    catch 
                    {}                   
                    return;
                }
                info.RefCount -= 1;
                if(info.RefCount<1)
                {
                    workingMap.Remove(it.UsingId);
                    Queue<RcInfo> rcs;
                    if (!poolMap.TryGetValue(it.ResourceId, out rcs))
                    {
                        rcs = new Queue<RcInfo>();
                        poolMap[it.ResourceId] = rcs;
                    }
                    if (rcs.Count >= this.PoolMaxCount)
                        ReleaseOneResource(rcs);
                    rcs.Enqueue(new RcInfo() { ResourceId = it.ResourceId,LastUsing=DateTime.Now,Resource=it.Entity,RefCount=0});
                }
            }
        }
        public bool TakeAnyOneFromPool { get; set; }
        private RcInfo TakeOneResource(TKey oringalKey)
        {
            RcInfo rc = null;
            string rcId = this.TakeResourceId(oringalKey);
            Queue<RcInfo> rcs;
            lock (this)
            {      
                if (poolMap.TryGetValue(rcId, out rcs))
                {

                    if (rcs.Count > 0)
                    {
                        rc = rcs.Dequeue();
                        //Debug.WriteLine("Reuse Resource {0},Count={1}!", rcId, rcs.Count);
                    }
                }


                if (rc == null)
                {
                    //Debug.WriteLine("{0}: Create New Resource {1}!", GetType(),rcId);
                    rc = new RcInfo() { ResourceId = rcId, Resource = createMethod(oringalKey), RefCount = 0, OriginKey = oringalKey };
                }                
            }
            return rc;
        }
        private void ReleaseOneResource(Queue<RcInfo> rcs)
        {
            RcInfo rc=rcs.Dequeue();           
            rc.Resource.Dispose();
            //Debug.WriteLine("Dispose Resource {0},Count={1}!", rc.ResourceId, rcs.Count);
        }
        private string TakeResourceId(TKey oringalKey)
        {
            var connSrc=oringalKey as IConnSource;
            return (connSrc!=null)? connSrc.ConnString:oringalKey.ToString();
        }
        private string TakeUsingId(TKey oringalKey)
        {
            var key = (rcKeyFunc != null) ? rcKeyFunc(oringalKey) : TakeResourceId(oringalKey);
            return (threadShareResource) ? key : $"{key}:{Thread.CurrentThread.ManagedThreadId}";
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
                    lock (this)
                    {
                        foreach (var rcInfo in workingMap.Values)
                        {
                            rcInfo.Resource.Dispose();
                        }
                        foreach (var que in poolMap.Values)
                        {
                            foreach (var rcInfo in que)
                            {
                                rcInfo.Resource.Dispose();
                            }
                        }
                        workingMap.Clear();
                        poolMap.Clear();
                    }
                    
                    
                }
            }
            //resource = null;
            disposed = true;
        }
        private bool disposed; 
        private Dictionary<string, RcInfo> workingMap = new Dictionary<string, RcInfo>();
        private Dictionary<string, Queue<RcInfo>> poolMap = new Dictionary<string, Queue<RcInfo>>();
        private Func<TKey, TResource> createMethod;
        private bool threadShareResource;
        private Timer timer;
        public int PoolMaxCount { get; set; }
        /// <summary>
        /// 秒
        /// </summary>
        public TimeSpan PoolTimeOut { get; set; }

    }
}
