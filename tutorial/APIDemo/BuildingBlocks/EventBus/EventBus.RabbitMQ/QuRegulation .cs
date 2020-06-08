////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/4/2020 2:46:28 PM 
// Description: MQBase.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using Support.Net.Util;
using Support.Serializer;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EventBus.RabbitMQ
{
    internal static class QuRegulation
    {
        static QuRegulation()
        {
            Transfer = new JsonNetTransfer();
        }
        public static BaseTransfer Transfer { get; set; }        
    }
    internal static class QuRegulation<TService>
        where TService:class
    {
        static QuRegulation()
        {
            defaultQuetePair = new KeyValuePair<string, string>(typeof(TService).FullName, $"@{typeof(TService).FullName}");            
        }
        public static List<KeyValuePair<string, string>> TakeQuetePairs()
        {
            var queueNames = new List<KeyValuePair<string, string>>();
            queueNames.Add(defaultQuetePair);
            var methodInfos=typeof(TService).GetMethods();
            foreach (var methodInfo in methodInfos)
            {
                KeyValuePair<string, string> qs;
                if(TryTakeCustomDefinedQuetePair(methodInfo,out qs))
                    queueNames.Add(qs);
            }
            return queueNames;
        }
        public static bool TryTakeCustomDefinedQuetePair(MethodInfo methodInfo,out KeyValuePair<string, string> qpair)
        {
            var specAttri=methodInfo.GetCustomAttribute<ApiSpecAttribute>();
            if (specAttri == null || !specAttri.AsQueueName)
            {
                qpair= defaultQuetePair;
                return false;
            }
            qpair = new KeyValuePair<string, string>($"{defaultQuetePair.Key}:{specAttri.Template}", $"{defaultQuetePair.Value}");
            return true;
        }
        private static KeyValuePair<string, string> defaultQuetePair { get; set; }
       
    }
}
