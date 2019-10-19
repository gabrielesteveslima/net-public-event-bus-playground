using System;
using RabbitMQ.Client;

namespace EventBusPlayground.Connection
{
    public interface IRabbitMqConnection : IDisposable
    {
        bool IsConnected { get; }
        bool TryConnect();
        IModel CreateModel();
    }
}
