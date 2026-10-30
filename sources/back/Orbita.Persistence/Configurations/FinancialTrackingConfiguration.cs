using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbita.Domain.Entities;

namespace Orbita.Persistence.Configurations;

internal sealed class ActualPurchaseConfiguration : IEntityTypeConfiguration<ActualPurchase>
{
    public void Configure(EntityTypeBuilder<ActualPurchase> builder)
    {
        builder.ToTable("ActualPurchases", table =>
            table.HasCheckConstraint(
                "CK_ActualPurchases_Amounts",
                "\"Quantity\" > 0 AND \"UnitCost\" >= 0 AND \"RebillAmount\" >= 0"));

        builder.HasOne<PurchaseBudgetLine>().WithMany().HasForeignKey(value => value.PurchaseBudgetLineId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(value => value.Description).HasMaxLength(1000);
        builder.Property(value => value.Supplier).HasMaxLength(256);
        builder.Property(value => value.Status).HasConversion<int>();
        builder.Property(value => value.Comment).HasMaxLength(4000);
        builder.HasOne<User>().WithMany().HasForeignKey(value => value.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(value => new { value.ProjectId, value.Date });
    }
}

internal sealed class ActualExpenseConfiguration : IEntityTypeConfiguration<ActualExpense>
{
    public void Configure(EntityTypeBuilder<ActualExpense> builder)
    {
        builder.ToTable("ActualExpenses", table =>
            table.HasCheckConstraint("CK_ActualExpenses_Amount_Positive", "\"Amount\" > 0"));

        builder.HasOne<ExpenseImport>().WithMany().HasForeignKey(value => value.ExpenseImportId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(value => value.Collaborator).HasMaxLength(256);
        builder.Property(value => value.Category).HasConversion<int>();
        builder.Property(value => value.Description).HasMaxLength(1000);
        builder.Property(value => value.Comment).HasMaxLength(4000);
        builder.HasOne<User>().WithMany().HasForeignKey(value => value.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(value => new { value.ProjectId, value.Date });
    }
}

internal sealed class ExpenseImportConfiguration : IEntityTypeConfiguration<ExpenseImport>
{
    public void Configure(EntityTypeBuilder<ExpenseImport> builder)
    {
        builder.ToTable("ExpenseImports");
        builder.HasOne<Project>().WithMany().HasForeignKey(value => value.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(value => value.Source).HasMaxLength(100);
        builder.Property(value => value.FileName).HasMaxLength(260);
        builder.HasOne<User>().WithMany().HasForeignKey(value => value.ImportedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(value => new { value.ProjectId, value.ImportedAt });
    }
}

internal sealed class IndicatorSnapshotConfiguration : IEntityTypeConfiguration<IndicatorSnapshot>
{
    public void Configure(EntityTypeBuilder<IndicatorSnapshot> builder)
    {
        builder.ToTable("IndicatorSnapshots");
        builder.Property(value => value.Kind).HasConversion<int>();
        builder.HasIndex(value => new { value.ProjectId, value.Date, value.Kind }).IsUnique();
    }
}

internal sealed class ProjectNotificationConfiguration : IEntityTypeConfiguration<ProjectNotification>
{
    public void Configure(EntityTypeBuilder<ProjectNotification> builder)
    {
        builder.ToTable("ProjectNotifications");
        builder.Property(value => value.Kind).HasConversion<int>();
        builder.Property(value => value.DeliveryStatus).HasConversion<int>();
        builder.HasOne<User>().WithMany().HasForeignKey(value => value.RecipientUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(value => new { value.RecipientUserId, value.DeliveryStatus, value.TargetDate });
        builder.HasIndex(value => new { value.ProjectId, value.SubjectId, value.Kind });
    }
}
