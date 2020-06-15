////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/4/2020 11:43:11 AM 
// Description: SubHanlder.cs  
// Revisions  : RabbitMQ Subscriber        		
// **************************************************************************** 
//using Google.Apis.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Support.Net.Util;
using System.Reflection;
using Common.Contract;
using Support.Net.Proxy;

namespace EventBus.RabbitMQ
{
    public class QuListener: QuBase
    {
        public QuListener(string host, IServiceProvider serviceProvider = null)
            : base(host, serviceProvider)
        {
        }
        public QuListener(ConnectionFactory connFactory, IServiceProvider serviceProvider)
            : base(connFactory, serviceProvider)
        {
            
        }
        
        public void Subscribe<TService>()
           where TService : class
        {
            using (var scope = TheServiceScopeFactory.CreateScope())
            {
                var svcs=scope.ServiceProvider.GetServices<TService>();                
                Subscribe<TService>(svcs);
            }
        }
        public void Subscribe<TService>(TService svc)
          where TService : class
        {
            Subscribe<TService>(new TService[] { svc });
        }
        public void Subscribe<TService>(IEnumerable<TService> svcs)
           where TService : class
        {
            var queueSpecs = QuRegulation<TService>.TakeAllQueueSpec();
            queueSpecs.ForEach(qpspec => Subscribe<TService>(qpspec,svcs, ProcessEvent));
        }
        /// <summary>
        /// 
        /// </summary>        
        internal void Subscribe<TService>(TService svc, QuSpecAttribute quSpec)
           where TService : class
        {
            Subscribe<TService>(quSpec, new TService[] { svc }, ProcessEvent);            
        }
        private void Subscribe<TService>(QuSpecAttribute quSpec, IEnumerable<TService> svcs, Func<string, IBasicProperties, IEnumerable<TService>, Task> processEvent)
            where TService : class
        {            
            ListeningChannel.QueueDeclare(quSpec.Queue, quSpec.Durable, quSpec.Exclusive, quSpec.AutoDelete, null);            
            var consumer = new AsyncEventingBasicConsumer(ListeningChannel);
            //consumer.Received += ConsumerReceived;
            consumer.Received += async (sender, e) =>
            {
                //e.BasicProperties.ReplyTo;
                //var eventName = e.RoutingKey;
                var msgText = Encoding.UTF8.GetString(e.Body);
                try
                {
                    await processEvent(msgText,e.BasicProperties, svcs).ConfigureAwait(false);
                }
                catch (Exception err)
                {
                    if (TheLogger != null)
                        TheLogger.LogError(err.Message);
                    throw;
                }
            };
            ListeningChannel.BasicConsume(quSpec.Queue, true, consumer);
        }


        private async Task ProcessEvent<TService>(string msgText,IBasicProperties basicProperties, IEnumerable<TService> svcs)
            where TService : class
        {
            //var eventName = orginalMsg.RoutingKey;            
            var type = typeof(TService);
            var msg = QuRegulation.Transfer.ToObject<QuMsg>(msgText);
            //type.GetInterfaces
            
            var methodInfo=type.FindIncludBaseType<MethodInfo>(t=>t.GetMethod(msg.MethodName));
            
            var paramInfos = methodInfo.GetParameters();
            var methodparams= new object[paramInfos.Length];
            for (int i = 0; i < paramInfos.Length; i++)
            {
                var paramType = paramInfos[i].ParameterType;
                var jobject = msg.Params[i] as JObject;
                methodparams[i] = (jobject==null)? msg.Params[i]: jobject.ToObject(paramType);
            }
            foreach (var svc in svcs)
            {
                //methodInfo.Invoke(svc, methodparams);
                if (string.IsNullOrEmpty(basicProperties.ReplyTo))
                {
                    await Task.Run(() => methodInfo.Invoke(svc, methodparams)).ConfigureAwait(false);
                }
                else
                {
                    object rlt= await Task<object>.Run(() => methodInfo.Invoke(svc, methodparams)).ConfigureAwait(false);
                    //IQuCorrleation quRlt = (typeof(IQuCorrleation).IsAssignableFrom(methodInfo.ReturnType))
                    //    ? rlt as IQuCorrleation
                    //    : new QuResult<object>(rlt);
                    //quRlt.CorrleationId= orginalMsg.BasicProperties.CorrelationId;
                    rlt = (typeof(QuResult).IsAssignableFrom(methodInfo.ReturnType)) ? ((QuResult)rlt).Value :rlt;
                    QuMsg repMsg = new QuMsg(new object[] { basicProperties.CorrelationId,rlt });                    
                    //repMsg.CorrleationId = orginalMsg.BasicProperties.CorrelationId;
                    repMsg.MethodName = nameof(IQuResponseService.ReceiveResponse);
                    //Channel.QueueDeclare(orginalMsg.BasicProperties.ReplyTo, queueSpec.Durable, queueSpec.Exclusive, queueSpec.AutoDelete, null);
                    var rspMessage = QuRegulation.Transfer.ToText(repMsg);//JsonConvert.SerializeObject(msg);
                    var body = Encoding.UTF8.GetBytes(rspMessage);
                    using(var channel=Conn.Entity.Create())
                        channel.Entity.BasicPublish("", basicProperties.ReplyTo, null, body);
                }
            }

        }
        protected IModel ListeningChannel
        {
            get
            {
                listeningChannel ??= Conn.Entity.Create();
                return listeningChannel.Entity;
            }        
        }
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //TODO: Add resource.Dispose() logic here 
                    listeningChannel?.Dispose();
                    base.Dispose(disposing);
                }
            }
            //resource = null;
            disposed = true;

        }
        private bool disposed;
        private DisposableAdapter<IModel> listeningChannel;

    }
}
