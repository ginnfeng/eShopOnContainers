////*************************Copyright © 2013 Feng 豐**************************	
// Created    : 6/23/2016 11:51:59 AM 
// Description: DynamicProperty.cs  
//              可用在Dynamic Object的property也是Dynamic Object
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Dynamic;

namespace Common.Support.Net.Proxy
{
    public class DynamicProperty<T> : DynamicObject
    {
        public DynamicProperty(T node)
        {
            this.node = node;
            Type type = typeof(T);
            setAction = (_node, k, v) => type.GetProperty(k).SetValue(_node, v);
            getFunction = (_node, k) => type.GetProperty(k).GetValue(_node);
        }
        public DynamicProperty(T node, Action<T, string, object> setAct, Func<T, string, object> getAct)
        {
            this.node = node;
            setAction = setAct;
            getFunction = getAct;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (value != null)
                setAction(node, binder.Name, value);
            return true;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            if (getFunction == null)
                return false;
            result = getFunction(node, binder.Name);
            return true;
        }
        private Action<T, string, object> setAction;
        private Func<T, string, object> getFunction;
        private T node;
    }
}
