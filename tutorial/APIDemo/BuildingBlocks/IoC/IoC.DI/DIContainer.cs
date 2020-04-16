using EventBus.Domain;
using EventBus.RabbitMQ;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Service.Ordering.ApiImp;
using Service.Ordering.Contract.Service;
using System;

public static class DIContainer
{
    public static void ResgisterServices(IServiceCollection services)
    {
        //Domain Bus
        services.AddMediatR(typeof(IMediator));
        services.AddTransient<IEventBus, RabbitMQBus>(sp=>new RabbitMQBus(sp));

        //Service
        services.AddTransient<IOrderingService, OrderingService>();
   
    }
}