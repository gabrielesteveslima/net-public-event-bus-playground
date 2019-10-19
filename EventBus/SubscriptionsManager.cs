using System;
using System.Collections.Generic;
using System.Linq;
using EventBusPlayground.Abstractions;
using EventBusPlayground.Events;

namespace EventBusPlayground
{
    public class SubscriptionsManager : ISubscriptionsManage
    {
        private readonly List<Type> _eventTypes;
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;

        public SubscriptionsManager()
        {
            _handlers = new Dictionary<string, List<SubscriptionInfo>>();
            _eventTypes = new List<Type>();
        }

        public void Clear() => _handlers.Clear();

        public void AddSubscription<T, TH>()
            where T : Event
            where TH : IEventHandler<T>
        {
            var eventName = GetEventKey<T>();

            DoAddSubscription(typeof(TH), eventName);

            if (!_eventTypes.Contains(typeof(T)))
                _eventTypes.Add(typeof(T));
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];

        public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);

        public Type GetEventTypeByName(string eventName) => _eventTypes.SingleOrDefault(t => t.Name == eventName);

        public string GetEventKey<T>() => typeof(T).Name;

        private void DoAddSubscription(Type handlerType, string eventName)
        {
            if (!HasSubscriptionsForEvent(eventName))
                _handlers.Add(eventName, new List<SubscriptionInfo>());

            if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
                throw new ArgumentException($"Handler Type {handlerType.Name} already registered for '{eventName}'",
                    nameof(handlerType));

            _handlers[eventName].Add(new SubscriptionInfo(handlerType));
        }
    }
}
