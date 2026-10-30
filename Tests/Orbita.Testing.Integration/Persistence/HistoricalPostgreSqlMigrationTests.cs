using Franz.Common.Mediator.Dispatchers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Orbita.Persistence;
using Testcontainers.PostgreSql;

namespace Orbita.Testing.Integration.Persistence;

public sealed class HistoricalPostgreSqlMigrationTests
{
    [Fact]
    public async Task Historical_migrations_create_the_original_schema_and_preserve_unique_identity_constraint()
    {
        await using var postgres = new PostgreSqlBuilder("postgres:18").Build();
        await postgres.StartAsync();

        using var services = new ServiceCollection().BuildServiceProvider();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(postgres.GetConnectionString())
            // This test deliberately executes the historical Magellan baseline before the Orbita
            // forward migration exists. Keep the exception local to this legacy-baseline test.
            // The final Orbita migration tests must run without suppressing this warning.
            .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning))
            .Options;

        await using var context = new ApplicationDbContext(options, new FranzDispatcher(services));

        await context.Database.MigrateAsync();

        var appliedMigrations = (await context.Database.GetAppliedMigrationsAsync()).ToArray();

        Assert.Equal(
            [
                "20261019121926_Postgresqlmigration",
                "20261024160000_EvolutionsRevueBimensuelle"
            ],
            appliedMigrations);

        await using (var command = context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = """
                SELECT COUNT(*)
                FROM information_schema.tables
                WHERE table_schema = 'public'
                  AND table_name IN ('Utilisateurs', 'Projets', 'Lots', 'Commentaires')
                """;

            await context.Database.OpenConnectionAsync();
            var tableCount = Convert.ToInt32(await command.ExecuteScalarAsync());

            Assert.Equal(4, tableCount);
        }

        var userId = Guid.NewGuid();
        await context.Database.ExecuteSqlInterpolatedAsync($"""
            INSERT INTO "Utilisateurs"
                ("Id", "IdentifiantExterne", "Nom", "Prenom", "Email", "Actif", "Administrateur", "CreeLe", "Version")
            VALUES
                ({userId}, {"tenant-object-identity"}, {"Architect"}, {"Orbita"}, {"architect@example.invalid"},
                 {true}, {false}, {DateTimeOffset.UtcNow}, {1})
            """);

        var duplicateIdentity = await Assert.ThrowsAsync<PostgresException>(() =>
            context.Database.ExecuteSqlInterpolatedAsync($"""
                INSERT INTO "Utilisateurs"
                    ("Id", "IdentifiantExterne", "Nom", "Prenom", "Email", "Actif", "Administrateur", "CreeLe", "Version")
                VALUES
                    ({Guid.NewGuid()}, {"tenant-object-identity"}, {"Second"}, {"User"}, {"second@example.invalid"},
                     {true}, {false}, {DateTimeOffset.UtcNow}, {1})
                """));

        Assert.Equal(PostgresErrorCodes.UniqueViolation, duplicateIdentity.SqlState);
    }
}
