using Common.Contract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service.Banking.ApiImp;
using Service.Banking.Contract.Service;
using System;

namespace IoC.DI.Banking
{
    public static class DIContainer
    {
        public static void ResgisterServices(IServiceCollection services,IConfiguration cfg)
        {
            IoC.DI.DIContainer.ResgisterServices(services,cfg);
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<IDepositService, PaymentService>();
        }
    }
}
