using System.Threading.Tasks;
using BariBank.EventBus.Events;

namespace BariBank.EventBus.Abstractions
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
