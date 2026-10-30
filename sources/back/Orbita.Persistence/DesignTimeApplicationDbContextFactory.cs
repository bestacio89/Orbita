using Franz.Common.Mediator.Dispatchers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace Orbita.Persistence;

/// <summary>
/// Creates the PostgreSQL context for EF Core tooling without starting the API host.
/// The same Database configuration section used by the runtime is authoritative.
/// </summary>
public sealed class DesignTimeApplicationDbContextFactory
    : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var apiDirectory = FindApiConfigurationDirectory();
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environments.Production;

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var database = configuration.GetSection("Database");
        var provider = database["Provider"];

        if (!string.Equals(provider, "Postgres", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"EF Core migrations require Database:Provider=Postgres. Configured provider: '{provider ?? "<missing>"}'.");
        }

        var connectionString = new NpgsqlConnectionStringBuilder
        {
            Host = database["ServerName"] ?? "localhost",
            Database = database["DatabaseName"]
                ?? throw new InvalidOperationException("Database:DatabaseName is required."),
            Username = database["UserName"] ?? "postgres",
            Password = database["Password"] ?? string.Empty,
            Port = int.TryParse(database["Port"], out var port) && port > 0 ? port : 5432,
            SslMode = Enum.TryParse<SslMode>(database["Ssl"], ignoreCase: true, out var sslMode)
                ? sslMode
                : SslMode.Disable
        }.ConnectionString;

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        // Franz DbContextBase requires a dispatcher. EF model discovery and migration
        // generation do not dispatch application messages.
        var dispatcherServices = new ServiceCollection().BuildServiceProvider();
        var dispatcher = new FranzDispatcher(dispatcherServices);

        return new ApplicationDbContext(options, dispatcher);
    }

    private static string FindApiConfigurationDirectory()
    {
        for (var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
             directory is not null;
             directory = directory.Parent)
        {
            var apiDirectory = Path.Combine(directory.FullName, "sources", "back", "Orbita.API");
            if (File.Exists(Path.Combine(apiDirectory, "appsettings.json")))
            {
                return apiDirectory;
            }

            if (string.Equals(directory.Name, "Orbita.API", StringComparison.OrdinalIgnoreCase) &&
                File.Exists(Path.Combine(directory.FullName, "appsettings.json")))
            {
                return directory.FullName;
            }
        }

        throw new DirectoryNotFoundException(
            "Could not locate sources/back/Orbita.API/appsettings.json. Run EF Core tooling from the repository or API directory.");
    }
}
