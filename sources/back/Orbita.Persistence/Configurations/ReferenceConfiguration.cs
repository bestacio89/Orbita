using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Domain.Entities;

namespace Orbita.Persistence.Configurations;

internal sealed class ProjectAxisConfiguration : IEntityTypeConfiguration<ProjectAxis>
{
    public void Configure(EntityTypeBuilder<ProjectAxis> builder)
    {
        builder.ToTable("ProjectAxes");
        builder.Property(value => value.Code).HasMaxLength(50).IsRequired();
        builder.Property(value => value.Name).HasMaxLength(400).IsRequired();
        builder.HasIndex(value => new { value.ProjectId, value.Code }).IsUnique();
    }
}

internal sealed class ProjectLotConfiguration : IEntityTypeConfiguration<ProjectLot>
{
    public void Configure(EntityTypeBuilder<ProjectLot> builder)
    {
        builder.ToTable("ProjectLots");
        builder.Property(value => value.Code).HasMaxLength(50).IsRequired();
        builder.Property(value => value.Name).HasMaxLength(400).IsRequired();
        builder.HasIndex(value => new { value.ProjectId, value.Code }).IsUnique();
    }
}

internal sealed class ProjectPhaseConfiguration : IEntityTypeConfiguration<ProjectPhase>
{
    public void Configure(EntityTypeBuilder<ProjectPhase> builder)
    {
        builder.ToTable("ProjectPhases");
        builder.Property(value => value.Code).HasMaxLength(50).IsRequired();
        builder.HasIndex(value => new { value.ProjectId, value.Code }).IsUnique();
    }
}

internal sealed class ProjectStepConfiguration : IEntityTypeConfiguration<ProjectStep>
{
    public void Configure(EntityTypeBuilder<ProjectStep> builder)
    {
        builder.ToTable("ProjectSteps");
        builder.Property(value => value.Name).HasMaxLength(400).IsRequired();
        builder.HasIndex(value => new { value.ProjectId, value.Order });
    }
}

internal sealed class ProjectTaskTypeConfiguration : IEntityTypeConfiguration<ProjectTaskType>
{
    public void Configure(EntityTypeBuilder<ProjectTaskType> builder)
    {
        builder.ToTable("ProjectTaskTypes");
        builder.Property(value => value.Code).HasMaxLength(50).IsRequired();
        builder.Property(value => value.Name).HasMaxLength(200).IsRequired();
        builder.HasIndex(value => new { value.ProjectId, value.Code }).IsUnique();
    }
}

internal sealed class AiReferenceValueConfiguration : IEntityTypeConfiguration<AiReferenceValue>
{
    public void Configure(EntityTypeBuilder<AiReferenceValue> builder)
    {
        builder.ToTable("AiReferenceValues");
        builder.Property(value => value.ListKind).HasConversion<int>();
        builder.Property(value => value.Label).HasMaxLength(400).IsRequired();
        builder.HasIndex(value => new { value.ProjectId, value.ListKind, value.Label }).IsUnique();
    }
}
