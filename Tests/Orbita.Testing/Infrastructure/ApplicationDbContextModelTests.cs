using Franz.Common.Business.Domain;
using Franz.Common.Mediator.Dispatchers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orbita.Domain.Entities;
using Orbita.Persistence;

namespace Orbita.Testing.Infrastructure;

public sealed class ApplicationDbContextModelTests
{
    [Fact]
    public void Domain_entities_are_mapped_as_independent_relational_roots()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=orbita_model;Username=postgres")
            .Options;

        using var services = new ServiceCollection().BuildServiceProvider();
        using var context = new ApplicationDbContext(options, new FranzDispatcher(services));

        var model = context.Model;
        var domainEntities = model.GetEntityTypes()
            .Where(entityType => typeof(DomainEntity).IsAssignableFrom(entityType.ClrType))
            .ToArray();

        Assert.NotEmpty(domainEntities);
        Assert.All(domainEntities, entityType => Assert.Null(entityType.BaseType));
        Assert.Null(model.FindEntityType(typeof(DomainEntity)));
        Assert.Null(model.FindEntityType(typeof(ProjectEntity)));
        Assert.Null(model.FindEntityType(typeof(Entity<Guid>)));

        Assert.Contains(typeof(Project), domainEntities.Select(entityType => entityType.ClrType));
        Assert.Contains(typeof(WorkTask), domainEntities.Select(entityType => entityType.ClrType));
        Assert.Contains(typeof(Absence), domainEntities.Select(entityType => entityType.ClrType));
    }

    [Fact]
    public void External_identity_is_persisted_with_global_issuer_subject_uniqueness()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=orbita_model;Username=postgres")
            .Options;

        using var services = new ServiceCollection().BuildServiceProvider();
        using var context = new ApplicationDbContext(options, new FranzDispatcher(services));

        var user = context.Model.FindEntityType(typeof(User));
        var identity = context.Model.FindEntityType(typeof(ExternalIdentity));

        Assert.NotNull(user);
        Assert.NotNull(identity);

        var relationship = Assert.Single(identity.GetForeignKeys());
        Assert.Equal(typeof(User), relationship.PrincipalEntityType.ClrType);
        Assert.Equal(DeleteBehavior.Restrict, relationship.DeleteBehavior);

        var uniqueIdentityKey = Assert.Single(identity.GetIndexes().Where(index =>
            index.IsUnique &&
            index.Properties.Select(property => property.Name)
                .SequenceEqual(new[] { nameof(ExternalIdentity.Issuer), nameof(ExternalIdentity.Subject) })));

        Assert.Equal(
            "IX_ExternalIdentities_Issuer_Subject",
            uniqueIdentityKey.GetDatabaseName());
    }

}
