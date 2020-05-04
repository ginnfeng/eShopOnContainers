////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 11/27/2008 4:33:26 PM 
// Description: ElementChecker.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
//using System.Windows.Controls;

namespace Support.Net.CondExp
{
    public class Cond:ICond
    {
        enum CondOperator
        {
            And,
            Or
        }

        public Cond()
        {            
        }
        public Cond(Func<bool> expression)
        {
            this.expression = expression;
        }
        public Cond AddCond(Func<bool> expression)
        {
            conds.Add(new Cond(expression));
            if (operators.Count + 1 < conds.Count)
                operators.Add(CondOperator.And);
            return this;            
        }
        public Cond AddCond<T1>(T1 param1, Func<T1, bool> cond)
        {
            conds.Add(new Cond<T1>(param1, cond));
            if (operators.Count + 1 < conds.Count)
                operators.Add(CondOperator.And);
            return this;
        }

        public Cond AddCond<T1, T2>(T1 param1, T2 param2, Func<T1, T2, bool> cond)
        {
            conds.Add(new Cond<T1,T2>(param1,param2,cond));
            if (operators.Count + 1 < conds.Count)
                AddCondOperator(CondOperator.And);
            return this;
        }

        public Cond AND()
        {            
            AddCondOperator(CondOperator.And);
            return this;
        }
        public Cond OR()
        {
            AddCondOperator(CondOperator.And);
            return this;
        }

        public bool Eval()
        {
            if (expression != null) return expression();
            if (conds.Count == 0) return true;
            bool result = conds[0].Eval();
            for (int i = 0; i < operators.Count; i++)
            {
                switch (operators[i])
                {
                    case CondOperator.And:
                        result = result && conds[i + 1].Eval();
                        break;
                    case CondOperator.Or:                 
                    default:
                        result = result || conds[i + 1].Eval();
                        break;
                }
            }
            return result;
        }

        public void ClearConditions()
        {
            expression = null;
            conds.Clear();
            operators.Clear();
        }

        private void AddCondOperator(CondOperator op)
        {
            if (operators.Count + 1 >= conds.Count)
                throw new ArgumentOutOfRangeException(op.ToString(), "Invalid Condition Operator");

            operators.Add(op);
        }

        private List<ICond> conds = new List<ICond>();
        private List<CondOperator> operators = new List<CondOperator>();
        private Func<bool> expression;
    }
}
