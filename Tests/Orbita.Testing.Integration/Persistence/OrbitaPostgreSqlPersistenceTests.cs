using Franz.Common.Mediator.Dispatchers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;
using Orbita.Persistence;
using Orbita.Persistence.Repositories;
using Testcontainers.PostgreSql;

namespace Orbita.Testing.Integration.Persistence;

public sealed class OrbitaPostgreSqlPersistenceTests : IClassFixture<PostgreSqlPersistenceFixture>
{
    private readonly PostgreSqlPersistenceFixture _fixture;

    public OrbitaPostgreSqlPersistenceTests(PostgreSqlPersistenceFixture fixture) =>
        _fixture = fixture;

    [Fact]
    public async Task Migrations_create_the_current_model_and_leave_no_pending_changes()
    {
        await using var context = _fixture.CreateContext();

        Assert.Empty(await context.Database.GetPendingMigrationsAsync());
        Assert.False(context.Database.HasPendingModelChanges());

        var applied = (await context.Database.GetAppliedMigrationsAsync()).ToArray();
        Assert.NotEmpty(applied);
        Assert.Contains(applied, migration => migration.EndsWith("_InitialOrbitaSchema", StringComparison.Ordinal));

        var legacyTables = await ReadStringArrayAsync(
            context,
            """
            SELECT table_name
            FROM information_schema.tables
            WHERE table_schema = 'public'
              AND table_type = 'BASE TABLE'
              AND table_name IN ('Utilisateurs', 'Projets', 'Lots', 'Commentaires')
            """);

        Assert.Empty(legacyTables);
    }

    [Fact]
    public async Task PostgreSql_schema_contains_every_mapped_table_column_index_and_foreign_key()
    {
        await using var context = _fixture.CreateContext();
        await context.Database.OpenConnectionAsync();

        var expectedTables = context.Model.GetEntityTypes()
            .Select(entityType => (EntityType: entityType, Table: entityType.GetTableName(), Schema: entityType.GetSchema()))
            .Where(item => item.Table is not null)
            .Select(item => (item.Table!, item.Schema ?? "public"))
            .Distinct()
            .ToArray();

        var actualTables = await ReadPairsAsync(
            context,
            """
            SELECT table_name, table_schema
            FROM information_schema.tables
            WHERE table_type = 'BASE TABLE'
              AND table_schema NOT IN ('pg_catalog', 'information_schema')
            """);

        foreach (var table in expectedTables)
        {
            Assert.Contains((table.Item1, table.Item2), actualTables);
        }

        var actualColumns = await ReadPairsAsync(
            context,
            """
            SELECT table_name, column_name
            FROM information_schema.columns
            WHERE table_schema = 'public'
            """);

        foreach (var entityType in context.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (tableName is null)
            {
                continue;
            }

            var storeObject = StoreObjectIdentifier.Table(tableName, entityType.GetSchema());
            foreach (var property in entityType.GetProperties())
            {
                var columnName = property.GetColumnName(storeObject);
                if (columnName is not null)
                {
                    Assert.Contains((tableName, columnName), actualColumns);
                }
            }
        }

        var expectedIndexes = context.Model.GetEntityTypes()
            .SelectMany(entityType => entityType.GetIndexes())
            .Select(index => index.GetDatabaseName())
            .Where(name => name is not null)
            .Cast<string>()
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        var actualIndexes = await ReadStringArrayAsync(
            context,
            "SELECT indexname FROM pg_indexes WHERE schemaname = 'public'");

        foreach (var indexName in expectedIndexes)
        {
            Assert.Contains(indexName, actualIndexes);
        }

        var expectedForeignKeys = context.Model.GetEntityTypes()
            .SelectMany(entityType => entityType.GetForeignKeys())
            .Select(foreignKey => foreignKey.GetConstraintName())
            .Where(name => name is not null)
            .Cast<string>()
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        var actualForeignKeys = await ReadStringArrayAsync(
            context,
            """
            SELECT constraint_name
            FROM information_schema.table_constraints
            WHERE constraint_schema = 'public'
              AND constraint_type = 'FOREIGN KEY'
            """);

        foreach (var foreignKeyName in expectedForeignKeys)
        {
            Assert.Contains(foreignKeyName, actualForeignKeys);
        }
    }

    [Fact]
    public async Task User_repository_round_trips_external_identity_and_enforces_tenant_object_uniqueness()
    {
        var tenantId = Guid.NewGuid();
        var objectId = Guid.NewGuid();
        var user = User.Provision(
            tenantId,
            objectId,
            " Ada ",
            " Lovelace ",
            "ada@example.invalid",
            DateTimeOffset.UtcNow);

        await using (var context = _fixture.CreateContext())
        {
            var repository = new UserRepository(context);
            await repository.AddAsync(user);

            var persisted = await repository.GetByExternalIdentityAsync(tenantId, objectId);
            Assert.NotNull(persisted);
            Assert.Equal(user.Id, persisted.Id);
            Assert.Equal("Ada", persisted.FirstName);
            Assert.Equal("Lovelace", persisted.LastName);
            Assert.Equal("ada@example.invalid", persisted.Email);
        }

        await using var duplicateContext = _fixture.CreateContext();
        duplicateContext.Users.Add(User.Provision(
            tenantId,
            objectId,
            "Duplicate",
            "Identity",
            "duplicate@example.invalid",
            DateTimeOffset.UtcNow));

        var exception = await Assert.ThrowsAsync<DbUpdateException>(
            () => duplicateContext.SaveChangesAsync());

        Assert.Equal(PostgresErrorCodes.UniqueViolation, FindPostgresException(exception).SqlState);
    }

