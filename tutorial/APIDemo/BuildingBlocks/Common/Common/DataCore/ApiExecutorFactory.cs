
////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 8/22/2013 11:42:29 AM 
// Description: ApiExecutorFactory.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using Common.Support.Net.Util;

namespace Common.DataCore
{
    public class ApiExecutorFactory
    {
        public bool TryGetExecutor<TActionSet>(out IApiExecutor executor)
        {
            return TryGetExecutor(typeof(TActionSet), out executor);
        }
        public bool TryGetExecutor(Type actionSetType,out IApiExecutor executor)
        {
            return TryGetExecutor(actionSetType.FullName, out executor);
        }
        public bool TryGetExecutor(string actionSetTypeName,out IApiExecutor executor)
        {
            return apiExecutorMap.TryGetValue(actionSetTypeName, out executor);
        }
        public void RegistActionSet(Type actinoSetType, object[] acts = null, IPolicyAccess policyAccess = null)
        {
            Type genericType=typeof(ApiExecutor<>);
            object[] ctorParameters = new object[] { acts, policyAccess };
            var genericObject = GenericTypeHelper.CreateInstance(genericType, new Type[] { actinoSetType }, ctorParameters);
            apiExecutorMap[actinoSetType.FullName] = (IApiExecutor)genericObject;
        }
        private Dictionary<string, IApiExecutor> apiExecutorMap = new Dictionary<string, IApiExecutor>();
    }
}
