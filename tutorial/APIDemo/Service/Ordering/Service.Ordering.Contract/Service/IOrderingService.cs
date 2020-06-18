////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 3/30/2020 4:52:12 PM 
// Description: OrderingService.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using Sid.Bss.Ordering;

namespace Service.Ordering.Contract.Service
{
    [ApiSpec(typeof(IOrderingService), RouteTemplate.API_VER_SVC)]
    public interface IOrderingService
    {
        void IssueOrder(Order order );        
        Order QueryOrder(string orderId);
    }
}
