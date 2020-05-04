////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 3/30/2020 4:52:12 PM 
// Description: OrderingService.cs  
// Revisions  :            		
// **************************************************************************** 
using Service.Ordering.Contract.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Ordering.Contract.Service
{
    public interface IOrderingService
    {
        void IssueOrder(Order order );        
        Order QueryOrder(string orderId);
    }
}
