using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Domain.Entities;

namespace Orbita.Persistence.Configurations;

internal sealed class WorkTaskConfiguration : IEntityTypeConfiguration<WorkTask>
{
    public void Configure(EntityTypeBuilder<WorkTask> builder)
    {
        builder.ToTable("WorkTasks", table =>
            table.HasCheckConstraint(
                "CK_WorkTasks_Progress",
                "\"Progress\" IS NULL OR (\"Progress\" >= 0 AND \"Progress\" <= 1)"));

        builder.HasIndex(task => new { task.ProjectId, task.Reference }).IsUnique();
        builder.HasIndex(task => new { task.ProjectId, task.Position });
        builder.Property(task => task.Reference).HasMaxLength(20).IsRequired();
        builder.Property(task => task.TspAxisCode).HasMaxLength(50);
        builder.Property(task => task.TspLot).HasMaxLength(50);
        builder.Property(task => task.TspPhase).HasMaxLength(50);
        builder.Property(task => task.TspStep).HasMaxLength(50);
        builder.Property(task => task.TspTitle).HasMaxLength(1000);
        builder.Property(task => task.Title).HasMaxLength(1000);
        builder.Property(task => task.Assumption).HasMaxLength(4000);
        builder.Property(task => task.Comment).HasMaxLength(4000);
        builder.Property(task => task.DeliverableMarker).HasMaxLength(50);
        builder.Property(task => task.AiOptimizationType).HasMaxLength(100);
        builder.Property(task => task.AiComment).HasMaxLength(4000);
        builder.Ignore(task => task.IsCancelled);

        builder.HasOne<ProjectAxis>().WithMany().HasForeignKey(task => task.AxisId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ProjectPhase>().WithMany().HasForeignKey(task => task.PhaseId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ProjectTaskType>().WithMany().HasForeignKey(task => task.TaskTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<InvoiceMilestone>().WithMany().HasForeignKey(task => task.InvoiceMilestoneId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(task => task.Efforts).WithOne().HasForeignKey(effort => effort.TaskId);
        builder.HasMany(task => task.TimeEntries).WithOne().HasForeignKey(entry => entry.TaskId);
        builder.HasMany(task => task.ScopeChanges).WithOne().HasForeignKey(change => change.TaskId);
    }
}

internal sealed class TaskEffortConfiguration : IEntityTypeConfiguration<TaskEffort>
{
    public void Configure(EntityTypeBuilder<TaskEffort> builder)
    {
        builder.ToTable("TaskEfforts", table =>
            table.HasCheckConstraint(
                "CK_TaskEfforts_NonNegative",
                "\"SoldEffort\" >= 0 AND \"AdditionalEffort\" >= 0"));

        builder.HasOne<WorkTask>().WithMany().HasForeignKey(effort => effort.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<ProjectProfile>().WithMany().HasForeignKey(effort => effort.ProjectProfileId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(effort => new { effort.TaskId, effort.ProjectProfileId }).IsUnique();
        builder.Property(effort => effort.AdditionReason).HasMaxLength(2000);
        builder.HasOne<User>().WithMany().HasForeignKey(effort => effort.AddedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.ToTable("TimeEntries", table =>
            table.HasCheckConstraint("CK_TimeEntries_PersonDays_Positive", "\"PersonDays\" > 0"));

        builder.HasOne<WorkTask>().WithMany().HasForeignKey(entry => entry.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<ProjectProfile>().WithMany().HasForeignKey(entry => entry.ProjectProfileId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(entry => new { entry.TaskId, entry.ProjectProfileId, entry.WorkDate }).IsUnique();
        builder.HasIndex(entry => new { entry.ProjectProfileId, entry.WorkDate });
        builder.HasOne<User>().WithMany().HasForeignKey(entry => entry.RecordedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class ScopeChangeConfiguration : IEntityTypeConfiguration<ScopeChange>
{
    public void Configure(EntityTypeBuilder<ScopeChange> builder)
    {
        builder.ToTable("ScopeChanges");
        builder.HasOne<WorkTask>().WithMany().HasForeignKey(change => change.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<ProjectProfile>().WithMany().HasForeignKey(change => change.ProjectProfileId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ProjectProfile>().WithMany().HasForeignKey(change => change.SourceProjectProfileId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(change => change.ActorUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ProjectDecision>().WithMany().HasForeignKey(change => change.DecisionId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(change => new { change.TaskId, change.OccurredAt });
        builder.Property(change => change.Reason).HasMaxLength(2000);
    }
}
