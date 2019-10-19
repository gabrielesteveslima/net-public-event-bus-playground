using System.Threading.Tasks;
using EventBusPlayground.Events;

namespace EventBusPlayground.Services.HostedService.IntegrationEvents
{
    public interface IHostedEventService
    {
        Task PublishThroughEventBusAsync(Event @event);
    }
}
