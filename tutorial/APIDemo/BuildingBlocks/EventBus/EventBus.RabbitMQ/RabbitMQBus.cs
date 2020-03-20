using EventBus.Domain;
using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// https://github.com/tharuka95/MicroRabbit
/// https://docs.microsoft.com/zh-tw/archive/msdn-magazine/2016/june/essential-net-dependency-injection-with-net-core
/// https://ithelp.ithome.com.tw/articles/10195923
/// </summary>
namespace EventBus.RabbitMQ
{
    
    public sealed class RabbitMQBus : IEventBus
    {
        private readonly IMediator mediator;
        private readonly Dictionary<string, List<Type>> handlers;
        private readonly List<Type> eventTypes;
        private readonly IServiceScopeFactory serviceScopeFactory;
        public RabbitMQBus()
        {
           
        }
        public RabbitMQBus(IMediator mediator, IServiceScopeFactory serviceScopeFactory)
        {
            
            this.mediator = mediator;
            this.handlers = new Dictionary<string, List<Type>>();
            this.eventTypes = new List<Type>();
            this.serviceScopeFactory = serviceScopeFactory;
        }
        public Task SendCommand<T>(T command) where T : Command
        {
            return mediator.Send(command);
        }

        public void Publish<T>(T @event) where T : Event
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var conn = factory.CreateConnection())
            using (var channel = conn.CreateModel())
            {
                var enentName = @event.GetType().Name;
                channel.QueueDeclare(enentName, false, false, false, null);
                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("", enentName, null, body);

            }
        }

        public void Subscribe<T, TH>()
            where T : Event
            where TH : IEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var handlerType = typeof(TH);
            if (!eventTypes.Contains(typeof(T)))
                eventTypes.Add(typeof(T));
            if (!handlers.ContainsKey(eventName))
                handlers[eventName] = new List<Type>();
            if (handlers[eventName].Any(it => it.GetType() == handlerType))
                throw new ArgumentException($"Handler Type {handlerType.Name} already is registered fo '{eventName}", nameof(handlerType));
            handlers[eventName].Add(handlerType);
            StartBasicConsume<T>();
        }

        private void StartBasicConsume<T>() where T : Event
        {
            var factory = new ConnectionFactory() { HostName = "localhost", DispatchConsumersAsync = true };
            var conn = factory.CreateConnection();
            var channel = conn.CreateModel();
            var eventName = typeof(T).Name;
            channel.QueueDeclare(eventName, false, false, false, null);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;
            channel.BasicConsume(eventName, true, consumer);
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            var message = Encoding.UTF8.GetString(e.Body);
            try
            {
                await ProcessEvent(eventName, message).ConfigureAwait(false);
            }
            catch (Exception err)
            {

                throw err;
            }
        }
        private async Task ProcessEvent(string eventName, string message)
        {
            if (handlers.ContainsKey(eventName))
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var subscriptions = handlers[eventName];
                    foreach (var subscription in subscriptions)
                    {
                        var handler = scope.ServiceProvider.GetService(subscription);
                        if (handler == null) continue;
                        var eventType = eventTypes.SingleOrDefault(t => t.Name == eventName);
                        var @event = JsonConvert.DeserializeObject(message, eventType);
                        var conreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                        await (Task)conreteType.GetMethod("Handle").Invoke(handler, new object[] { @event });
                    }
                }
            }

        }
        /*
        private async Task ProcessEvent(string eventName, string message)
        {
            var subscriptions = handlers[eventName];
            foreach (var subscription in subscriptions)
            {
                var handler = Activator.CreateInstance(subscription);
                if (handler == null) continue;
                var eventType = eventTypes.SingleOrDefault(t => t.Name == eventName);
                var @event = JsonConvert.DeserializeObject(message, eventType);
                var conreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                await (Task)conreteType.GetMethod("Handle").Invoke(handler,new object[] { @event});
            }
            
        }*/
    }
}