////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/12/2020 11:14:45 AM 
// Description: QuConnPool.cs  
// Revisions  :            		
// **************************************************************************** 
using RabbitMQ.Client;
using Support;
using Support.Net.Container;
using Support.Net.Proxy;
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
            Func<IConnectionFactory, QuConn> method = (connFactory) => { return new QuConn(connFactory.CreateConnection()); };
            connPool = new SmartPool<QuConn,IConnectionFactory>(method,(connfactory)=>$"{connfactory.GetType().Name}{connfactory.GetHashCode()}");
            connPool.PoolMaxCount = 1;
        }       
        
        public DisposableAdapter<QuConn> Create(IConnectionFactory connectionFactory)
        {
            return connPool.Create(connectionFactory);
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
        private SmartPool<QuConn, IConnectionFactory> connPool;
        //private Dictionary<string, IConnectionFactory> defaultIConnectionFactoryMap;
    }
    

    
}
