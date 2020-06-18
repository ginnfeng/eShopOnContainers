////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 3/30/2020 4:42:56 PM 
// Description: Order.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sid.Bss.Ordering
{
    public class Order: IDataContract
    {
        public string OrderId { get; set; }
        public string CustomerId { get; set; }        
        public string ShipAddress { get; set; }
        public string Status { get; set; }
        public OrderDetail Detail { get; set; }
    }
}
