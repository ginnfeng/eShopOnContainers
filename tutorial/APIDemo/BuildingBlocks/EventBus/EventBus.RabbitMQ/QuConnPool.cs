////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/12/2020 11:14:45 AM 
// Description: QuConnPool.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.DataContract;
using RabbitMQ.Client;
using Common.Support;
using Common.Support.Net.Container;
using Common.Support.Net.Proxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace EventBus.RabbitMQ
{
    public class QuConnPool:IDisposable
    {
        static public QuConnPool Instance=> Singleton<QuConnPool>.Instance;
        
        public QuConnPool()
        {
            Func<IConnSource, QuConn> method = (connSource) => { return new QuConn(connSource); };
            connPool = new SmartPool<IConnSource,QuConn>(method,threadShareResource:true);
            connPool.PoolMaxCount = 1;
        }       
        
        public DisposableAdapter<QuConn> Create(IConnSource connSrc)
        {
            //var connFactory=src.TakeCache<ConnectionFactory>();            
            return connPool.Create(connSrc);
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
                    connPool?.Dispose();
                }
            }
            //resource = null;
            disposed = true;
        }
        private bool disposed;
        private SmartPool<IConnSource,QuConn> connPool;
        //private Dictionary<string, IConnectionFactory> defaultIConnectionFactoryMap;
    }
    

    
}