    [Fact]
    public async Task Project_repository_round_trips_owned_settings_and_project_assignments()
    {
        var user = User.Provision(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Grace",
            "Hopper",
            "grace@example.invalid",
            DateTimeOffset.UtcNow);

        var project = Project.CreateManually(
            "Analytical Engines",
            "Compiler Modernisation",
            "ORB-001",
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 12, 31),
            "Fixed price",
            user.Id,
            DateTimeOffset.UtcNow);

        project.AssignUser(user.Id, ProjectRole.ProjectManager, DateTimeOffset.UtcNow, user.Id);

        await using (var context = _fixture.CreateContext())
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();

            await new ProjectRepository(context).AddAsync(project);
        }

        await using var readContext = _fixture.CreateContext();
        var persisted = await new ProjectRepository(readContext).GetByIdAsync(project.Id);

        Assert.NotNull(persisted);
        Assert.Equal(project.UniqueKey, persisted.UniqueKey);
        Assert.Equal("Analytical Engines", persisted.Client);
        Assert.Equal("Compiler Modernisation", persisted.Name);
        Assert.Single(persisted.Assignments);
        Assert.Equal(user.Id, persisted.Assignments.Single().UserId);
        Assert.NotNull(persisted.Settings);
    }

    [Fact]
    public async Task Concurrent_user_updates_are_rejected_by_the_database_concurrency_token()
    {
        var user = User.Provision(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Initial",
            "Name",
            "initial@example.invalid",
            DateTimeOffset.UtcNow);

        await using (var seedContext = _fixture.CreateContext())
        {
            seedContext.Users.Add(user);
            await seedContext.SaveChangesAsync();
        }

        await using var firstContext = _fixture.CreateContext();
        await using var secondContext = _fixture.CreateContext();

        var first = await firstContext.Users.SingleAsync(candidate => candidate.Id == user.Id);
        var second = await secondContext.Users.SingleAsync(candidate => candidate.Id == user.Id);

        first.UpdateProfile("First", "Writer", "first@example.invalid");
        second.UpdateProfile("Second", "Writer", "second@example.invalid");

        await new UserRepository(firstContext).UpdateAsync(first);

        var exception = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
            () => new UserRepository(secondContext).UpdateAsync(second));

        Assert.Contains("Users", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Explicit_transaction_rolls_back_persisted_changes()
    {
        var user = User.Provision(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Rollback",
            "Candidate",
            "rollback@example.invalid",
            DateTimeOffset.UtcNow);

        await using (var context = _fixture.CreateContext())
        {
            await using var transaction = await context.Database.BeginTransactionAsync();
            context.Users.Add(user);
            await context.SaveChangesAsync();
            await transaction.RollbackAsync();
        }

        await using var verificationContext = _fixture.CreateContext();
        Assert.False(await verificationContext.Users.AnyAsync(candidate => candidate.Id == user.Id));
    }

    private static async Task<string[]> ReadStringArrayAsync(
        ApplicationDbContext context,
        string commandText)
    {
        await using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = commandText;
        await using var reader = await command.ExecuteReaderAsync();

        var values = new List<string>();
        while (await reader.ReadAsync())
        {
            values.Add(reader.GetString(0));
        }

        return values.ToArray();
    }

    private static async Task<HashSet<(string First, string Second)>> ReadPairsAsync(
        ApplicationDbContext context,
        string commandText)
    {
        await using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = commandText;
        await using var reader = await command.ExecuteReaderAsync();

        var values = new HashSet<(string First, string Second)>();
        while (await reader.ReadAsync())
        {
            values.Add((reader.GetString(0), reader.GetString(1)));
        }

        return values;
    }

    private static PostgresException FindPostgresException(Exception exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (current is PostgresException postgresException)
            {
                return postgresException;
            }
        }

        throw new InvalidOperationException("The expected PostgreSQL exception was not present.", exception);
    }
}

public sealed class PostgreSqlPersistenceFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:18").Build();
    private ServiceProvider? _dispatcherServices;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        _dispatcherServices = new ServiceCollection().BuildServiceProvider();

        await using var context = CreateContext();
        await context.Database.MigrateAsync();
    }

    public ApplicationDbContext CreateContext()
    {
        if (_dispatcherServices is null)
        {
            throw new InvalidOperationException("The PostgreSQL fixture has not been initialized.");
        }

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        return new ApplicationDbContext(options, new FranzDispatcher(_dispatcherServices));
    }

    public async Task DisposeAsync()
    {
        if (_dispatcherServices is not null)
        {
            await _dispatcherServices.DisposeAsync();
        }

        await _postgres.DisposeAsync();
    }
}
