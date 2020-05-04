

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
        
        /// <typeparam name="TD">data contract</typeparam>        
        Task SendCmd<TD>(CmdBase<TD> command);
        Task<TRlt> SendCmd<TD, TRlt>(CmdBase<TD, TRlt> command);
        Task PublishCmd<T>(T command) where T : IEvent;

        public void PublishEvent<T>(T theEevent) where T : IEvent;
           
        void SubscribeEvent<T,TH>()
            where T : IEvent
            where TH : IEventHandler<T>;
    }
}