
using Common.Contract;
using MediatR;
using System;

namespace EventBus.Domain
{
    public abstract class CmdBase<TData, T> : EventBase<TData>,IRequest<T>, INotification
        //where TData : IDataContract,new()
    {
        public CmdBase()
        {}
        public CmdBase(TData data)
            :base(data)
        {}        
    }
    public abstract class CmdBase<TData> : CmdBase<TData, bool>
        //where TData : IDataContract, new()         
    {
        
        public CmdBase(TData data)
            : base(data) { }
    } 
  
}
