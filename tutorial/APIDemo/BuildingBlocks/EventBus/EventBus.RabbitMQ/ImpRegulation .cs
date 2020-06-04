////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/4/2020 2:46:28 PM 
// Description: MQBase.cs  
// Revisions  :            		
// **************************************************************************** 
using Support.Serializer;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.RabbitMQ
{
    internal static class ImpRegulation<TService>
        where TService:class
    {
        static ImpRegulation()
        {
            TargetQueue = typeof(TService).FullName;
            ResponseQueue = $"@{typeof(TService).FullName}";
            Transfer = new JsonNetTransfer();
        }
        public static string TargetQueue { get; private set; }
        public static string ResponseQueue { get; private set; }
        public static BaseTransfer Transfer { get; set; }
    }
}
