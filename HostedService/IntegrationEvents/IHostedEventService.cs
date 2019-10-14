using System.Threading.Tasks;
using BariBank.EventBus.Events;

namespace BariBank.Services.HostedService.IntegrationEvents
{
    public interface IHostedEventService
    {
        Task PublishThroughEventBusAsync(Event @event);
    }
}
