using EventBus.Domain;
using EventBus.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace System.IoC
{
    public static class DIContainer
    {
        public static void ResgisterServices(IServiceCollection services)
        {
            //Domain Bus
            services.AddTransient<IEventBus, RabbitMQBus>();
        }
    }
}
