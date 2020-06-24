using Common.Contract;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Service.Ordering.ApiImp;
using Service.Ordering.Contract.Servic;
using Service.Ordering.Contract.Service;
using System;

namespace IoC.DI.Ordering
{
    public static class DIContainer
    {
        public static void ResgisterServices(IServiceCollection services, IConfiguration cfg)
        {
            IoC.DI.DIContainer.ResgisterServices(services,cfg);        
            services.AddTransient<IOrderingService, OrderingService>();
            services.AddTransient<IPaymentCallbackService, PaymentCallbackService>();
        }
    }
}

