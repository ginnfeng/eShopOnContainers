////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/4/2020 11:41:01 AM 
// Description: PubProxy.cs  
// Revisions  : RabbitMQ Publisher           		
// https://www.rabbitmq.com/dotnet-api-guide.html
// **************************************************************************** 
using Common.Contract;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Support.Net.Proxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.RabbitMQ
{
    public class QuCleintProxy<TService>: QuBase,IServiceProxy<TService>
        where TService:class
    {
        public QuCleintProxy()
            :base(null)
        {
            realProxy.InvokeMethodEvent += RealProxyInvokeMethodEvent;            
        }
        private object RealProxyInvokeMethodEvent(MethodInfo methodInfo, ref object[] args)
        {
            KeyValuePair<string, string> queuePair;
            if(!queueOfMethodMap.TryGetValue(methodInfo,out queuePair))
            {
                QuRegulation<TService>.TryTakeCustomDefinedQuetePair(methodInfo, out queuePair);
                queueOfMethodMap[methodInfo] = queuePair;
            }
            var msg = CreateQueueMessage(methodInfo,args);
            var targetQueue = queuePair.Key;
            var replyQueue = queuePair.Value;
            IQuResult quRlt=null;
            IBasicProperties props = null;
            bool noReturn = methodInfo.ReturnType.Equals(typeof(void));            
            if(noReturn)
            {
                props = Channel.CreateBasicProperties();
                props.ReplyTo = replyQueue;
                props.CorrelationId = msg.Id;
                quRlt=RegistQuReslut(methodInfo, msg.Id, replyQueue);
            }
            Channel.QueueDeclare(targetQueue, false, false, false, null);
            var message = QuRegulation.Transfer.ToText(msg);//JsonConvert.SerializeObject(msg);
            var body = Encoding.UTF8.GetBytes(message);
            Channel.BasicPublish("", targetQueue, props, body);
            if (quRlt != null)
            {
                StartListeningReslutQueue(replyQueue);
                return quRlt;
            }
            return (noReturn) ? null : Activator.CreateInstance(methodInfo.ReturnParameter.ParameterType);
        }
        public TService Svc
        {
            get { return realProxy.Entity; }
        }
        private IQuResult RegistQuReslut(MethodInfo methodInfo, string correlationId,string replyQueue)
        {
            bool noReturn = !methodInfo.ReturnType.IsAssignableFrom(typeof(IQuResult));
            if (noReturn) return null;
            IQuResult quRlt = Activator.CreateInstance(methodInfo.ReturnType) as IQuResult;
            quRlt.CorrleationId = correlationId;            
            quResultMap ??= new Dictionary<string, IQuResult>();
            lock (this)
                quResultMap[quRlt.CorrleationId] = quRlt;            
            var props = Channel.CreateBasicProperties();
            props.ReplyTo = replyQueue;
            props.CorrelationId = correlationId;
            return quRlt;
        }
        private void StartListeningReslutQueue(string replyQueue)
        {
            if (!isListeningReslutQueue)
            {
                Channel.QueueDeclare(replyQueue, false, true, false, null);
                var consumer = new EventingBasicConsumer(Channel);
                consumer.Received += ResultReceived;
                Channel.BasicConsume(replyQueue, true, consumer);
                isListeningReslutQueue = true;
            }
        }

        private void ResultReceived(object sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            var message = Encoding.UTF8.GetString(e.Body);
            var msg = QuRegulation.Transfer.ToObject<QuMsg>(message);
        }

        static private QuMsg CreateQueueMessage(MethodInfo methodInfo, object[] args)
        {
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));
            return new QuMsg(args)
            {
                Id = Guid.NewGuid().ToString(),              
                MethodName = methodInfo.Name                
            };
        }

        
        private RealProxy<TService> realProxy = new RealProxy<TService>();
        private Dictionary<MethodInfo, KeyValuePair<string, string>> queueOfMethodMap = new Dictionary<MethodInfo, KeyValuePair<string, string>>();
        private Dictionary<string, IQuResult> quResultMap;
        private bool isListeningReslutQueue;
    }
}
