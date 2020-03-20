////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 3/18/2020 4:15:45 PM 
// Description: TransferEvent.cs  
// Revisions  :            		
// **************************************************************************** 
using EventBus.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Banking.Domain
{
    
    public class TransferEvent : Event
    {
        public TransferEvent(int from, int to, decimal amount)
        {
            From = from;
            To = to;
            Amount = amount;
        }
        public int From { get; private set; }
        public int To { get; private set; }
        public decimal Amount { get; private set; }
    }
}
