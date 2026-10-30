using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Domain.Entities;

namespace Orbita.Persistence.Configurations;

internal sealed class ForecastProfileConfiguration : IEntityTypeConfiguration<ForecastProfile>
{
    public void Configure(EntityTypeBuilder<ForecastProfile> builder)
    {
        builder.ToTable("ForecastProfiles");
        builder.Property(value => value.Month).HasMaxLength(7).IsRequired();
        builder.HasOne<ProjectProfile>().WithMany().HasForeignKey(value => value.ProjectProfileId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(value => new { value.ProjectProfileId, value.Month }).IsUnique();
    }
}

internal sealed class PlanningDateConfiguration : IEntityTypeConfiguration<PlanningDate>
{
    public void Configure(EntityTypeBuilder<PlanningDate> builder)
    {
        builder.ToTable("PlanningDates");
        builder.Property(value => value.Key).HasMaxLength(200).IsRequired();
        builder.Property(value => value.Name).HasMaxLength(200);
        builder.HasIndex(value => new { value.ProjectId, value.Key }).IsUnique();
    }
}

internal sealed class ProjectMilestoneConfiguration : IEntityTypeConfiguration<ProjectMilestone>
{
    public void Configure(EntityTypeBuilder<ProjectMilestone> builder)
    {
        builder.ToTable("ProjectMilestones");
        builder.Property(value => value.Name).HasMaxLength(200).IsRequired();
    }
}

internal sealed class PlanningElementConfiguration : IEntityTypeConfiguration<PlanningElement>
{
    public void Configure(EntityTypeBuilder<PlanningElement> builder)
    {
        builder.ToTable("PlanningElements");
        builder.Property(value => value.Key).HasMaxLength(200).IsRequired();
        builder.HasIndex(value => new { value.ProjectId, value.Kind, value.Key }).IsUnique();
    }
}

internal sealed class ReferencePlanConfiguration : IEntityTypeConfiguration<ReferencePlan>
{
    public void Configure(EntityTypeBuilder<ReferencePlan> builder)
    {
        builder.ToTable("ReferencePlans");
        builder.Property(value => value.Name).HasMaxLength(200);
        builder.HasMany(value => value.Lines).WithOne().HasForeignKey(line => line.ReferencePlanId);
        builder.HasOne<User>().WithMany().HasForeignKey(value => value.FrozenByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class ReferencePlanTaskLineConfiguration : IEntityTypeConfiguration<ReferencePlanTaskLine>
{
    public void Configure(EntityTypeBuilder<ReferencePlanTaskLine> builder)
    {
        builder.ToTable("ReferencePlanTaskLines");
        builder.HasOne<WorkTask>().WithMany().HasForeignKey(line => line.TaskId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(line => new { line.ReferencePlanId, line.TaskId }).IsUnique();
        builder.Property(line => line.VarianceCause).HasMaxLength(30);
        builder.Property(line => line.VarianceExplanation).HasMaxLength(1000);
    }
}

internal sealed class ReferencePlanDateConfiguration : IEntityTypeConfiguration<ReferencePlanDate>
{
    public void Configure(EntityTypeBuilder<ReferencePlanDate> builder)
    {
        builder.ToTable("ReferencePlanDates");
        builder.HasOne<Project>().WithMany().HasForeignKey(value => value.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<ReferencePlan>().WithMany().HasForeignKey(value => value.ReferencePlanId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(value => value.Key).HasMaxLength(200).IsRequired();
        builder.HasIndex(value => new { value.ReferencePlanId, value.Key }).IsUnique();
    }
}

internal sealed class AbsenceConfiguration : IEntityTypeConfiguration<Absence>
{
    public void Configure(EntityTypeBuilder<Absence> builder)
    {
        builder.ToTable("Absences");
        builder.Property(value => value.PersonKey).HasMaxLength(200).IsRequired();
        builder.Property(value => value.PersonName).HasMaxLength(200).IsRequired();
        builder.Property(value => value.Period).HasConversion<int>();
        builder.HasIndex(value => new { value.PersonKey, value.Date }).IsUnique();
        builder.HasIndex(value => value.UserId);
        builder.HasOne<User>().WithMany().HasForeignKey(value => value.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(value => value.RecordedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class ProjectAddedPersonConfiguration : IEntityTypeConfiguration<ProjectAddedPerson>
{
    public void Configure(EntityTypeBuilder<ProjectAddedPerson> builder)
    {
        builder.ToTable("ProjectAddedPeople");
        builder.HasOne<Project>().WithMany().HasForeignKey(value => value.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(value => value.Name).HasMaxLength(200).IsRequired();
        builder.Property(value => value.Key).HasMaxLength(200).IsRequired();
        builder.HasIndex(value => new { value.ProjectId, value.Key }).IsUnique();
    }
}
