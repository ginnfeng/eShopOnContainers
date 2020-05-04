////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 4/17/2020 1:11:03 PM 
// Description: QueryOrderCmd.cs  
// Revisions  :            		
// **************************************************************************** 
using EventBus.Domain;
using Service.Ordering.Contract.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Ordering.Contract.Command
{
    public class QueryOrderCmd : CmdBase<string,Order>
    {
        public QueryOrderCmd(string orderId)
            :base(orderId)
        {

        }
    }
}
