////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 4/17/2020 10:42:40 AM 
// Description: IssueOrderCommandHandler.cs  
// Revisions  :            		
// **************************************************************************** 
using EventBus.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Service.Banking.Contract.Event;
using Service.Ordering.Contract.Command;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Ordering.Application.CommandHandler
{
    public class IssueOrderCmdHandler : CmdHandlerBase,IRequestHandler<IssueOrderCmd, bool>
    {
        public IssueOrderCmdHandler(IEventBus bus, ILoggerFactory loggerFactory)
            :base(bus,loggerFactory)
        {
        }
        
        public Task<bool> Handle(IssueOrderCmd request, CancellationToken cancellationToken)
        {
            TheLogger.LogInformation($"IssueOrderCmdHandler OrderId={request.DataContract.Id}");
            var issueOrderEvent = new IssueOrderEvent() { DataContract = request.DataContract };
            TheEventBus.PublishEvent(issueOrderEvent);
            return Task.FromResult(true);
        }
    }
}
