using Microsoft.Extensions.DependencyInjection;
using System;

namespace IoC.DI.Banking
{
    public static class DIContainer
    {
        public static void ResgisterServices(IServiceCollection services)
        {
            IoC.DI.DIContainer.ResgisterServices(services);
        }
    }
}
