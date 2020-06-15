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
            Func<ConnectionFactory, QuConn> method = (connFactory) => { return new QuConn(connFactory.CreateConnection()); };
            connPool = new SmartPool<QuConn,ConnectionFactory>(method,(connfactory)=>$"{connfactory.GetType().Name}{connfactory.GetHashCode()}");
            connPool.PoolMaxCount = 1;
        }
       
        public DisposableAdapter<QuConn> Create(string host)
        {
            var connFactory = TakeDefaultConnectionFactory(host);
            return connPool.Create(connFactory);
        }
        public DisposableAdapter<QuConn> Create(ConnectionFactory connectionFactory)
        {
            return connPool.Create(connectionFactory);
        }
        
        public ConnectionFactory TakeDefaultConnectionFactory(string host)
        {
            defaultConnectionFactoryMap ??= new Dictionary<string, ConnectionFactory>();
            ConnectionFactory connFactory;
            if (!defaultConnectionFactoryMap.TryGetValue(host,out connFactory))
            {
                connFactory = new ConnectionFactory
                {
                    HostName = host,
                    DispatchConsumersAsync = true,
                    AutomaticRecoveryEnabled = true
                };
                defaultConnectionFactoryMap[host] = connFactory;
            }
            return connFactory;
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
        private SmartPool<QuConn, ConnectionFactory> connPool;
        private Dictionary<string, ConnectionFactory> defaultConnectionFactoryMap;
    }
    public  class QuConn : IDisposable
    {       
        internal QuConn(IConnection conn)
        {
            Init(conn);
        }
       
        private void Init(IConnection conn)
        {
            this.conn = conn;
            if (conn == null)
                throw new NullReferenceException(nameof(Init));
            ChannelPool = new SmartPool<IModel, string>(rckey => conn.CreateModel());
            ChannelPool.PoolMaxCount = 1;
        }
        public DisposableAdapter<IModel> Create()
        {
            return ChannelPool.Create(typeof(IModel).Name);
        }
        public SmartPool<IModel, string> ChannelPool { get; private set; }
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
                    ChannelPool?.Dispose();
                    conn?.Dispose();
                }
            }
            //resource = null;
            disposed = true;
        }
        private bool disposed;        
        private IConnection conn;
    }

    
}
