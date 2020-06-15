////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/5/2020 2:41:37 PM 
// Description: MQBase.cs  
// Revisions  :            		
// **************************************************************************** 
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Support.Net.Proxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.RabbitMQ
{
    public class QuBase:IDisposable
    {
        public QuBase(string host, IServiceProvider serviceProvider)
            :this((ConnectionFactory)null, serviceProvider)
        {
            Conn=QuConnPool.Instance.Create(host);            
        }
        public QuBase(ConnectionFactory connFactory, IServiceProvider serviceProvider)           
        {
            if(connFactory!=null)
                Conn = QuConnPool.Instance.Create(connFactory);            
            if (serviceProvider == null) return;
            TheServiceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            if (loggerFactory != null)
                TheLogger = loggerFactory.CreateLogger<QuListener>();
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
