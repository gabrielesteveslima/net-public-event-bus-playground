using System;
using RabbitMQ.Client;

namespace BariBank.EventBus.Connection
{
    public interface IRabbitMqConnection : IDisposable
    {
        bool IsConnected { get; }
        bool TryConnect();
        IModel CreateModel();
    }
}
