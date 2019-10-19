using System.Threading.Tasks;
using EventBusPlayground.Events;

namespace EventBusPlayground.Abstractions
{
    public interface IEventHandler<in TEvent> : IEventHandler
        where TEvent : Event
    {
        Task Handle(TEvent @event);
    }

    public interface IEventHandler
    {
    }
}
