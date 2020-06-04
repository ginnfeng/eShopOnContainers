////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/4/2020 11:43:11 AM 
// Description: SubHanlder.cs  
// Revisions  : RabbitMQ Subscriber        		
// **************************************************************************** 
//using Google.Apis.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.RabbitMQ
{
    public class MQServiceHandler
    {
        public MQServiceHandler()
        {
            ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            if (loggerFactory != null)
                logger = loggerFactory.CreateLogger<MQServiceHandler>();
            connFactory = new ConnectionFactory() { HostName = "localhost" };//暫時
        }
        public MQServiceHandler(IServiceProvider serviceProvider)
            :this()
        {
            this.serviceProvider = serviceProvider;
            this.serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();            
        }
        public void Subscribe<TService>()
           where TService : class
        {
            Subscribe<TService>(ImpRegulation<TService>.TargetQueue);
        }
        public void Subscribe<TService>(string targetQueue,TService[] svcimps)
            where TService : class
        {
            Subscribe<TService>(targetQueue, ProcessEvent);
        }
        public void Subscribe<TService>(string targetQueue)
            where TService : class            
        {
            Subscribe<TService>(targetQueue, ProcessEvent);
        }
        public void Subscribe<TService>(string targetQueue, Func<string, string,Task> processEvent)
            where TService : class
        {
            var conn = connFactory.CreateConnection();
            var channel = conn.CreateModel();
            channel.QueueDeclare(targetQueue, false, false, false, null);
            var consumer = new AsyncEventingBasicConsumer(channel);
            //consumer.Received += ConsumerReceived;
            consumer.Received += async (sender, e) =>
            {
                var eventName = e.RoutingKey;
                var message = Encoding.UTF8.GetString(e.Body);
                try
                {
                    await processEvent(eventName, message).ConfigureAwait(false);
                }
                catch (Exception err)
                {
                    if (logger != null)
                        logger.LogError(err.Message);
                    throw;
                }
            };
            channel.BasicConsume(targetQueue, true, consumer);
        }


        private async Task ProcessEvent(string eventName, string message)
        {
            //if (handlers.ContainsKey(eventName))
            //{
            //    using (var scope = serviceScopeFactory.CreateScope())
            //    {
            //        var subscriptions = handlers[eventName];
            //        foreach (var subscription in subscriptions)
            //        {
            //            var eventType = eventTypes.SingleOrDefault(t => t.Name == eventName);
            //            var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
            //            var handler = scope.ServiceProvider.GetService(handlerType);
            //            if (handler == null) continue;
            //            var eventObj = JsonConvert.DeserializeObject(message, eventType);
            //            await ((Task)handlerType.GetMethod("Handle").Invoke(handler, new object[] { eventObj })).ConfigureAwait(false);
            //        }
            //    }
            //}

        }
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;
        private ConnectionFactory connFactory;
    }
}
