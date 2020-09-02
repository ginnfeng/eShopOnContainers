////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 10/22/2009 9:22:18 AM 
// Description: MemberDelegate.cs  
// Revisions  :            		
// **************************************************************************** 
using System;

namespace Common.Support.Net.LINQ
{
    public class MemberDelegate
    {
        public MemberDelegate(Delegate expressionDelegate, string expression)
        {
            ExpressionDefinition = expression;
            ExpressionDelegate = expressionDelegate;
        }        
        public Delegate ExpressionDelegate { get; set; }
        public string ExpressionDefinition { get; private set; }
        public object Invoke(object owner, params object[] parameters)
        {
            if (parameters.Length == 0)
                return ExpressionDelegate.DynamicInvoke(owner);
            object[] iuputParameters = new object[parameters.Length + 1];
            iuputParameters.SetValue(owner, 0);
            for (int i = 1; i <= parameters.Length; i++)
                iuputParameters.SetValue(parameters[i - 1], i);
            return ExpressionDelegate.DynamicInvoke(iuputParameters);
        }
    }
}
