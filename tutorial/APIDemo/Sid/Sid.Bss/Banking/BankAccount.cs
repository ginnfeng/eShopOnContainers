////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/16/2020 3:02:36 PM 
// Description: BankAccount.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sid.Bss.Banking
{
    public class BankAccount:IDataContract
    {
        public string Id { get; set; }
        public string AccountType { get; set; }
        public decimal AccountBalance { get; set; }//餘額
    }
}
