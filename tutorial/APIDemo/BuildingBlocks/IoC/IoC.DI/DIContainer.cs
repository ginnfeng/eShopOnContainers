using EventBus.Domain;
using EventBus.RabbitMQ;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Service.Banking.Application.EventHandler;

using Service.Banking.Contract.Event;
using Service.Ordering.ApiImp;
using Service.Ordering.Application.CommandHandler;
using Service.Ordering.Contract.Command;

using Service.Ordering.Contract.Service;
using System;
using System.IO;
using Sid.Bss.Ordering;
using ApiGw.ClientProxy;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Common.Contract;
using RabbitMQ.Client;

namespace IoC.DI
{
    public static class DIContainer
    {
        public static void ResgisterServices(IServiceCollection services, IConfiguration cfg)
        {
            //Domain Bus
            //services.AddMediatR(typeof(IMediator));
            //services.AddSingleton<IEventBus, RabbitMQBus>(sp => new RabbitMQBus(sp)); 
            services.TryAdd(ServiceDescriptor.Transient(typeof(IClientProxy<>), typeof(ClientProxy<>)));
            services.TryAdd(ServiceDescriptor.Transient(typeof(IQuProxy<>), typeof(QuProxy<>)));
            services.AddTransient<QuListener>();

            if (cfg == null) return;
            string qHost = cfg.GetValue<string>("EventBusConnection");
            services.AddSingleton<IConnectionFactory>(sp => QuBase.TakeDefaultIConnectionFactory(qHost) );
            //others
            services.AddLogging(builder => builder.AddConsole().AddDebug().AddFilter(level => level >= LogLevel.Debug));
           

        }
        public static void TakeServicDefinitions()
        {
        }
    }
}