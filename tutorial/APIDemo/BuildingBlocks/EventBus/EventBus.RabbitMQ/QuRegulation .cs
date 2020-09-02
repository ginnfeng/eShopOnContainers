////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/4/2020 2:46:28 PM 
// Description: MQBase.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using Common.Open.Serializer;
using Common.Support.Net.Util;
using Common.Support.Serializer;
using System;
using System.Collections.Generic;
using System.Reflection;


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
        public static List<QuSpecAttribute> TakeAllQueueSpec()
        {
            Type type = typeof(TService);            
            var specs = new List<QuSpecAttribute>() { QuSpecAttribute.TakeSpec(type) };
            
            var methodInfos = type.GetAllIncludBaseType<MethodInfo>(t => t.GetMethods());
            foreach (var methodInfo in methodInfos)
            {
                var methodSpec=QuSpecAttribute.TakeSpec(type,methodInfo);
                if (specs.Find(it => it.Queue==methodSpec.Queue) == null)
                    specs.Add(methodSpec);
               
            }
            return specs;
        }
        public static QuSpecAttribute TakeQueueSpec(MethodInfo methodInfo)
        {
            return QuSpecAttribute.TakeSpec(typeof(TService), methodInfo);            
        }
        public static QuSpecAttribute TakeQueueSpec()
        {
            return QuSpecAttribute.TakeSpec(typeof(TService));
        }
        private static KeyValuePair<string, string> defaultQuetePair { get; set; }
       
    }
}
