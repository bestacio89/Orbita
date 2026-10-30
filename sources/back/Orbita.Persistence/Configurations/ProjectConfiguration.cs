using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Domain.Entities;

namespace Orbita.Persistence.Configurations;

internal sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");
        builder.HasKey(project => project.Id);
        builder.HasIndex(project => project.UniqueKey).IsUnique();
        builder.Property(project => project.UniqueKey).HasMaxLength(512).IsRequired();
        builder.Property(project => project.Client).HasMaxLength(256).IsRequired();
        builder.Property(project => project.Name).HasMaxLength(256).IsRequired();
        builder.Property(project => project.Reference).HasMaxLength(128);
        builder.Property(project => project.Code).HasMaxLength(50);
        builder.Property(project => project.Agency).HasMaxLength(256);
        builder.Property(project => project.ServiceLine).HasMaxLength(256);
        builder.Property(project => project.Director).HasMaxLength(256);
        builder.Property(project => project.ServiceLineManager).HasMaxLength(256);
        builder.Property(project => project.SalesRepresentative).HasMaxLength(256);
        builder.Property(project => project.ClientContact).HasMaxLength(256);
        builder.Property(project => project.ContractType).HasMaxLength(100);

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
        builder.Navigation(project => project.Assignments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
