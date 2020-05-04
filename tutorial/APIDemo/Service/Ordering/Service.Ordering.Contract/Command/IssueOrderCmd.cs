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

namespace Service.Ordering.Contract.Command
{
    public class IssueOrderCmd:CmdBase<Order>
    {
        public IssueOrderCmd(Order order)
            :base(order)
        {

        }
    }
}
