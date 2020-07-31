using Common.Contract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service.Banking.ApiImp;
using Service.Banking.Contract.Service;
using Support;
using System;

namespace IoC.DI.Banking
{
    public class DIContainer: DIContainerBase
    {
        static public DIContainer Instance => Singleton<DIContainer>.Instance;
        protected override void DoResgisterServices(IServiceCollection services, IConfiguration cfg)
        {
            base.ResgisterQuService(services,cfg);
            //base.ResgisterApiService(services, cfg);
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<IDepositService, PaymentService>();
        }
        
    }
}
