////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/5/2020 2:41:37 PM 
// Description: MQBase.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using Common.DataContract;
using Common.Support.Common.DataCore;
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
        public QuBase(IConnSource src, ILoggerFactory loggerFactory)           
        {
            if(src!=null)
                Conn = QuConnPool.Instance.Create(src); 
            if (loggerFactory != null)
                TheLogger = loggerFactory.CreateLogger<QuListener>();
        }
        static public IConnSource TakeDefaultIConnectionFactory(string host)
        {
            var connStr = $"HostName={host};DispatchConsumersAsync=true;AutomaticRecoveryEnabled=true;UserName=guest;Password=guest";
            return new ConnSourceProxy(connStr);            
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
