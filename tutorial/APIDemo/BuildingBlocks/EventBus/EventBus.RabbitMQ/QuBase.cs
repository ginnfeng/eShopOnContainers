﻿////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/5/2020 2:41:37 PM 
// Description: MQBase.cs  
// Revisions  :            		
// **************************************************************************** 
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.RabbitMQ
{
    public class QuBase:IDisposable
    {
        public QuBase()
            :this(null)
        {
        }
        public QuBase(IServiceProvider serviceProvider)
        {
            ConnFactory = new ConnectionFactory() { HostName = "localhost", DispatchConsumersAsync = true };//暫時   

            if (serviceProvider == null) return;
            TheServiceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            if (loggerFactory != null)
                TheLogger = loggerFactory.CreateLogger<QuServiceHandler>();
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
                if (disposing && Conn != null)
                {
                    //TODO: Add resource.Dispose() logic here                    
                    if (Channel != null) Channel.Dispose();
                    Conn.Dispose();
                }
            }
            //resource = null;
            disposed = true;
            
        }
        
        protected ConnectionFactory ConnFactory { get; private set; }
        protected IServiceScopeFactory TheServiceScopeFactory { get; }
        protected ILogger TheLogger { get; }
        protected IConnection Conn 
        { 
            get {
                conn ??= ConnFactory.CreateConnection();
                return conn;
            } 
        }
        protected IModel Channel
        {
            get
            {
                channel ??= Conn.CreateModel();
                return channel;
            }
        }
        private bool disposed;
        private IConnection conn;
        private IModel channel;
        
    }
}