using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Domain.Entities;

namespace Orbita.Persistence.Configurations;

internal sealed class ProjectFieldConfiguration : IEntityTypeConfiguration<ProjectField>
{
    public void Configure(EntityTypeBuilder<ProjectField> builder)
    {
        builder.ToTable("ProjectFields");
        builder.Property(value => value.Label).HasMaxLength(400).IsRequired();
        builder.Property(value => value.Value).HasMaxLength(2000);
        builder.Property(value => value.Key).HasMaxLength(50);
        builder.HasIndex(value => new { value.ProjectId, value.Order });
    }
}

internal sealed class TspImportRecordConfiguration : IEntityTypeConfiguration<TspImportRecord>
{
    public void Configure(EntityTypeBuilder<TspImportRecord> builder)
    {
        builder.ToTable("TspImportRecords");
        builder.Property(value => value.FileName).HasMaxLength(260);
        builder.Property(value => value.ObservedPoints).HasMaxLength(4000);
        builder.Property(value => value.ProfilePlanningSource).HasMaxLength(4000);
        builder.Property(value => value.PhasePlanningSource).HasMaxLength(4000);
        builder.HasOne<User>().WithMany().HasForeignKey(value => value.ImportedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(value => value.HiddenByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(value => new { value.ProjectId, value.ImportedAt });
    }
}
