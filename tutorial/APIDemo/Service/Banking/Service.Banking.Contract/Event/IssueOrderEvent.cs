////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 4/17/2020 10:54:40 AM 
// Description: IssueOrderCmd.cs  
// Revisions  :            		
// **************************************************************************** 
using EventBus.Domain;
using Service.Ordering.Contract.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Banking.Contract.Event
{
    public class IssueOrderEvent:EventBase<Order>
    {       
    }
}
