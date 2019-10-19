using System;
using System.Collections.Generic;
using EventBusPlayground.Abstractions;
using EventBusPlayground.Events;

namespace EventBusPlayground
{
    public interface ISubscriptionsManage
    {
        void Clear();

        void AddSubscription<T, TH>()
            where T : Event
            where TH : IEventHandler<T>;

        bool HasSubscriptionsForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
        string GetEventKey<T>();
    }
}
