using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Support;
using System;

namespace IoC.DI.Client
{
    public class DIContainer : DIContainerBase
    {
        static public DIContainer Instance => Singleton<DIContainer>.Instance;
        protected override void DoResgisterServices(IServiceCollection services, IConfiguration cfg)
        {
            //base.ResgisterQuService(services, cfg);
            base.ResgisterApiService(services, cfg);
        }

    }
}
