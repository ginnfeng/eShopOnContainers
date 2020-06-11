////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/10/2020 10:03:01 AM 
// Description: QuSpecAttribute.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Common.Contract
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor)]
    public class QuSpecAttribute : Attribute
    {
        public QuSpecAttribute()
        {

        }
        public QuSpecAttribute(string queueName)
        {
            Queue = queueName;
        }
        public string Queue { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public bool Durable{ get; set; }
        public string ReplyQueue { get; private set;}
        static public QuSpecAttribute TakeSpec(Type type,MethodInfo methodInfo)
        {
            var typeSpec = TakeSpec(type);
            var methodSpec = methodInfo.GetCustomAttribute<QuSpecAttribute>();
            if (methodSpec != null) {
                if (string.IsNullOrEmpty(methodSpec.Queue))
                    methodSpec.Queue = $"{typeSpec.Queue}:{methodInfo.Name}";
            }
            else 
                methodSpec ??= typeSpec;            
            methodSpec.ReplyQueue = typeSpec.ReplyQueue;//便免Client端啟動過多listener
            return methodSpec;
        }
        static public QuSpecAttribute TakeSpec(Type type)
        {
            
            var spec=type.GetCustomAttribute<QuSpecAttribute>();
            return (spec != null) ? spec : new QuSpecAttribute(type.Name) { ReplyQueue=$"#{type.Name}@{Environment.MachineName}"};
        }
    }
}
