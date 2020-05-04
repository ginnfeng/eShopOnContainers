////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 12/16/2011 11:19:54 AM 
// Description: ApiFactory.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using Support.Net.Util;

namespace Common.DataCore
{
    public class ApiFactory<TIActionSet>
    {
        static public TIActionSet Create(params object[] bindingActions)
        {
            return Create(null, bindingActions);
        }
        static private object CreateAction(Type type, object[] bindingActions)
        {
            object action=null;
            if (bindingActions != null)
            {
                foreach (var act in bindingActions)
                {                    
                    if (act != null && (type.IsAssignableFrom(act.GetType())))
                    {
                        action = act;
                    }
                }
            }
            return (action == null && type.HasDefaultConstructor()) ? Activator.CreateInstance(type) : action;

        }
        static public TIActionSet Create(IPolicyAccess policyAccess, params object[] bindingActions)
        {
            return Create(policyAccess, null, bindingActions);            
        }

        static public TIActionSet Create(IPolicyAccess policyAccess,Action<string,object> method, params object[] bindingActions)
        {
            Type apiType = typeof(TIActionSet);

            var dicProxy = new DictionaryProxy<object>();

            foreach (var propertyInfo in apiType.GetPropertyInfosOfInterfaces())
            {
                var action = CreateAction(propertyInfo.PropertyType, bindingActions);
                //Support.Log.DailyLogger.Instance.Write("**** TIActionSet Create {0}  {1}", propertyInfo.Name, (action == null) ? "NULL" : action.GetType().Name);
                dicProxy[propertyInfo.Name] = action;
                if (method != null)  method(propertyInfo.Name,action);
                if (policyAccess == null) continue;
                var domainAccess = action as IDomainAction;
                if (domainAccess != null)
                    domainAccess.PolicyAccess = policyAccess;
            }
            return dicProxy.GenEntityProxy<TIActionSet>();
        }
    }
}
