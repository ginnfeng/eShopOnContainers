////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 12/29/2011 10:26:27 AM 
// Description: DynamicXmlBase.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Dynamic;

namespace Support.Net.Proxy
{
    public interface IDynamicXmlBase
    {
        string Value { get; set; }
    }

    public abstract class DynamicXmlBase<T> : DynamicObject, IDynamicXmlBase
        //where T : XObject
    {
        public DynamicXmlBase(T node)
        {
            this.Node = node;
        }
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;
            var type = Node.GetType();
            Type[] types = new Type[binder.CallInfo.ArgumentCount];
            for (int i = 0; i < args.Length; i++)
                types[i] = args[i] != null ? args[i].GetType() : null;
            var methodInfo = type.GetMethod(binder.Name, types);
            if (methodInfo == null)
                return false;
            result = methodInfo.Invoke(Node, args);
            return true;
        }
        abstract public string Value { get; set; }
        public T Node { get; set; }
    }
}
