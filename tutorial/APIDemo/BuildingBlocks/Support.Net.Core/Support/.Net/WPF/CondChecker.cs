////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 11/27/2008 4:39:34 PM 
// Description: ElementCheckerContainer.cs  
// Revisions  :            		
// **************************************************************************** 

#if WINONLY


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Support.Net.CondExp;
using System.Windows.Controls;

namespace Support.Net.WPF
{
    public class CondChecker<T>
    {        
        public CondChecker()
        {            
        }

        public virtual Cond BindingCond(T control)
        {
            ControlWrap<T> controlWrap=new ControlWrap<T>(control);           
            controlWraps.Add(controlWrap);
            return controlWrap.BindingCond;
        }

        public void ApplyCond(ApplyCondHandler<T> applyExpression) 
        {
            foreach (var item in controlWraps)
            {
                item.ApplyCond(applyExpression);
            }
        }


        List<ControlWrap<T>> controlWraps = new List<ControlWrap<T>>();
    }
}

#endif