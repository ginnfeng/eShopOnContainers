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
    internal class QuMsg
    {
        public QuMsg(object[] parameters)
        {
            if(parameters!=null)
                Params = new List<object>(parameters);
        }
        public string Id { get; set; }
        public string MethodName  { get; set; }
        public List<object> Params { get; set; }
    }
}
