﻿////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/5/2020 2:41:37 PM 
// Description: MQBase.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Support.Net.Proxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.RabbitMQ
{
    public class QuBase
    {
        public QuBase(string host, ILoggerFactory loggerFactory)
            :this(TakeDefaultIConnectionFactory(host), loggerFactory)
        {
        }
        public QuBase(IConnectionFactory connFactory, ILoggerFactory loggerFactory)           
        {
            if(connFactory!=null)
                Conn = QuConnPool.Instance.Create(connFactory); 
            if (loggerFactory != null)
                TheLogger = loggerFactory.CreateLogger<QuListener>();
        }
        static public IConnectionFactory TakeDefaultIConnectionFactory(string host)
        {
            return new ConnectionFactory
            {
                HostName = host,
                DispatchConsumersAsync = true,
                AutomaticRecoveryEnabled = true
            };
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
                    Conn?.Dispose();
                }
            }
            //resource = null;
            disposed = true;            
        }
        
        
        protected IServiceScopeFactory TheServiceScopeFactory { get; }
        protected ILogger TheLogger { get; }
        protected DisposableAdapter<QuConn> Conn { get; }
        private bool disposed;        
    }
}
