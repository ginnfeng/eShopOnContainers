////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 3/30/2020 5:15:43 PM 
// Description: OrderingService.cs  
// Revisions  :            		
// **************************************************************************** 

using EventBus.Domain;
using Service.Ordering.Contract.Command;
using Service.Ordering.Contract.Service;
using Sid.Bss.Ordering;

namespace Service.Ordering.ApiImp
{
    public class OrderingService : IOrderingService
    {
        private readonly IEventBus bus;
        public OrderingService(IEventBus bus)
        {
            this.bus = bus;
        }
        public void IssueOrder(Order order)
        {
            bus.SendCmd(new IssueOrderCmd(order));
        }

        public Order QueryOrder(string orderId)
        {
            var task= bus.SendCmd(new QueryOrderCmd(orderId));            
            return task.Result;
        }
    }
}
