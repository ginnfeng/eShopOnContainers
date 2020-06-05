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
    public class MQCleintProxy<TService>: MQBase,IServiceProxy<TService>
        where TService:class
    {
        public MQCleintProxy()
        {
            realProxy.InvokeMethodEvent += RealProxyInvokeMethodEvent;
            
        }
        private object RealProxyInvokeMethodEvent(MethodInfo methodInfo, ref object[] args)
        {
            KeyValuePair<string, string> queuePair;
            if(!queueOfMethodMap.TryGetValue(methodInfo,out queuePair))
            {
                ImpRegulation<TService>.TryTakeCustomDefinedQuetePair(methodInfo, out queuePair);
                queueOfMethodMap[methodInfo] = queuePair;
            }
            var msg = CreateQueueMessage(methodInfo,args);
            var targetQueue = queuePair.Key;
            var replyQueue = queuePair.Value;            
            
            IBasicProperties props = null;
            bool noReturn = methodInfo.ReturnType.Equals(typeof(void));
            if (!noReturn)
            {
                props = Channel.CreateBasicProperties(); 
                props.ReplyTo = replyQueue; 
                props.CorrelationId=msg.Id;
            }                
            Channel.QueueDeclare(targetQueue, false, false, false, null);
            var message = ImpRegulation<TService>.Transfer.ToText(msg);//JsonConvert.SerializeObject(msg);
            var body = Encoding.UTF8.GetBytes(message);
            Channel.BasicPublish("", targetQueue, props, body);                               
            return (noReturn) ? null : Activator.CreateInstance(methodInfo.ReturnParameter.ParameterType);                
            
            
        }
        public TService Svc
        {
            get { return realProxy.Entity; }
        }
       
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

        
        private RealProxy<TService> realProxy = new RealProxy<TService>();
        Dictionary<MethodInfo, KeyValuePair<string, string>> queueOfMethodMap = new Dictionary<MethodInfo, KeyValuePair<string, string>>();
    }
}
