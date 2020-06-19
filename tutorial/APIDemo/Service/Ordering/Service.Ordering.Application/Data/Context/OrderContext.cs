////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/18/2020 5:52:27 PM 
// Description: OrderContext.cs  
// Revisions  :            		
// **************************************************************************** 
using Sid.Bss.Ordering;
using Support;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Ordering.Application.Data.Context
{
    public class OrderContext
    {//暫時模擬DB data
        static public OrderContext Instance => Singleton<OrderContext>.Instance;
        
        public bool TryGetValue(string id, out Order order)
        {
            lock(this)
                return orderRepository.TryGetValue(id, out order);
        }
        public bool Insert(Order order)
        {
            lock (this)
            {
                if (orderRepository.ContainsKey(order.Id))
                    return false;
                orderRepository[order.Id] = order;
                return true;
            }
        }
        public bool Update(Order order)
        {
            lock (this)
            {
                if (!orderRepository.ContainsKey(order.Id))
                    return false;
                orderRepository[order.Id] = order;
                return true;
            }
        }
        public bool FindAndDo(Func<Order,bool> act)
        {
            foreach (var order in orderRepository.Values)
            {
                if (act(order)) return true;
            }
            return false;
        }
        private Dictionary<string, Order> orderRepository = new Dictionary<string, Order>();
    }
}
