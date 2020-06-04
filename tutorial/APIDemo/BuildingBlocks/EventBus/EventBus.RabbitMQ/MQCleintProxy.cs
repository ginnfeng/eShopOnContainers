////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/4/2020 11:41:01 AM 
// Description: PubProxy.cs  
// Revisions  : RabbitMQ Publisher           		
// https://www.rabbitmq.com/dotnet-api-guide.html
// **************************************************************************** 
using Common.Contract;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Support.Net.Proxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EventBus.RabbitMQ
{
    public class MQCleintProxy<TService>:IServiceProxy<TService>
        where TService:class
    {
        public MQCleintProxy()
        {
            realProxy.InvokeMethodEvent += RealProxyInvokeMethodEvent;
            connFactory = new ConnectionFactory() { HostName = "localhost" };//暫時
            TargetQueue = ImpRegulation<TService>.TargetQueue;
            ReplyQueue = ImpRegulation<TService>.ResponseQueue;
        }
        private object RealProxyInvokeMethodEvent(MethodInfo methodInfo, ref object[] args)
        {
            var msg = CreateQueueMessage(methodInfo,args);
            using (var conn = connFactory.CreateConnection())
            using (var channel = conn.CreateModel())
            {
                IBasicProperties props = null;
                bool noReturn = methodInfo.ReturnType.Equals(typeof(void));
                if (!noReturn)
                {
                    props = channel.CreateBasicProperties(); 
                    props.ReplyTo = ReplyQueue; 
                    props.CorrelationId=msg.Id;
                }                
                channel.QueueDeclare(TargetQueue, false, false, false, null);
                var message = ImpRegulation<TService>.Transfer.ToText(msg);//JsonConvert.SerializeObject(msg);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("", TargetQueue, props, body);
                               
                return (noReturn) ? null : Activator.CreateInstance(methodInfo.ReturnParameter.ParameterType);                
            }
            
        }
        public TService Svc
        {
            get { return realProxy.Entity; }
        }
        public string TargetQueue { get; set; }
        public string ReplyQueue { get; set; }
        static private Msg CreateQueueMessage(MethodInfo methodInfo, object[] args)
        {
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));
            return new Msg(args)
            {
                Id = Guid.NewGuid().ToString(),              
                MethodName = methodInfo.Name                
            };
        }
        private ConnectionFactory connFactory;
        private RealProxy<TService> realProxy = new RealProxy<TService>();
    }
}
