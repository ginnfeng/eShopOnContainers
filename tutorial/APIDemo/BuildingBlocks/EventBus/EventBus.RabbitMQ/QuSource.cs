////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/29/2020 11:13:49 AM 
// Description: QuSource.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.RabbitMQ
{
    public class QuSource: IQuSource
    {
        //public QuSource(IConnSource src)
        public QuSource()
        {
            //取得default values
            ConnFactory = new ConnectionFactory();
        }
        public string HostName { get { return ((ConnectionFactory)ConnFactory).HostName; } set { ((ConnectionFactory)ConnFactory).HostName = value; } }
        public bool AutomaticRecoveryEnabled { get { return ((ConnectionFactory)ConnFactory).AutomaticRecoveryEnabled; } set { ((ConnectionFactory)ConnFactory).AutomaticRecoveryEnabled = value; } }
        public bool DispatchConsumersAsync { get { return ((ConnectionFactory)ConnFactory).DispatchConsumersAsync; } set { ((ConnectionFactory)ConnFactory).DispatchConsumersAsync = value; } }

        public ushort RequestedChannelMax { get { return ConnFactory.RequestedChannelMax; } set { ConnFactory.RequestedChannelMax = value; } }
        //public Uri Uri { get { return ConnFactory.Uri; } set { ConnFactory.Uri = value; } }
        public string VirtualHost { get { return ConnFactory.VirtualHost; } set { ConnFactory.VirtualHost = value; } }
        public string UserName { get { return ConnFactory.UserName; } set { ConnFactory.UserName = value; } }
        public bool UseBackgroundThreadsForIO { get { return ConnFactory.UseBackgroundThreadsForIO; } set { ConnFactory.UseBackgroundThreadsForIO = value; } }
        public ushort RequestedHeartbeat { get { return ConnFactory.RequestedHeartbeat; } set { ConnFactory.RequestedHeartbeat = value; } }
        public uint RequestedFrameMax { get { return ConnFactory.RequestedFrameMax; } set { ConnFactory.RequestedFrameMax = value; } }
        public TimeSpan ContinuationTimeout { get { return ConnFactory.ContinuationTimeout; } set { ConnFactory.ContinuationTimeout = value; } }
        public string Password { get { return ConnFactory.Password; } set { ConnFactory.Password = value; } }
        public TimeSpan HandshakeContinuationTimeout { get { return ConnFactory.HandshakeContinuationTimeout; } set { ConnFactory.HandshakeContinuationTimeout = value; } }

        public IConnectionFactory ConnFactory { get; }

    }
    
}
