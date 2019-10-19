using EventBusPlayground.Events;

namespace EventBusPlayground.Services.HostedService.IntegrationEvents.Events
{
    public class HelloWorldEvent : Event
    {
        public static string Message => "Hello World!";
    }
}
