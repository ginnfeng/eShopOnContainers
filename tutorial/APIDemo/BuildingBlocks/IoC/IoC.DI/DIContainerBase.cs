
using EventBus.RabbitMQ;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Microsoft.Extensions.DependencyInjection;

using ApiGw.ClientProxy;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Common.Contract;

using Support;
using System;
using Common.DataContract;
using Common.Support.Common.DataCore;

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

            services.AddLogging(builder => builder.AddConsole().AddDebug().AddFilter(level => level >= LogLevel.Debug));
            
            if (cfg == null) return;            
            var quConnString= cfg.GetValue<string>("EventBusConnection");
            if (!string.IsNullOrEmpty(quConnString))
            {
                services.AddSingleton<IConnSource<IQuSetting>>(new ConnSourceProxy<IQuSetting>(quConnString));                
            }

            var apiConnString = cfg.GetValue<string>("ApiGatewayConnection");
            if (!string.IsNullOrEmpty(apiConnString))
            {
                services.AddSingleton<IConnSource<IApiSetting>>(new ConnSourceProxy<IApiSetting>(quConnString));
            }

            DoResgisterServices(services,cfg);
        }
        
        abstract protected void DoResgisterServices(IServiceCollection services, IConfiguration cfg);
        
    }
}