using Franz.Common.Mediator.Dispatchers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Orbita.Persistence;
using Testcontainers.PostgreSql;

namespace Orbita.Testing.Integration.Persistence;

public sealed class HistoricalPostgreSqlMigrationTests
{
    private const string HistoricalBaseline =
        "20261024160000_EvolutionsRevueBimensuelle";

    [Fact]
    public async Task Historical_migrations_create_the_original_schema_and_preserve_constraints()
    {
        await using var postgres = new PostgreSqlBuilder("postgres:18").Build();
        await postgres.StartAsync();

        using var services = new ServiceCollection().BuildServiceProvider();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(postgres.GetConnectionString())
            .Options;

        await using var context = new ApplicationDbContext(options, new FranzDispatcher(services));

        // Apply the historical baseline explicitly. This test verifies the imported migration
        // lineage, not the current Orbita model snapshot, which is validated by the forward
        // schema-translation migration tests.
        await context.GetService<IMigrator>().MigrateAsync(HistoricalBaseline);

        var appliedMigrations = (await context.Database.GetAppliedMigrationsAsync()).ToArray();

        Assert.Equal(
            [
                "20261019121926_Postgresqlmigration",
                HistoricalBaseline
            ],
            appliedMigrations);
        Assert.Empty(await context.Database.GetPendingMigrationsAsync());

        await context.Database.OpenConnectionAsync();

        await using (var command = context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = """
                SELECT COUNT(*)
                FROM information_schema.tables
                WHERE table_schema = 'public'
                  AND table_name IN ('Utilisateurs', 'Projets', 'Lots', 'Commentaires')
                """;

            var tableCount = Convert.ToInt32(await command.ExecuteScalarAsync());
            Assert.Equal(4, tableCount);
        }

        await using (var command = context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = """
                SELECT COUNT(*)
                FROM pg_indexes
                WHERE schemaname = 'public'
                  AND indexname IN (
                    'IX_Utilisateurs_IdentifiantExterne',
                    'IX_Lots_ProjetId_Code'
                  )
                """;

            var indexCount = Convert.ToInt32(await command.ExecuteScalarAsync());
            Assert.Equal(2, indexCount);
        }

        await using (var command = context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = """
                SELECT confdeltype
                FROM pg_constraint
                WHERE conname = 'FK_Lots_Projets_ProjetId'
                """;

            var deleteBehavior = (string?)await command.ExecuteScalarAsync();
            Assert.Equal("r", deleteBehavior);
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

    [Fact]
    public async Task Historical_project_lot_relationship_rejects_duplicate_codes_and_parent_deletion()
    {
        await using var postgres = new PostgreSqlBuilder("postgres:18").Build();
        await postgres.StartAsync();

        using var services = new ServiceCollection().BuildServiceProvider();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(postgres.GetConnectionString())
            .Options;

        await using var context = new ApplicationDbContext(options, new FranzDispatcher(services));
        await context.GetService<IMigrator>().MigrateAsync(HistoricalBaseline);

        var projectId = Guid.NewGuid();
        await context.Database.ExecuteSqlInterpolatedAsync($"""
            INSERT INTO "Projets"
                ("Id", "Nom", "Client", "Statut", "Remise", "RemiseSaisie", "Origine",
                 "CreeLe", "Parametres_Regroupement", "Parametres_NombreMois", "Version")
            VALUES
                ({projectId}, {"Migration test"}, {"Orbita"}, {"Actif"}, {0m}, {false},
                 {"Manuel"}, {DateTimeOffset.UtcNow}, {"Mois"}, {12}, {1})
            """);

        await context.Database.ExecuteSqlInterpolatedAsync($"""
            INSERT INTO "Lots" ("Id", "Code", "Nom", "ProjetId", "Version")
            VALUES ({Guid.NewGuid()}, {"LOT-01"}, {"Lot one"}, {projectId}, {1})
            """);

        var duplicateLotCode = await Assert.ThrowsAsync<PostgresException>(() =>
            context.Database.ExecuteSqlInterpolatedAsync($"""
                INSERT INTO "Lots" ("Id", "Code", "Nom", "ProjetId", "Version")
                VALUES ({Guid.NewGuid()}, {"LOT-01"}, {"Duplicate"}, {projectId}, {1})
                """));

        Assert.Equal(PostgresErrorCodes.UniqueViolation, duplicateLotCode.SqlState);

        var parentDelete = await Assert.ThrowsAsync<PostgresException>(() =>
            context.Database.ExecuteSqlInterpolatedAsync(
                $"""DELETE FROM "Projets" WHERE "Id" = {projectId}"""));

        Assert.Equal(PostgresErrorCodes.ForeignKeyViolation, parentDelete.SqlState);
    }
}
