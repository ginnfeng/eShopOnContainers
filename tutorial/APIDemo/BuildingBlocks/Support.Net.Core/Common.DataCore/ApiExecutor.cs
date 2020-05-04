using Support.Serializer;
////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 8/20/2013 5:17:38 PM 
// Description: ApiExecuter.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using Support.Net.Util;

namespace Common.DataCore
{
    public interface IApiExecutor
    {
        void Run(string cmd);
        object Exec(string cmd);
        TResult Exec<TResult>(string cmd);
        string Exec2String<TResult>(string cmd);
        string Exec2String<TResult>(string cmd, BaseTransfer transfer);
        ApiParameter Exec2ApiParameter<TResult>(string cmd);
        ApiParameter Exec2ApiParameter<TResult>(string cmd, BaseTransfer transfer);
        IApiExecutor NewOne(object[] apis=null, IPolicyAccess policyAccess = null);
        void ForeachAction(Action<object> acts);

        /// <param name="actName">field name in actset</param>
        /// <param name="act">Action object</param>
        /// <returns></returns>
        bool TryGetAction(string actName, out object act);
    }
    public class ApiExecutor<TActionSet> : IApiExecutor
    {
        
        public ApiExecutor(object[] apis=null, IPolicyAccess policyAccess = null)
        {
            API = ApiFactory<TActionSet>.Create(policyAccess, (actName, act) => actMap[actName]=act, apis);            
        }
        public TActionSet API { get;private set; }

        public IApiExecutor NewOne(object[] apis = null, IPolicyAccess policyAccess = null)
        {
            return new ApiExecutor<TActionSet>(apis, policyAccess);            
        }
        
        public void Run(string cmd)
        {
            var act = new StatementField<TActionSet>() { Content = cmd };
            act.Invoke(API);
        }
        public object Exec(string cmd)
        {
            var act = new StatementField<TActionSet>() { Content = cmd };
            return act.Invoke(API);
        }
        public TResult Exec<TResult>(string cmd)
        {
            return (TResult)Exec(cmd);            
        }
        public string Exec2String<TResult>(string cmd)
        {
            return Exec2String<TResult>(cmd, transfer);
        }
        public string Exec2String<TResult>(string cmd, BaseTransfer transfer)
        {
            TResult rlt = Exec<TResult>(cmd);
            return transfer.ToText(rlt);
        }
        public ApiParameter Exec2ApiParameter<TResult>(string cmd)
        {
            return Exec2ApiParameter<TResult>(cmd, transfer);
        }
        public ApiParameter Exec2ApiParameter<TResult>(string cmd, BaseTransfer transfer)
        {
            TResult rlt = Exec<TResult>(cmd);
            string rltStr = null;
            string rltType = typeof(TResult).FullName;
            if (rlt != null)
            {
                rltType = rlt.GetType().FullName;
                rltStr = transfer.ToText(rlt);
            }
            return new ApiParameter() { TypeName = rltType, Serializer = transfer.GetType().FullName, Value = rltStr };
        }

        public void ForeachAction(Action<object> toDo)
        {
            actMap.Values.ForEach(it=>toDo(it));            
        }
        public bool TryGetAction(string actName,out object act)
        {
            return actMap.TryGetValue(actName, out act);
                  
        }
        
        private BaseTransfer transfer = new JsonTransfer();
        private Dictionary<string, object> actMap = new Dictionary<string, object>();
    }    
}
