////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/16/2020 3:14:18 PM 
// Description: PaymentDetail.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Banking.Contract.Entity
{
    public class PaymentDetail : IDataContract
    {
        public string Id { get; set; }
        public decimal TaxRate{ get; set; }
        public decimal Amount { get; set; }
    }
}
