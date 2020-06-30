
using EventBus.RabbitMQ;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Microsoft.Extensions.DependencyInjection;

using ApiGw.ClientProxy;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Common.Contract;

using Support;
using System;

namespace IoC.DI
{
    public abstract class DIContainerBase
    {
        public void ResgisterServices(IServiceCollection services, IConfiguration cfg)
        {
            //Domain Bus
            //services.AddMediatR(typeof(IMediator));
            //services.AddSingleton<IEventBus, RabbitMQBus>(sp => new RabbitMQBus(sp)); 
            services.TryAdd(ServiceDescriptor.Transient(typeof(IApiProxy<>), typeof(ApiProxy<>)));
            services.TryAdd(ServiceDescriptor.Transient(typeof(IQuProxy<>), typeof(QuProxy<>)));
            services.AddTransient<QuListener>();

            if (cfg == null) return;
            string qHost = cfg.GetValue<string>("EventBusConnection");
            services.AddSingleton<IQuSource>(sp => QuBase.TakeDefaultIConnectionFactory(qHost) );
            //others
            services.AddLogging(builder => builder.AddConsole().AddDebug().AddFilter(level => level >= LogLevel.Debug));

            DoResgisterServices(services,cfg);
        }
        
        abstract protected void DoResgisterServices(IServiceCollection services, IConfiguration cfg);
        
    }
}