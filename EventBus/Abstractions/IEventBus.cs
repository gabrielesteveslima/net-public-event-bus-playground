using EventBusPlayground.Events;

namespace EventBusPlayground.Abstractions
{
    public interface IEventBus
    {
        void Publish(Event @event);

        void Subscribe<T, TH>()
            where T : Event
            where TH : IEventHandler<T>;
    }
}
