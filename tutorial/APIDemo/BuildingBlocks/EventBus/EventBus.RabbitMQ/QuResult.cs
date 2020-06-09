////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/8/2020 10:26:16 AM 
// Description: QuResult.cs  
// Revisions  :            		
// **************************************************************************** 
using Common.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.RabbitMQ
{
    //public class QuResult<T> : IQuResult<T>
    //{
    //    public QuResult(T it)
    //    {
    //        Value = it;
    //    }
    //    public string CorrleationId { get; set; }

    //    public event Action<T> WaitResultEvent;
    //    public T Value { get;}
    //    public void OnResult(ICorrleation rlt)
    //    {
    //        if (WaitResultEvent != null)
    //        {
    //            var rlt2 = rlt as IQuResult<T>;
    //            if (rlt2 == null)
    //                throw new NullReferenceException(nameof(OnResult));
    //            WaitResultEvent(rlt2.Value);
    //        }
    //    }

    //    //static public implicit operator QuResult<T>(T it)
    //    //{
    //    //    return new QuResult<T>() { Value = it };
    //    //}
    //    //static public implicit operator T(QuResult<T> it)
    //    //{
    //    //    dynamic v= (it==null) ? null: it.Value;
    //    //    return ImpRegulation.Transfer.AutoToObject<T>();
    //    //}
    //}
}
