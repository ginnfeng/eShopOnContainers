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

namespace EventBus.RabbitMQ
{
    public class QuConnPool:IDisposable
    {
        static public QuConnPool Instance=> Singleton<QuConnPool>.Instance;
        public QuConnPool()
        {
            Func<Uri, IConnection> method = (uri) => { return this.connFactory.CreateConnection(); };
            connPool = new SmartPool<IConnection, Uri>(method);
        }
        public DisposableAdapter<IConnection> Create(string host)
        {
            connFactory.HostName = host;
            var conn=connPool.Create(new Uri(host));
            Reset2DefaultConnFactory();
            return conn;
        }
        public DisposableAdapter<IConnection> Create(ConnectionFactory connectionFactory=null)
        {
            connectionFactory ??= defaultConnFactory;
            connFactory = connectionFactory;
            var conn=connPool.Create(new Uri(connFactory.HostName));
            Reset2DefaultConnFactory();
            return conn;
        }
        private void Reset2DefaultConnFactory()
        {
            connFactory = defaultConnFactory;
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
                if (disposing && connPool!=null)
                {
                    //TODO: Add resource.Dispose() logic here
                    connPool.Dispose();
                }
            }
            //resource = null;
            disposed = true;
        }
        private bool disposed;
        private SmartPool<IConnection, Uri> connPool;
        private ConnectionFactory connFactory;
        static private readonly ConnectionFactory defaultConnFactory = new ConnectionFactory
        {
            HostName="localhost",
            DispatchConsumersAsync = true,
            AutomaticRecoveryEnabled = true
        };
    }

    public class QuChannelPool 
    {
        static QuChannelPool()
        {
            
//            Func<Uri, IModel> method = (uri) => { return conn.CreateModel(); };
//#pragma warning disable CA2000 // Dispose objects before losing scope            
//            Singleton<SmartPool<IModel, Uri>>.Create(() => new SmartPool<IModel, Uri>(method));
//#pragma warning restore CA2000 // Dispose objects before losing scope
        }
        public QuChannelPool(IConnection conn)
        {
            
        }
        private SmartPool<IModel, Uri> channelPool = Singleton<SmartPool<IModel, Uri>>.Instance;
    }
    public class QuChannel : IDisposable
    {
        public QuChannel(IConnection conn)
        {
            Func<Uri, IModel> method = (uri) => { return conn.CreateModel(); };
#pragma warning disable CA2000 // Dispose objects before losing scope            
            Singleton<SmartPool<IModel, Uri>>.Create(() => new SmartPool<IModel, Uri>(method));
#pragma warning restore CA2000 // Dispose objects before losing scope
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
                }
            }
            //resource = null;
            disposed = true;            
        }
        private bool disposed;
        private DisposableAdapter<IModel> channelDisposableAdapter;
        
    }
}
