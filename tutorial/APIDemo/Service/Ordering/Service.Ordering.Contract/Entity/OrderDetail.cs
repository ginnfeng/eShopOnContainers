////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 3/30/2020 4:48:22 PM 
// Description: OrderDetail.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Ordering.Contract.Entity
{
    public class OrderDetail: IDataContract
    {
        public string ProductId  { get; set; }
        public int Quantity { get; set; }
    }
}
