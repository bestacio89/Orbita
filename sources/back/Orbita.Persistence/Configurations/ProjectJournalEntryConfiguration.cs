using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Domain.Entities;

namespace Orbita.Persistence.Configurations;

internal sealed class ProjectJournalEntryConfiguration : IEntityTypeConfiguration<ProjectJournalEntry>
{
    public void Configure(EntityTypeBuilder<ProjectJournalEntry> builder)
    {
        builder.ToTable("ProjectJournalEntries");
        builder.HasKey(entry => entry.Id);
        builder.HasIndex(entry => new { entry.ProjectId, entry.OccurredAt });
        builder.HasIndex(entry => new { entry.ProjectId, entry.EntityType, entry.EntityId, entry.OccurredAt });
        builder.Property(entry => entry.EntityType).HasMaxLength(128).IsRequired();
        builder.Property(entry => entry.MemberName).HasMaxLength(128).IsRequired();
        builder.Property(entry => entry.PreviousValue).HasMaxLength(4000);
        builder.Property(entry => entry.NewValue).HasMaxLength(4000);
    }
}
