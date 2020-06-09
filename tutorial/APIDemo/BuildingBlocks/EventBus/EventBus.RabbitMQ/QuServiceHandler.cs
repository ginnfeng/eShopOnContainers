﻿////*************************Copyright © 2020 Feng 豐**************************	
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Support.Net.Util;
using System.Reflection;

namespace EventBus.RabbitMQ
{
    public class QuServiceHandler: QuBase
    {
        public QuServiceHandler()            
        {
        }
        public QuServiceHandler(IServiceProvider serviceProvider)
            :base(serviceProvider)
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
            var qPairs = QuRegulation<TService>.TakeQuetePairs();
            qPairs.ForEach(qp => Subscribe<TService>(qp.Key, qp.Value,svcs, ProcessEvent));
        }
        private void Subscribe<TService>(string targetQueue,string replyQueue, IEnumerable<TService> svcs, Func<string, string, IEnumerable<TService>, Task> processEvent)
            where TService : class
        {            
            Channel.QueueDeclare(targetQueue, false, false, false, null);            
            var consumer = new AsyncEventingBasicConsumer(Channel);
            //consumer.Received += ConsumerReceived;
            consumer.Received += async (sender, e) =>
            {
                var eventName = e.RoutingKey;
                var message = Encoding.UTF8.GetString(e.Body);
                try
                {
                    await processEvent(eventName, message, svcs).ConfigureAwait(false);
                }
                catch (Exception err)
                {
                    if (TheLogger != null)
                        TheLogger.LogError(err.Message);
                    throw;
                }
            };
            Channel.BasicConsume(targetQueue, true, consumer);
        }


        private async Task ProcessEvent<TService>(string eventName, string msgText, IEnumerable<TService> svcs)
            where TService : class
        {
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
                await Task.Run(() => methodInfo.Invoke(svc, methodparams)).ConfigureAwait(false); 
            }
           
        }
               
    }
}
