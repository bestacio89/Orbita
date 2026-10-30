using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Domain.Entities;

namespace Orbita.Persistence.Configurations;

internal sealed class ProjectDraftConfiguration : IEntityTypeConfiguration<ProjectDraft>
{
    public void Configure(EntityTypeBuilder<ProjectDraft> builder)
    {
        builder.ToTable("ProjectDrafts");
        builder.HasKey(draft => draft.Id);
        builder.HasIndex(draft => new { draft.AuthorUserId, draft.ModifiedAt });
        builder.HasIndex(draft => new { draft.CoAuthorUserId, draft.ModifiedAt });
        builder.Property(draft => draft.Client).HasMaxLength(256).IsRequired();
        builder.Property(draft => draft.ProjectName).HasMaxLength(256).IsRequired();
        builder.Property(draft => draft.Reference).HasMaxLength(128);
        builder.Property(draft => draft.Source).HasMaxLength(512);
        builder.Property(draft => draft.Content).IsRequired();
        builder.Property(draft => draft.Origin).HasConversion<int>();
    }
}
