using BariBank.EventBus.Events;

namespace BariBank.Services.HostedService.IntegrationEvents.Events
{
    public class HelloWorldEvent : Event
    {
        public static string Message => "Hello World!";
    }
}
