

using Common.Contract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Domain
{
    public interface IEventBus
    {
        //Task SendCommand<T>(T command) where T : Command;
        public void PublishEvent<T>(T theEevent) where T : IEvent;
           
        void SubscribeEvent<T,TH>()
            where T : IEvent
            where TH : IEventHandler<T>;
    }
}