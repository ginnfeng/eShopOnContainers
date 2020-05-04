
using Common.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Domain
{
    
    public abstract class EventBase<TData>: IEvent
        //where TData : IDataContract, new()
    {
        public DateTime Timestamp { get; protected set; }
        protected EventBase()           
        {}
        public EventBase(TData data)
        {
            Timestamp = DateTime.Now;
            DataContract = data;
        }
        public TData DataContract { get; set; }        
    }
}

