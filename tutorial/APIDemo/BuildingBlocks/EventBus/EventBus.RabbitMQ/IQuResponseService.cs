////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/10/2020 10:25:06 AM 
// Description: IQuResponseService.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.RabbitMQ
{
    [QuSpec(Exclusive =false,AutoDelete =true)]
    internal interface IQuResponseService
    {
        void ReceiveResponse(string corrleationId,object rlt);
    }
}
