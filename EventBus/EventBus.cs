using System;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EventBusPlayground.Abstractions;
using EventBusPlayground.Connection;
using EventBusPlayground.Connection.ResiliencePolicies;
using EventBusPlayground.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace EventBusPlayground
{
    public class EventBus : IEventBus, IDisposable
    {
        private const string AutofacScopeName = "BariBankScope";
        private const string BrokerName = "BariBank";

        private readonly ILifetimeScope _autofac;
        private readonly IRabbitMqConnection _connection;
        private readonly string _queueName;
        private readonly ISubscriptionsManage _subscriptionsManage;
        private readonly ILogger _logger;

        private IModel _consumerChannel;

        public EventBus(IRabbitMqConnection connection,
            ILifetimeScope autofac, ISubscriptionsManage subsManager, ILogger logger, string queueName = null)
        {
            _connection =
                connection ?? throw new ArgumentNullException(nameof(connection));
            _subscriptionsManage = subsManager ?? new SubscriptionsManager();
            _queueName = queueName;
            _consumerChannel = CreateConsumerChannel();
            _autofac = autofac;
            _logger = logger;
        }

        public void Dispose()
        {
            _consumerChannel?.Dispose();
            _subscriptionsManage.Clear();
        }

        public void Publish(Event @event)
        {
            if (!_connection.IsConnected) _connection.TryConnect();

            using var channel = _connection.CreateModel();
            var eventName = @event.GetType()
                .Name;

            channel.ExchangeDeclare(BrokerName,
                "direct");

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            Policies.WaitRetryPolicy(3).Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2;

                channel.BasicPublish(BrokerName,
                    eventName,
                    true,
                    properties,
                    body);
            });
        }

        public void Subscribe<T, TH>()
            where T : Event
            where TH : IEventHandler<T>
        {
            var eventName = _subscriptionsManage.GetEventKey<T>();
            DoInternalSubscription(eventName);

            _logger.Information("Subscribing to event {EventName} with {EventHandler}", eventName,
                typeof(TH));

            _subscriptionsManage.AddSubscription<T, TH>();
        }

        private void DoInternalSubscription(string eventName)
        {
            var containsKey = _subscriptionsManage.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                if (!_connection.IsConnected)
                    _connection.TryConnect();

                using var channel = _connection.CreateModel();
                channel.QueueBind(_queueName,
                    BrokerName,
                    eventName);
            }
        }

        private IModel CreateConsumerChannel()
        {
            if (!_connection.IsConnected)
                _connection.TryConnect();

            var channel = _connection.CreateModel();
            channel.ExchangeDeclare(BrokerName,
                "direct");

            channel.QueueDeclare(_queueName,
                true,
                false,
                false,
                null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body);

                await ProcessEvent(eventName, message);
                channel.BasicAck(ea.DeliveryTag, false);
            };

            channel.BasicConsume(_queueName,
                false,
                consumer);

            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };

            return channel;
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_subscriptionsManage.HasSubscriptionsForEvent(eventName))
            {
                using var scope = _autofac.BeginLifetimeScope(AutofacScopeName);
                foreach (var subscription in _subscriptionsManage.GetHandlersForEvent(eventName))
                {
                    var handler = scope.ResolveOptional(subscription.HandlerType);
                    if (handler != null)
                    {
                        var eventType = _subscriptionsManage.GetEventTypeByName(eventName);
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                        await (Task)concreteType.GetMethod("Handle")
                            .Invoke(handler, new[] {integrationEvent});
                    }
                }
            }
        }
    }
}
