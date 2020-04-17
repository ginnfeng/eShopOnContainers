

using System.Threading.Tasks;

namespace EventBus.Domain
{
    public interface IEventHandler<in TEvent> : IntEventHandler
        where TEvent : IEvent
    {
        Task Handle(TEvent theEvent);
    }

    public interface IntEventHandler
    {
    }

}