﻿////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/24/2020 2:17:08 PM 
// Description: QuConn.cs  
// Revisions  :            		
// **************************************************************************** 
using RabbitMQ.Client;
using Support.Net.Container;
using Support.Net.Proxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.RabbitMQ
{
    public class QuConn : IDisposable
    {
        internal QuConn(IConnection conn)
        {
            Init(conn);
        }

        private void Init(IConnection conn)
        {
            this.conn = conn ?? throw new NullReferenceException(nameof(Init));
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
