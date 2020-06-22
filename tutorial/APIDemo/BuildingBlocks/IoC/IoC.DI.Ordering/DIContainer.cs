using Common.Contract;
using EventBus.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Service.Banking.Contract.Service;
using Service.Ordering.ApiImp;
using Service.Ordering.Contract.Servic;
using Service.Ordering.Contract.Service;
using System;

namespace IoC.DI.Ordering
{
    public static class DIContainer
    {
        public static void ResgisterServices(IServiceCollection services)
        {
            IoC.DI.DIContainer.ResgisterServices(services);        
            services.AddTransient<IOrderingService, OrderingService>();
            services.AddTransient<IQuServiceProxy<IPaymentCallbackService>, QuCleintProxy<IPaymentCallbackService>>();
            services.AddTransient<IServiceProxy<IPaymentService>, QuCleintProxy<IPaymentService>>();
        }
    }
}

