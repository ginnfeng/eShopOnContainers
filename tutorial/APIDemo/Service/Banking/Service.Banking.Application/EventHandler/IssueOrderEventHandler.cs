////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 4/17/2020 11:39:12 AM 
// Description: IssueOrderEventHandler.cs  
// Revisions  :            		
// **************************************************************************** 
using EventBus.Domain;
using Microsoft.Extensions.Logging;
using Service.Banking.Contract.Event;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Banking.Application.EventHandler
{
    public class IssueOrderEventHandler:EventHandlerBase, IEventHandler<IssueOrderEvent>
    {
        public IssueOrderEventHandler(IEventBus bus, ILoggerFactory loggerFactory)
            :base(bus, loggerFactory)
        {

        }
        public Task Handle(IssueOrderEvent theEvent)
        {
            var order = theEvent.DataContract;
            TheLogger.LogInformation($"OrderId={order.Id} ShipAddres={order.ShipAddress}");
            return Task.CompletedTask;
        }
    }
}
