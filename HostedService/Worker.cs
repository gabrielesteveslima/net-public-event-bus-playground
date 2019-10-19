using System;
using System.Threading;
using System.Threading.Tasks;
using EventBusPlayground.Services.HostedService.IntegrationEvents;
using EventBusPlayground.Services.HostedService.IntegrationEvents.Events;
using Microsoft.Extensions.Hosting;

namespace EventBusPlayground.Services.HostedService
{
    /// <summary>
    ///     Worker class inherits a long running <see cref="T:Microsoft.Extensions.Hosting.IHostedService" />.
    /// </summary>
    public class Worker : BackgroundService
    {
        private readonly IHostedEventService _eventService;

        public Worker(IHostedEventService eventService)
        {
            _eventService = eventService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _eventService.PublishThroughEventBusAsync(new HelloWorldEvent
                {
                    ServiceId = Program.ApplicationName
                });

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
