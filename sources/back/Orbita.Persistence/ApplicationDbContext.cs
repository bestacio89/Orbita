using Franz.Common.Business.Domain;
using Franz.Common.EntityFramework;
using Franz.Common.Mediator.Dispatchers;
using Microsoft.EntityFrameworkCore;

namespace Orbita.Persistence;

public sealed class ApplicationDbContext : DbContextBase
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IDispatcher dispatcher) : base(options, dispatcher)
    {
    }

    public DbSet<Orbita.Domain.Entities.Project> Projects => Set<Orbita.Domain.Entities.Project>();
    public DbSet<Orbita.Domain.Entities.ProjectAssignment> ProjectAssignments => Set<Orbita.Domain.Entities.ProjectAssignment>();
    public DbSet<Orbita.Domain.Entities.ProjectDraft> ProjectDrafts => Set<Orbita.Domain.Entities.ProjectDraft>();
    public DbSet<Orbita.Domain.Entities.User> Users => Set<Orbita.Domain.Entities.User>();
    public DbSet<Orbita.Domain.Entities.ProjectJournalEntry> ProjectJournalEntries => Set<Orbita.Domain.Entities.ProjectJournalEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Every Franz-backed domain entity uses the same optimistic-concurrency contract.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.FindProperty(nameof(Orbita.Domain.Entities.DomainEntity.Version)) is not null)
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(Orbita.Domain.Entities.DomainEntity.Version))
                    .IsConcurrencyToken();
            }
        }
    }
}
