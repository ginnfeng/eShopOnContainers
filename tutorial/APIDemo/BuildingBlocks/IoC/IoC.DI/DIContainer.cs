using EventBus.Domain;
using EventBus.RabbitMQ;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Service.Banking.Application.EventHandler;

using Service.Banking.Contract.Event;
using Service.Ordering.ApiImp;
using Service.Ordering.Application.CommandHandler;
using Service.Ordering.Contract.Command;

using Service.Ordering.Contract.Service;
using System;
using System.IO;
using Sid.Bss.Ordering;

namespace IoC.DI
{
    public static class DIContainer
    {
        public static void ResgisterServices(IServiceCollection services)
        {
            //Domain Bus
            services.AddMediatR(typeof(IMediator));
            services.AddSingleton<IEventBus, RabbitMQBus>(sp => new RabbitMQBus(sp));

            //Service
            services.AddTransient<IOrderingService, OrderingService>();

            //Event
            services.AddTransient<IEventHandler<IssueOrderEvent>, IssueOrderEventHandler>();

            //Command
            services.AddTransient<IRequestHandler<IssueOrderCmd, bool>, IssueOrderCmdHandler>();
            services.AddTransient<IRequestHandler<QueryOrderCmd, Order>, QueryOrderCmdHandler>();

            //others
            services.AddLogging(builder => builder.AddConsole().AddDebug().AddFilter(level => level >= LogLevel.Debug));
            services.AddSingleton<IConfiguration>(
                   sp =>
                   {
                       var basePath = Directory.GetCurrentDirectory();
                       var builder = new ConfigurationBuilder()
                           .SetFileProvider(new PhysicalFileProvider(basePath))
                           //.AddEnvironmentVariables()
                           .AddJsonFile("appsettings.json")
                           .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                               optional: true);
                       return builder.Build();
                   }
                   );

        }
        public static void TakeServicDefinitions()
        {
        }
    }
}