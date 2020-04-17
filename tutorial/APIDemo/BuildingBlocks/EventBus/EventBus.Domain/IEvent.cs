////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 4/17/2020 11:51:29 AM 
// Description: IEvent.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Domain
{
    public interface IEvent
    {
        public DateTime Timestamp { get; }
    }
}
