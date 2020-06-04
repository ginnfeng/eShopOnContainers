////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/4/2020 1:34:23 PM 
// Description: SvcMsg.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EventBus.RabbitMQ
{
    internal class Msg
    {
        public Msg(object[] parameters)
        {
            Params = new ReadOnlyCollection<object>(parameters);
        }
        public string Id { get; set; }
        public string MethodName  { get; set; }
        public ReadOnlyCollection<object> Params { get; private set; }
    }
}
