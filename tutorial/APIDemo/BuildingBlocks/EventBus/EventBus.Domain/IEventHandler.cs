

using System.Threading.Tasks;

namespace EventBus.Domain
{
    public interface IEventHandler<in TEvent> : IEventHandler
        where TEvent : IEvent
    {
        Task Handle(TEvent theEvent);
    }

    public interface IEventHandler
    {
    }
}