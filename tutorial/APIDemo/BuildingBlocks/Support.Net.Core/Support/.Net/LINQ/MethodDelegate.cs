////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 10/22/2009 9:22:02 AM 
// Description: MethodDelegate.cs  
// Revisions  :            		
// **************************************************************************** 
using System;

namespace Support.Net.LINQ
{
    public class MethodDelegate : MemberDelegate
    {
        public MethodDelegate(Delegate expressionDelegate, string expression)
            :base(expressionDelegate, expression)
        {

        }
        public TResult Function<TResult>(object owner, params object[] parameters)
        {
            return (TResult)Function(owner, parameters);
        }
        public object Function(object owner, params object[] parameters)
        {
            return base.Invoke(owner, parameters);
        }
        public void Action(object owner, params object[] parameters)
        {
            Function(owner, parameters);
        }
    }
}
