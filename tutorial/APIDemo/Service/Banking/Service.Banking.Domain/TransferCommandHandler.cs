////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 3/18/2020 3:44:58 PM 
// Description: TransferCommandHandler.cs  
// Revisions  :            		
// **************************************************************************** 
using EventBus.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Banking.Domain
{
    public class TransferCommandHandler : IRequestHandler<TransferCommand, bool>
    {
        private readonly IEventBus bus;
        public TransferCommandHandler(IEventBus bus)
        {
            this.bus = bus;
        }
        public Task<bool> Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            //publish event to RabbitMQ
            //bus.Publish(new TransferCreatedEvent(request.From, request.To, request.Amount));
            return Task.FromResult(true);
        }
    }
}
