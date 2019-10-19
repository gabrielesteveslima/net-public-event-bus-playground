using System.Threading.Tasks;
using EventBusPlayground.Abstractions;
using EventBusPlayground.Services.HostedService.IntegrationEvents.Events;
using Serilog;

namespace EventBusPlayground.Services.HostedService.IntegrationEvents.Handlers
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