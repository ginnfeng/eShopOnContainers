////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/6/2020 4:53:53 PM 
// Description: RouteBasisAttribute.cs  
// Revisions  :            		
// **************************************************************************** 
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Contract
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor)]
    public class RouteConfigAttribute : RouteAttribute
    {
        public RouteConfigAttribute(Type interfaceType)
            : this(GetRouteAttribute(interfaceType).Template)
        {
        }
        public RouteConfigAttribute(Type interfaceType, string methodName)
            : this(GetRouteAttribute(interfaceType, methodName).Template)
        {
            var attri = GetRouteAttribute(interfaceType, methodName);
            this.Name = attri.Name;
            this.Order = attri.Order;
        }
        public RouteConfigAttribute(string template)
            : base(template)
        {
        }
        static private RouteAttribute GetRouteAttribute(Type interfaceType)
        {
            var attris = interfaceType.GetCustomAttributes(typeof(RouteAttribute), true);
            return (attris.Length > 0) ? (RouteAttribute)attris.First() : new RouteAttribute("api/[controller]") { };
        }
        static private RouteAttribute GetRouteAttribute(Type interfaceType, string methodName)
        {
            var method = interfaceType.GetMethod(methodName);
            var attris = method.GetCustomAttributes(typeof(RouteAttribute), true);
            return (attris.Length > 0) ? (RouteAttribute)attris.First() : new RouteAttribute(methodName) { };
        }

    }
}
