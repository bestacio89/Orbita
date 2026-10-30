using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Domain.Entities;

namespace Orbita.Persistence.Configurations;

internal sealed class ProjectProfileConfiguration : IEntityTypeConfiguration<ProjectProfile>
{
    public void Configure(EntityTypeBuilder<ProjectProfile> builder)
    {
        builder.ToTable("ProjectProfiles");
        builder.Property(value => value.LegalEntity).HasMaxLength(20);
        builder.Property(value => value.AdditionReason).HasMaxLength(2000);
        builder.HasIndex(value => new { value.ProjectId, value.Rank });
        builder.HasOne<User>().WithMany().HasForeignKey(value => value.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(value => value.AddedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class AgencyRateConfiguration : IEntityTypeConfiguration<AgencyRate>
{
    public void Configure(EntityTypeBuilder<AgencyRate> builder)
    {
        builder.ToTable("AgencyRates");
        builder.Property(value => value.Profile).HasMaxLength(200).IsRequired();
        builder.HasIndex(value => new { value.ProjectId, value.Order });
    }
}

internal sealed class SoldProfilePlanningConfiguration : IEntityTypeConfiguration<SoldProfilePlanning>
{
    public void Configure(EntityTypeBuilder<SoldProfilePlanning> builder)
    {
        builder.ToTable("SoldProfilePlannings");
        builder.Property(value => value.Month).HasMaxLength(7).IsRequired();
        builder.HasOne<ProjectProfile>().WithMany().HasForeignKey(value => value.ProjectProfileId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(value => new { value.ProjectProfileId, value.Month }).IsUnique();
    }
}

internal sealed class SoldPhasePlanningConfiguration : IEntityTypeConfiguration<SoldPhasePlanning>
{
    public void Configure(EntityTypeBuilder<SoldPhasePlanning> builder)
    {
        builder.ToTable("SoldPhasePlannings");
        builder.Property(value => value.Phase).HasMaxLength(50);
        builder.Property(value => value.EnteredDailyCostRate).HasMaxLength(100);
    }
}

internal sealed class SoldPhaseMonthConfiguration : IEntityTypeConfiguration<SoldPhaseMonth>
{
    public void Configure(EntityTypeBuilder<SoldPhaseMonth> builder)
    {
        builder.ToTable("SoldPhaseMonths");
        builder.HasOne<Project>().WithMany().HasForeignKey(value => value.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<SoldPhasePlanning>().WithMany().HasForeignKey(value => value.SoldPhasePlanningId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(value => value.Month).HasMaxLength(7).IsRequired();
        builder.HasIndex(value => new { value.SoldPhasePlanningId, value.Month }).IsUnique();
    }
}

internal sealed class PurchaseBudgetLineConfiguration : IEntityTypeConfiguration<PurchaseBudgetLine>
{
    public void Configure(EntityTypeBuilder<PurchaseBudgetLine> builder)
    {
        builder.ToTable("PurchaseBudgetLines");
        builder.Property(value => value.Description).HasMaxLength(1000);
        builder.Property(value => value.Type).HasMaxLength(100);
        builder.Property(value => value.Comment).HasMaxLength(4000);
        builder.HasIndex(value => new { value.ProjectId, value.Order });
    }
}

internal sealed class ExpenseBudgetLineConfiguration : IEntityTypeConfiguration<ExpenseBudgetLine>
{
    public void Configure(EntityTypeBuilder<ExpenseBudgetLine> builder)
    {
        builder.ToTable("ExpenseBudgetLines");
        builder.Property(value => value.Description).HasMaxLength(1000);
        builder.Property(value => value.Comment).HasMaxLength(4000);
        builder.HasIndex(value => new { value.ProjectId, value.Order });
    }
}

internal sealed class ProjectSupplierConfiguration : IEntityTypeConfiguration<ProjectSupplier>
{
    public void Configure(EntityTypeBuilder<ProjectSupplier> builder)
    {
        builder.ToTable("ProjectSuppliers");
        builder.Property(value => value.Description).HasMaxLength(1000);
        builder.Property(value => value.Company).HasMaxLength(256);
        builder.Property(value => value.Comment).HasMaxLength(4000);
        builder.HasIndex(value => new { value.ProjectId, value.Order });
    }
}

internal sealed class ProjectMetricConfiguration : IEntityTypeConfiguration<ProjectMetric>
{
    public void Configure(EntityTypeBuilder<ProjectMetric> builder)
    {
        builder.ToTable("ProjectMetrics");
        builder.Property(value => value.Step).HasMaxLength(200).IsRequired();
        builder.HasIndex(value => new { value.ProjectId, value.Order });
    }
}

internal sealed class InvoiceMilestoneConfiguration : IEntityTypeConfiguration<InvoiceMilestone>
{
    public void Configure(EntityTypeBuilder<InvoiceMilestone> builder)
    {
        builder.ToTable("InvoiceMilestones");
        builder.Property(value => value.Name).HasMaxLength(200).IsRequired();
        builder.Property(value => value.TspMonth).HasMaxLength(7);
        builder.Property(value => value.Comment).HasMaxLength(4000);
        builder.Property(value => value.TriggerAxisCode).HasMaxLength(50);
        builder.HasIndex(value => new { value.ProjectId, value.Number }).IsUnique();
    }
}

internal sealed class InvoiceMilestonePhaseConfiguration : IEntityTypeConfiguration<InvoiceMilestonePhase>
{
    public void Configure(EntityTypeBuilder<InvoiceMilestonePhase> builder)
    {
        builder.ToTable("InvoiceMilestonePhases");
        builder.HasOne<Project>().WithMany().HasForeignKey(value => value.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<InvoiceMilestone>().WithMany().HasForeignKey(value => value.InvoiceMilestoneId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<ProjectPhase>().WithMany().HasForeignKey(value => value.PhaseId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(value => new { value.InvoiceMilestoneId, value.PhaseId }).IsUnique();
        builder.HasIndex(value => value.PhaseId).IsUnique();
    }
}
