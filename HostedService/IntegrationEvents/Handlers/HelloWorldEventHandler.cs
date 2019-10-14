using System.Threading.Tasks;
using BariBank.EventBus.Abstractions;
using BariBank.Services.HostedService.IntegrationEvents.Events;
using Serilog;

namespace BariBank.Services.HostedService.IntegrationEvents.Handlers
{
    public class HelloWorldEventHandler : IEventHandler<HelloWorldEvent>
    {
        private readonly ILogger _logger;

        public HelloWorldEventHandler(ILogger logger)
        {
            _logger = logger;
        }

        public Task Handle(HelloWorldEvent @event)
        {
            _logger.Information(
                "--- Receiving integration event: From {ServiceId} - ({@IntegrationEvent})",
                @event.ServiceId, @event);

            return Task.CompletedTask;
        }
    }
}