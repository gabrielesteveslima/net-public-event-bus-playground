using System;
using System.Threading.Tasks;
using EventBusPlayground.Abstractions;
using EventBusPlayground.Events;
using Serilog;

namespace EventBusPlayground.Services.HostedService.IntegrationEvents
{
    public class HostedEventService : IHostedEventService
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;

        public HostedEventService(IEventBus eventBus, ILogger logger)
        {
            _logger = logger;
            _eventBus = _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public async Task PublishThroughEventBusAsync(Event @event)
        {
            try
            {
                _logger.Information(
                    "--- Publishing integration event: {IntegrationEventId_published} - ({@IntegrationEvent})",
                    @event.Id, @event);

                _eventBus.Publish(@event);
            }
            catch (Exception e)
            {
                _logger.Error(
                    e, "--- ERROR Publishing integration event: {IntegrationEventId_published} - ({@IntegrationEvent})",
                    @event.Id, @event);
            }
        }
    }
}
