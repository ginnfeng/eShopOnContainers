////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 4/16/2020 5:16:02 PM 
// Description: TransferRecord.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Banking.Contract.Entity
{
    public class TransferRecord:IDataContract
    {
        public long Id { get; set; }
        public bool Succes { get; set; }
        public PaymentDetail Detail { get; set; }
        public DateTime At { get; set; }
        public string Info { get; set; }
    }
}
