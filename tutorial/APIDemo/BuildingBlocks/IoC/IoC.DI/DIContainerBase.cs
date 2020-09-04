
using EventBus.RabbitMQ;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Microsoft.Extensions.DependencyInjection;

using ApiGw.ClientProxy;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Common.Contract;

using Common.Support;
using System;
using Common.DataContract;
using Common.Support.Common.DataCore;

namespace IoC.DI
{
    public abstract class DIContainerBase
    {
        public void ResgisterServices(IServiceCollection services, IConfiguration cfg)
        {
            //services.AddOptions();
            
            

            services.AddLogging(builder => builder.AddConsole().AddDebug().AddFilter(level => level >= LogLevel.Debug));
            DoResgisterServices(services,cfg);
        }
        protected void ResgisterQuService(IServiceCollection services, IConfiguration cfg)
        {
            services.TryAdd(ServiceDescriptor.Transient(typeof(IQuProxy<>), typeof(QuProxy<>)));
            services.AddTransient<QuListener>();
            if (cfg == null) return;
            var connString = cfg.GetValue<string>("cfg_EventBusConnection");
            //if (!string.IsNullOrEmpty(connString))
            {
                services.AddSingleton<IConnSource<IQuSetting>>(new ConnSourceProxy<IQuSetting>(connString));
            }
        }
        protected void ResgisterApiService(IServiceCollection services, IConfiguration cfg)
        {
            services.TryAdd(ServiceDescriptor.Transient(typeof(IApiProxy<>), typeof(ApiProxy<>)));
            var connString = cfg.GetValue<string>("cfg_ApiGatewayConnection");
            //if (!string.IsNullOrEmpty(connString))
            {
                services.AddSingleton<IConnSource<IApiSetting>>(new ConnSourceProxy<IApiSetting>(connString));
            }
        }


        abstract protected void DoResgisterServices(IServiceCollection services, IConfiguration cfg);
        
    }
}