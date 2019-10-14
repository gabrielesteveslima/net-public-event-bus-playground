using System.Threading.Tasks;
using BariBank.EventBus.Abstractions;
using BariBank.Services.HostedService.IntegrationEvents.Events;
using Serilog;

namespace BariBank.Services.HostedService.IntegrationEvents.Handlers
{
    public class HelloWorldEventHandler : IEventHandler<HelloWorldEvent>
    {
        public Task Handle(HelloWorldEvent @event)
        {
            Log.Information(
                "--- {AppName} says {Message}",
                @event.ServiceId, HelloWorldEvent.Message);

            return Task.CompletedTask;
        }
    }
}
