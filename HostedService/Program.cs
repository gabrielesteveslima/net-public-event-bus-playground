using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BariBank.EventBus;
using BariBank.EventBus.Abstractions;
using BariBank.EventBus.Connection;
using BariBank.Services.HostedService.IntegrationEvents;
using BariBank.Services.HostedService.IntegrationEvents.Events;
using BariBank.Services.HostedService.IntegrationEvents.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace BariBank.Services.HostedService
{
    public static class Program
    {
        private static readonly string Namespace = typeof(Program).Namespace;

        public static readonly string ApplicationName =
            Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static async Task Main(string[] args)
        {
            // The `UseServiceProviderFactory(new AutofacServiceProviderFactory())` call here allows for
            // ConfigureContainer to be supported with
            // a strongly-typed ContainerBuilder. If you don't
            // have the call to `UseServiceProviderFactory(new AutofacServiceProviderFactory())` here, you won't get
            // ConfigureContainer support. This also automatically
            // calls Populate to put services you register during
            // ConfigureServices into Autofac.
            await Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(ConfigureContainer)
                .ConfigureServices(ConfigureServices)
                .RunConsoleAsync();
        }

        private static void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            // Add any Autofac modules or registrations.
            // This is called AFTER ConfigureServices so things you
            // register here OVERRIDE things registered in ConfigureServices.
            //
            // You must have the call to `UseServiceProviderFactory(new AutofacServiceProviderFactory())`
            // when building the host or this won't be called.
            containerBuilder.Register<ILogger>((c, p) => new LoggerConfiguration()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://127.0.0.1:9200")))
                .CreateLogger()).SingleInstance();
            containerBuilder.RegisterType<SubscriptionsManager>().As<ISubscriptionsManage>();
            containerBuilder.RegisterType<HelloWorldEventHandler>();
            containerBuilder.RegisterType<HostedEventService>()
                .As<IHostedEventService>();
            containerBuilder.Register(_ => new RabbitMqConnection(new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@127.0.0.1")
            })).As<IRabbitMqConnection>();
            containerBuilder.Register(componentContext =>
            {
                var connection = componentContext.Resolve<IRabbitMqConnection>();
                var subsManage = componentContext.Resolve<ISubscriptionsManage>();
                var lifeTimeScope = componentContext.Resolve<ILifetimeScope>();
                var logger = componentContext.Resolve<ILogger>();

                var eventBus = new EventBus.EventBus(connection, lifeTimeScope, subsManage, logger, ApplicationName);
                ConfigureSubscribes(eventBus);

                return eventBus;
            }).As<IEventBus>();
        }

        private static void ConfigureSubscribes(IEventBus eventBus) =>
            eventBus.Subscribe<HelloWorldEvent, HelloWorldEventHandler>();

        private static void ConfigureServices(IServiceCollection services) => services.AddHostedService<Worker>();
    }
}