////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 4/17/2020 11:10:07 AM 
// Description: CmdHandlerBase.cs  
// Revisions  :            		
// **************************************************************************** 
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Domain
{
    public class CmdHandlerBase
    {  
        protected ILogger TheLogger { get; private set; }
        protected IEventBus TheEventBus { get; private set; }
        public CmdHandlerBase(IEventBus bus,ILoggerFactory loggerFactory)
        {
            TheLogger = loggerFactory.CreateLogger(this.GetType());
            TheEventBus = bus;
        }
    }
}
