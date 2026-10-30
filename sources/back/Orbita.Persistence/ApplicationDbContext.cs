using Franz.Common.Business.Domain;
using Franz.Common.EntityFramework;
using Franz.Common.Mediator.Dispatchers;
using Microsoft.EntityFrameworkCore;
using Orbita.Domain.Entities;

namespace Orbita.Persistence;

public sealed class ApplicationDbContext : DbContextBase
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IDispatcher dispatcher) : base(options, dispatcher)
    {
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectAssignment> ProjectAssignments => Set<ProjectAssignment>();
    public DbSet<ProjectDraft> ProjectDrafts => Set<ProjectDraft>();
    public DbSet<User> Users => Set<User>();
    public DbSet<ProjectJournalEntry> ProjectJournalEntries => Set<ProjectJournalEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>(builder =>
        {
            builder.ToTable("Projects");
            builder.HasKey(project => project.Id);
            builder.HasIndex(project => project.UniqueKey).IsUnique();
            builder.Property(project => project.UniqueKey).HasMaxLength(512).IsRequired();
            builder.Property(project => project.Version).IsConcurrencyToken();
            builder.Property(project => project.Client).HasMaxLength(256).IsRequired();
            builder.Property(project => project.Name).HasMaxLength(256).IsRequired();
            builder.Property(project => project.Reference).HasMaxLength(128);
            builder.OwnsOne(project => project.Settings, settings =>
            {
                settings.Property(value => value.Grouping).HasConversion<int>();
                settings.Property(value => value.FirstPlanningMonth).HasMaxLength(7);
                settings.Property(value => value.GovernanceInstances).HasMaxLength(2000);
            });
            builder.Navigation(project => project.Settings).IsRequired();
            builder.HasMany(project => project.Assignments)
                .WithOne()
                .HasForeignKey(assignment => assignment.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(project => project.Assignments).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<ProjectAssignment>(builder =>
        {
            builder.ToTable("ProjectAssignments");
            builder.HasKey(assignment => assignment.Id);
            builder.HasIndex(assignment => new { assignment.ProjectId, assignment.UserId }).IsUnique();
            builder.Property(assignment => assignment.Version).IsConcurrencyToken();
        });

        modelBuilder.Entity<ProjectDraft>(builder =>
        {
            builder.ToTable("ProjectDrafts");
            builder.HasKey(draft => draft.Id);
            builder.HasIndex(draft => new { draft.AuthorUserId, draft.ModifiedAt });
            builder.HasIndex(draft => new { draft.CoAuthorUserId, draft.ModifiedAt });
            builder.Property(draft => draft.Version).IsConcurrencyToken();
            builder.Property(draft => draft.Client).HasMaxLength(256).IsRequired();
            builder.Property(draft => draft.ProjectName).HasMaxLength(256).IsRequired();
            builder.Property(draft => draft.Reference).HasMaxLength(128);
            builder.Property(draft => draft.Content).IsRequired();
        });

        modelBuilder.Entity<User>(builder =>
        {
            builder.ToTable("Users");
            builder.HasKey(user => user.Id);
            builder.HasIndex(user => new { user.TenantId, user.ObjectId }).IsUnique();
            builder.Property(user => user.Email).HasMaxLength(320);
            builder.Property(user => user.FirstName).HasMaxLength(128);
            builder.Property(user => user.LastName).HasMaxLength(128);
            builder.Ignore(user => user.DisplayName);
            builder.Property(user => user.Version).IsConcurrencyToken();
        });

        modelBuilder.Entity<ProjectJournalEntry>(builder =>
        {
            builder.ToTable("ProjectJournalEntries");
            builder.HasKey(entry => entry.Id);
            builder.HasIndex(entry => new { entry.ProjectId, entry.OccurredAt });
            builder.Property(entry => entry.EntityType).HasMaxLength(128).IsRequired();
            builder.Property(entry => entry.MemberName).HasMaxLength(128).IsRequired();
            builder.Property(entry => entry.PreviousValue);
            builder.Property(entry => entry.NewValue);
        });

    }
}
