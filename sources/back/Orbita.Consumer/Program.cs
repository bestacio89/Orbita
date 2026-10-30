using Orbita.Application;
using Orbita.Common.Business.Extensions;
using Orbita.Common.Http.EntityFramework.Extensions;
using Orbita.Common.Mediator.Extensions;
using Orbita.Common.Messaging.Bootstrap.Extenstions;
using Orbita.Common.Messaging.Hosting.Kafka;
using Orbita.Common.Messaging.Kafka.Extensions;
using Orbita.Domain.Entities;
using Orbita.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Orbita.Consumer;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        var environment = builder.Environment;
        var configuration = builder.Configuration;

        // Register the domain and application handlers used by inbound messages.
        builder.Services.AddBusiness(typeof(Book).Assembly);
        builder.Services.AddOrbitaMediator(new[] { typeof(ApplicationServiceCollectionExtensions).Assembly });

        // Provider-neutral database wiring; select the engine through configuration.
        builder.Services.AddRelationalDatabase<ApplicationDbContext>(environment, configuration);
        builder.Services.RegisterPersistenceServices<ApplicationDbContext>(configuration);

        // Register messaging contracts, Kafka transport, and host-owned listeners.
        builder.Services.AddMessagingArchitecture(environment, configuration);
        builder.Services.AddKafkaMessaging(configuration);
        builder.Services.AddKafkaHostedListener(
            options => configuration.GetSection("Messaging:Kafka").Bind(options));
        builder.Services.AddOutboxHostedListener(
            options => configuration.GetSection("Messaging:Outbox").Bind(options));

        using var host = builder.Build();
        await host.RunAsync();
    }
}
