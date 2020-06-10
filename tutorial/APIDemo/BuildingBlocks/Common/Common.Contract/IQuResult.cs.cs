////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/8/2020 10:26:47 AM 
// Description: IQuResult.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Contract
{
    public interface IQuCorrleation
    {
        string CorrleationId { get; set; }
        //public string ReplyQueue { set; get; }
        //object Value { get; }
        //void OnResult(object rlt);
    }
    public class QuResult: IQuCorrleation
    {
        public QuResult()
        {
        }
        public QuResult(object v)
        {
            Value = v;
        }
        public string CorrleationId { get; set; }        
        public object Value { get;  }
    }
    public class QuResult<T>: QuResult
    {
        public QuResult()
        {

        }
        public QuResult(T v)
            :base(v)
        {
        }        
        public T ToObject() { return (T)Value; }        
    }
    
}
