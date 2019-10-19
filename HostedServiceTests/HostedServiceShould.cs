using System;
using EventBusPlayground.Abstractions;
using EventBusPlayground.Events;
using EventBusPlayground.Services.HostedService.IntegrationEvents;
using EventBusPlayground.Services.HostedService.IntegrationEvents.Events;
using Moq;
using Serilog;
using Xunit;

namespace EventBusPlayground.Services.HostedServiceTests
{
    public class HostedServiceShould
    {
        public HostedServiceShould()
        {
            _eventBusMock = new Mock<IEventBus>();
            _loggerMock = new Mock<ILogger>();
        }

        private readonly Mock<IEventBus> _eventBusMock;
        private readonly Mock<ILogger> _loggerMock;

        [Fact]
        public void PublishHelloEvent()
        {
            var @event = new HelloWorldEvent();

            var service = new HostedEventService(_eventBusMock.Object, _loggerMock.Object);
            service.PublishThroughEventBusAsync(@event);

            _eventBusMock.Verify(x => x.Publish(@event));
            _loggerMock.Verify(x => x.Information(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Event>()));
        }
    }
}
