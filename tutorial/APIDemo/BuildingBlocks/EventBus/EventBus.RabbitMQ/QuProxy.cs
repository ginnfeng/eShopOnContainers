////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/4/2020 11:41:01 AM 
// Description: PubProxy.cs  
// Revisions  : RabbitMQ Publisher           		
// https://www.rabbitmq.com/dotnet-api-guide.html
// **************************************************************************** 
using Common.Contract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class QuProxy<TService>: QuBase, IQuProxy<TService>
        where TService:class
    {
        [ActivatorUtilitiesConstructor]//Default Constructor for DI
        public QuProxy(IConnectionFactory connFactory, ILoggerFactory loggerFactory)
            : base(connFactory, loggerFactory)
        {
            realProxy.InvokeMethodEvent += RealProxyInvokeMethodEvent;
            ResultListener = new QuListener(connFactory, loggerFactory);
        }
        public QuProxy(string host, ILoggerFactory loggerFactory = null)
            : base(host, loggerFactory)
        {
            realProxy.InvokeMethodEvent += RealProxyInvokeMethodEvent;
            ResultListener = new QuListener(host, loggerFactory);
        }
        
        private TimeSpan defaultWaitTimeout = new TimeSpan(0, 0, 120);
        public T WaitResult<T>(QuResult<T> rltStamp)
        {
            return WaitResult(rltStamp, defaultWaitTimeout);
        }
        public T WaitResult<T>(QuResult<T> rltStamp, TimeSpan timeOut)
        {
            return responseService.Wait<T>(rltStamp, timeOut);
        }
        public async  Task<T> AsyncWaitResult<T>(QuResult<T> rltStamp)
        {
            return await AsyncWaitResult(rltStamp, defaultWaitTimeout).ConfigureAwait(false);
        }
        public async Task<T> AsyncWaitResult<T>(QuResult<T> rltStamp, TimeSpan timeOut)
        {
            return await Task<T>.Run(() => responseService.Wait<T>(rltStamp, timeOut)).ConfigureAwait(false);
        }
        private object RealProxyInvokeMethodEvent(MethodInfo methodInfo, ref object[] args)
        {
            using (var channel = base.Conn.Entity.Create())
                return DispatchMethod(channel.Entity,methodInfo, ref args);

        }
        private object DispatchMethod(IModel channel,MethodInfo methodInfo, ref object[] args)
        {
            QuSpecAttribute queueSpec;
            if (!queueOfMethodMap.TryGetValue(methodInfo, out queueSpec))
            {
                queueSpec = QuRegulation<TService>.TakeQueueSpec(methodInfo);
                queueOfMethodMap[methodInfo] = queueSpec;
            }
            var msg = CreateQueueMessage(methodInfo, args);
            var targetQueue = string.IsNullOrEmpty(msg.TargetQueue) ? queueSpec.Queue : msg.TargetQueue;
            var replyQueue = string.IsNullOrEmpty(msg.ReplyQueue) ? queueSpec.ReplyQueue : msg.ReplyQueue;
            IQuCorrleation quRlt = null;
            IBasicProperties props = null;
            bool immediatelyWaitRasult = false;
            bool noReturn = methodInfo.ReturnType.Equals(typeof(void));
            if (!noReturn)
            {
                quRlt = RegistQuReslut(methodInfo, queueSpec, msg, out immediatelyWaitRasult);
                if (quRlt != null)
                {
                    props = channel.CreateBasicProperties();
                    props.ReplyTo = queueSpec.ReplyQueue;
                    props.CorrelationId = quRlt.CorrleationId;
                }
            }
            channel.QueueDeclare(targetQueue, queueSpec.Durable, queueSpec.Exclusive, queueSpec.AutoDelete, null);
            var message = QuRegulation.Transfer.ToText(msg);//JsonConvert.SerializeObject(msg);
            var body = Encoding.UTF8.GetBytes(message);

            if (quRlt != null)
            {
                responseService.Register(quRlt);
                StartListeningReslutQueue(replyQueue);
            }
            channel.BasicPublish("", targetQueue, props, body);
            if (noReturn || quRlt == null) return null;
            if (immediatelyWaitRasult)
            {
                var obj = responseService.Wait(methodInfo.ReturnParameter.ParameterType, quRlt, defaultWaitTimeout);
                return obj;
            }
            return quRlt;
            //return Activator.CreateInstance(methodInfo.ReturnParameter.ParameterType);
        }
        public TService Svc
        {
            get { return realProxy.Entity; }
        }
        private IQuCorrleation RegistQuReslut(MethodInfo methodInfo, QuSpecAttribute queueSpec,QuMsg msg,out bool immediatelyWaitRasult)
        {
            immediatelyWaitRasult = !typeof(IQuCorrleation).IsAssignableFrom(methodInfo.ReturnType);
            //if (immediatelyWaitRasult) return null;
            //var type=typeof(QuResult<>).MakeGenericType(methodInfo.ReturnType.GetGenericArguments());
            IQuCorrleation quRlt =(immediatelyWaitRasult)?new QuResult(): Activator.CreateInstance(methodInfo.ReturnType) as IQuCorrleation;
            quRlt.CorrleationId = string.IsNullOrEmpty(msg.CorrleationId)?msg.Id:msg.CorrleationId;            
            //quResultMap ??= new Dictionary<string, IQuCorrleation>();
            //lock (this)
            //    quResultMap[quRlt.CorrleationId] = quRlt;
            return quRlt;
        }
        private void StartListeningReslutQueue(string replyQueue)
        {
            var quSpec = QuRegulation<IQuResponseService>.TakeQueueSpec();
            quSpec.Queue = replyQueue;
            ResultListener.Subscribe<IQuResponseService>(responseService, quSpec);
            //if (!isListeningReslutQueue)
            //{
            //    Channel.QueueDeclare(replyQueue, false, true, false, null);
            //    var consumer = new AsyncEventingBasicConsumer(Channel);
            //    consumer.Received += ResultReceived;
            //    Channel.BasicConsume(replyQueue, true, consumer);
            //    isListeningReslutQueue = true;
            //}
        }

        async private Task ResultReceived(object sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            var message = Encoding.UTF8.GetString(e.Body);
            await Task.Run(()=>QuRegulation.Transfer.ToObject<QuMsg>(message)).ConfigureAwait(false);
        }

        static private QuMsg CreateQueueMessage(MethodInfo methodInfo, object[] args)
        {
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));
            if (args != null && args.Length == 1  && typeof(QuMsg).IsAssignableFrom(args[0].GetType()))
                return args[0] as QuMsg; //通常是response MSG
            return new QuMsg(args)
            {
                Id = Guid.NewGuid().ToString(),              
                MethodName = methodInfo.Name                
            };
        }
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //TODO: Add resource.Dispose() logic here 
                    ResultListener?.Dispose();
                    base.Dispose(disposing);
                }
            }
            //resource = null;
            disposed = true;

        }
        private bool disposed;
        private QuListener ResultListener { get; }
        private RealProxy<TService> realProxy = new RealProxy<TService>();
        private Dictionary<MethodInfo, QuSpecAttribute> queueOfMethodMap = new Dictionary<MethodInfo, QuSpecAttribute>();
        //private Dictionary<string, IQuCorrleation> quResultMap;
        private bool isListeningReslutQueue;
        private QuResponseService responseService = QuResponseService.Instance;
    }
}
