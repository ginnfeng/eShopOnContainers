using Common.Contract;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Service.Ordering.ApiImp;
using Service.Ordering.Contract.Servic;
using Service.Ordering.Contract.Service;
using Common.Support;
using System;

namespace IoC.DI.Ordering
{
   
    public class DIContainer : DIContainerBase
    {
        static public DIContainer Instance => Singleton<DIContainer>.Instance;
        protected override void DoResgisterServices(IServiceCollection services, IConfiguration cfg)
        {
            base.ResgisterQuService(services, cfg);
            //base.ResgisterApiService(services, cfg);
            services.AddTransient<IOrderingService, OrderingService>();
            services.AddTransient<IPaymentCallbackService, PaymentCallbackService>();
        }
        
    }
}

