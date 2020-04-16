////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 3/30/2020 5:15:43 PM 
// Description: OrderingService.cs  
// Revisions  :            		
// **************************************************************************** 

using EventBus.Domain;
using Service.Ordering.Contract.Entity;
using Service.Ordering.Contract.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
            throw new NotImplementedException();
        }

        public Order QueryOrder(string orderId)
        {
            return new Order() { OrderId="919783"};
        }
    }
}
