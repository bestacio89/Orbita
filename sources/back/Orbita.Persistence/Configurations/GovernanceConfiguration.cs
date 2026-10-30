using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Domain.Entities;

namespace Orbita.Persistence.Configurations;

internal sealed class WorkshopConfiguration : IEntityTypeConfiguration<Workshop>
{
    public void Configure(EntityTypeBuilder<Workshop> builder)
    {
        builder.ToTable("Workshops");
        builder.HasOne<Project>().WithMany().HasForeignKey(value => value.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<WorkTask>().WithMany().HasForeignKey(value => value.TaskId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(value => value.TaskId).IsUnique();
        builder.HasMany(value => value.Recipients).WithOne()
            .HasForeignKey(recipient => recipient.WorkshopId);
    }
}

internal sealed class WorkshopRecipientConfiguration : IEntityTypeConfiguration<WorkshopRecipient>
{
    public void Configure(EntityTypeBuilder<WorkshopRecipient> builder)
    {
        builder.ToTable("WorkshopRecipients");
        builder.HasOne<Project>().WithMany().HasForeignKey(value => value.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(value => value.NameOrEmail).HasMaxLength(320).IsRequired();
        builder.HasIndex(value => new { value.WorkshopId, value.Order });
        builder.HasOne<User>().WithMany().HasForeignKey(value => value.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class DeliverableConfiguration : IEntityTypeConfiguration<Deliverable>
{
    public void Configure(EntityTypeBuilder<Deliverable> builder)
    {
        builder.ToTable("Deliverables");
        builder.HasOne<Project>().WithMany().HasForeignKey(value => value.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<WorkTask>().WithMany().HasForeignKey(value => value.TaskId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(value => value.TaskId).IsUnique();
        builder.Property(value => value.StorageUri).HasMaxLength(1000);
    }
}

internal sealed class ProjectCommentConfiguration : IEntityTypeConfiguration<ProjectComment>
{
    public void Configure(EntityTypeBuilder<ProjectComment> builder)
    {
        builder.ToTable("ProjectComments");
        builder.HasOne<Project>().WithMany().HasForeignKey(value => value.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(value => value.TargetType).HasMaxLength(30).IsRequired();
        builder.Property(value => value.Text).HasMaxLength(4000).IsRequired();
        builder.HasIndex(value => new { value.ProjectId, value.TargetType, value.TargetId, value.CreatedAt });
        builder.HasOne<User>().WithMany().HasForeignKey(value => value.AuthorUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class WeeklyStatusPointConfiguration : IEntityTypeConfiguration<WeeklyStatusPoint>
{
    public void Configure(EntityTypeBuilder<WeeklyStatusPoint> builder)
    {
        builder.ToTable("WeeklyStatusPoints");
        builder.HasOne<Project>().WithMany().HasForeignKey(value => value.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(value => value.Key).HasMaxLength(20).IsRequired();
        builder.Property(value => value.Content).HasColumnType("text").IsRequired();
        builder.HasIndex(value => new { value.ProjectId, value.Key }).IsUnique();
        builder.HasOne<User>().WithMany().HasForeignKey(value => value.ModifiedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class ProjectActionConfiguration : IEntityTypeConfiguration<ProjectAction>
{
    public void Configure(EntityTypeBuilder<ProjectAction> builder)
    {
        builder.ToTable("ProjectActions", table =>
            table.HasCheckConstraint(
                "CK_ProjectActions_Progress",
                "\"Progress\" >= 0 AND \"Progress\" <= 1"));

        builder.Property(value => value.Reference).HasMaxLength(20).IsRequired();
        builder.Property(value => value.Title).HasMaxLength(1000).IsRequired();
        builder.Property(value => value.OwnerName).HasMaxLength(256);
        builder.Property(value => value.GovernanceInstance).HasMaxLength(100);
        builder.Property(value => value.Status).HasConversion<int>();
        builder.Property(value => value.Comment).HasMaxLength(4000);
        builder.HasIndex(value => new { value.ProjectId, value.Reference }).IsUnique();
        builder.HasOne<User>().WithMany().HasForeignKey(value => value.OwnerUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(value => value.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<WorkTask>().WithMany().HasForeignKey(value => value.TaskId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class ProjectDecisionConfiguration : IEntityTypeConfiguration<ProjectDecision>
{
    public void Configure(EntityTypeBuilder<ProjectDecision> builder)
    {
        builder.ToTable("ProjectDecisions");
        builder.Property(value => value.Reference).HasMaxLength(20).IsRequired();
        builder.Property(value => value.GovernanceInstance).HasMaxLength(100);
        builder.Property(value => value.Title).HasMaxLength(1000).IsRequired();
        builder.Property(value => value.DecisionMaker).HasMaxLength(256);
        builder.Property(value => value.Impact).HasMaxLength(2000);
        builder.Property(value => value.Status).HasConversion<int>();
        builder.Property(value => value.Comment).HasMaxLength(4000);
        builder.HasIndex(value => new { value.ProjectId, value.Reference }).IsUnique();
        builder.HasOne<User>().WithMany().HasForeignKey(value => value.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class ProjectRiskConfiguration : IEntityTypeConfiguration<ProjectRisk>
{
    public void Configure(EntityTypeBuilder<ProjectRisk> builder)
    {
        builder.ToTable("ProjectRisks");
        builder.Property(value => value.Reference).HasMaxLength(20).IsRequired();
        builder.Property(value => value.Title).HasMaxLength(1000).IsRequired();
        builder.Property(value => value.Probability).HasConversion<int>();
        builder.Property(value => value.Impact).HasConversion<int>();
        builder.Property(value => value.Type).HasConversion<int>();
        builder.Property(value => value.Cause).HasMaxLength(2000);
        builder.Property(value => value.Mitigation).HasMaxLength(2000);
        builder.Property(value => value.ActionPlan).HasMaxLength(2000);
        builder.Property(value => value.Owner).HasMaxLength(256);
        builder.Property(value => value.Status).HasConversion<int>();
        builder.Property(value => value.Comment).HasMaxLength(4000);
        builder.Ignore(value => value.Criticality);
        builder.HasIndex(value => new { value.ProjectId, value.Reference }).IsUnique();
        builder.HasOne<User>().WithMany().HasForeignKey(value => value.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ProjectAction>().WithMany().HasForeignKey(value => value.ActionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
