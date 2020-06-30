////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/29/2020 11:14:20 AM 
// Description: IQuSource.cs  
// Revisions  :            		
// **************************************************************************** 
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.RabbitMQ
{
    public interface IQuSource
    {
        string HostName { get; set; }
        bool AutomaticRecoveryEnabled { get; set; }
        bool DispatchConsumersAsync { get; set; }

        //
        // Summary:
        //     Maximum channel number to ask for.
        ushort RequestedChannelMax { get; set; }
        //
        // Summary:
        //     Sets or gets the AMQP Uri to be used for connections.
        //Uri Uri { get; set; }
        //
        // Summary:
        //     Virtual host to access during this connection.
        string VirtualHost { get; set; }
        //
        // Summary:
        //     Username to use when authenticating to the server.
        string UserName { get; set; }
        //
        // Summary:
        //     When set to true, background threads will be used for I/O and heartbeats.
        bool UseBackgroundThreadsForIO { get; set; }
        //
        // Summary:
        //     Heartbeat setting to request (in seconds).
        ushort RequestedHeartbeat { get; set; }
        //
        // Summary:
        //     Frame-max parameter to ask for (in bytes).
        uint RequestedFrameMax { get; set; }
        //
        // Summary:
        //     Amount of time protocol operations (e.g.
        //     queue.declare
        //     ) are allowed to take before timing out.
        TimeSpan ContinuationTimeout { get; set; }
        //
        // Summary:
        //     Password to use when authenticating to the server.
        string Password { get; set; }
        //
        // Summary:
        //     Dictionary of client properties to be sent to the server.
        //IDictionary<string, object> ClientProperties { get; set; }
        //
        // Summary:
        //     Amount of time protocol handshake operations are allowed to take before timing
        //     out.
        TimeSpan HandshakeContinuationTimeout { get; set; }

        IConnectionFactory ConnFactory { get;}        
    }
}
