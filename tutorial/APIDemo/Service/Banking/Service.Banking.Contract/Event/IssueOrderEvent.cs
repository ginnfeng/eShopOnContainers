////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 4/17/2020 10:54:40 AM 
// Description: IssueOrderCmd.cs  
// Revisions  :            		
// **************************************************************************** 
using EventBus.Domain;
using Sid.Bss.Ordering;

namespace Service.Banking.Contract.Event
{
    public class IssueOrderEvent:EventBase<Order>
    {       
    }
}
