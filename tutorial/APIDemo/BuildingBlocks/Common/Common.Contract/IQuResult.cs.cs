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
    public interface IQuResult
    {
        public string CorrleationId { get; set; }
        void OnResult(object rlt);
    }
    public interface IQuResult<T>
    {
        public event Action<T> WaitResultEvent;
        public T Value { get;}
    }
    
}
