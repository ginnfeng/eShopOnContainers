////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 10/22/2009 9:21:44 AM 
// Description: PropertyDelegate.cs  
// Revisions  :            		
// **************************************************************************** 
using System;

namespace Support.Net.LINQ
{
    public class PropertyDelegate : MemberDelegate
    {
        public PropertyDelegate(Delegate expressionDelegate, string expression)
            : base(expressionDelegate, expression)
        {
        }
        public object GetValue(object owner)
        {
            return base.Invoke(owner);
        }
        public TResult GetValue<TResult>(object owner)
        {
            return (TResult)GetValue(owner);
        }
        public void SetValue(object owner, object parameter)
        {
            base.Invoke(owner, parameter);
        }
    }
}
