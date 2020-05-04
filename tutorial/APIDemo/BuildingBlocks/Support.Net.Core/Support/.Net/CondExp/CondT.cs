////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 11/27/2008 5:29:16 PM 
// Description: Cond.cs  
// Revisions  :            		
// **************************************************************************** 
using System;

namespace Support.Net.CondExp
{
    public interface ICond
    {
        bool Eval();
    }

    public class Cond<T1> : ICond
    {
        public Cond(T1 param1, Func<T1, bool> exp)
        {
            Set(param1, exp);
        }
        public void Set(T1 param1, Func<T1, bool> exp)
        {
            parameter1 = param1;         
            expression = exp;
        }
        public bool Eval()
        {
            return expression(parameter1);
        }
        protected T1 parameter1;
        private Func<T1, bool> expression;
    }

    public class Cond<T1, T2> : ICond
    {
        public Cond(T1 param1,T2 param2,Func<T1,T2,bool> exp)
        {
            Set(param1,param2,exp);
        }
        public void Set(T1 param1, T2 param2, Func<T1,T2, bool> exp)
        {
            parameter1 = param1;
            parameter2 = param2;
            expression = exp;
        }
        public bool Eval()
        {
            return expression(parameter1, parameter2);
        }
        protected T1 parameter1;
        protected T2 parameter2;
        private Func<T1, T2, bool> expression;
    }
}
