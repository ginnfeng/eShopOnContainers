////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 11/28/2008 10:48:05 AM 
// Description: ControlWrap.cs  
// Revisions  :            		
// **************************************************************************** 

#if WINONLY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Support.Net.CondExp;

namespace Support.Net.WPF
{
    public delegate void ApplyCondHandler<T>(T control, bool setting);

    public interface IControlWrap
    {
        Cond BindingCond { get; set; }
    }
    public interface IControlWrap<T> : IControlWrap
    {
        void ApplyCond(ApplyCondHandler<T> applyExpression);        
    }

    class ControlWrap<T> : IControlWrap<T>        
    {
        public ControlWrap(T control)
        {
            this.control = control;
            BindingCond = new Cond();
        }
        public void ApplyCond(ApplyCondHandler<T> applyExpression)                       
        {
            applyExpression(this.control, BindingCond.Eval());
        }
        public Cond BindingCond { get; set; }
        
        private T control;
    }
}

#endif