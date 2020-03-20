using EventBus.Domain;
using EventBus.RabbitMQ;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Service.Banking.Domain;
using System;

namespace Common.IoC
{
    public class DependencyContainer
    {
        public static void RegisterServices(IServiceCollection services)
        {

            //Domain Bus
            services.AddSingleton<IEventBus, RabbitMQBus>(
                sp =>
                {
                    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                    return new RabbitMQBus(sp.GetRequiredService<IMediator>(), scopeFactory);
                }
            );
            //Subscriptions
            //services.AddTransient<TransferEventHandler>();

            //Domain Events
            //services.AddTransient<IEventHandler<Transfer.Domain.Events.TransferCreatedEvent>, TransferEventHandler>();


            //Domain Banking Commands
            services.AddTransient<IRequestHandler<TransferCommand, bool>, TransferCommandHandler>();

            //Application Services
            //services.AddTransient<IAccountService, AccountService>();
            //services.AddTransient<ITransferService, TransferService>();

            //Data
            //services.AddTransient<IAccountRepository, AccountRepository>();
            //services.AddTransient<ITransferRepository, TransferRepository>();
            //services.AddTransient<BankingDbContext>();
            //services.AddTransient<TransferDbContext>();

        }
    }
}
