
using Common.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Domain
{
    public interface IEvent 
    {
        public DateTime Timestamp { get;}
    }
    public abstract class EventBase<TData>: IEvent
        where TData : IDataContract, new()
    {
        public DateTime Timestamp { get; protected set; }
        protected EventBase()
            :this(new TData())
        {}
        public EventBase(TData data)
        {
            Timestamp = DateTime.Now;
            DataContract = data;
        }
        public TData DataContract { get; private set; }        
    }
}

