////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 4/17/2020 1:24:38 PM 
// Description: QueryOrderCmdHandler.cs  
// Revisions  :            		
// **************************************************************************** 
using EventBus.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Service.Ordering.Contract.Command;
using Sid.Bss.Ordering;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Ordering.Application.CommandHandler
{
    public class QueryOrderCmdHandler : CmdHandlerBase, IRequestHandler<QueryOrderCmd, Order>
    {
        public QueryOrderCmdHandler(IEventBus bus, ILoggerFactory loggerFactory)
            : base(bus, loggerFactory)
        {
        }
        public Task<Order> Handle(QueryOrderCmd request, CancellationToken cancellationToken)
        {
            string orderId = request.DataContract;
            TheLogger.LogInformation($"IssueOrderCmdHandler OrderId={orderId}");
            var order = new Order() {OrderId= orderId,CustomerId="R3234566777", ShipAddress="台北市信義路四段11號"};
            return Task.FromResult(order);
        }
    }
}
