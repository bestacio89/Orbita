using Orbita.Domain.Enums;

namespace Orbita.Domain.Entities;

/// <summary>Actual purchase ordered for a project, linked to a TSP budget line when applicable.</summary>
public sealed class ActualPurchase : ProjectEntity
{
    public Guid? PurchaseBudgetLineId { get; set; }
    public DateOnly Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Supplier { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal RebillAmount { get; set; }
    public PurchaseStatus Status { get; set; } = PurchaseStatus.Ordered;
    public string Comment { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? CreatedByUserId { get; set; }
}

/// <summary>Actual expense recorded against a project.</summary>
public sealed class ActualExpense : ProjectEntity
{
    public Guid? ExpenseImportId { get; set; }
    public DateOnly Date { get; set; }
    public string Collaborator { get; set; } = string.Empty;
    public ExpenseCategory Category { get; set; } = ExpenseCategory.Other;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsRebillable { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? CreatedByUserId { get; set; }
}

/// <summary>Batch import metadata for actual expenses.</summary>
public sealed class ExpenseImport : ProjectEntity
{
    public string Source { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public DateTimeOffset ImportedAt { get; set; }
    public Guid? ImportedByUserId { get; set; }
    public int RowCount { get; set; }
}

/// <summary>Financial and delivery indicators frozen for committee-to-committee comparison.</summary>
public sealed class IndicatorSnapshot : ProjectEntity
{
    public DateOnly Date { get; set; }
    public IndicatorSnapshotKind Kind { get; set; }
    public decimal PlannedEffort { get; set; }
    public decimal ActualEffort { get; set; }
    public decimal RemainingEffort { get; set; }
    public decimal ForecastEffort { get; set; }
    public decimal FinalCost { get; set; }
    public decimal Margin { get; set; }
    public decimal NetMargin { get; set; }
    public decimal NetMarginRate { get; set; }
    public DateOnly? ForecastEndDate { get; set; }
}

/// <summary>Delivery notification addressed to one application user.</summary>
public sealed class ProjectNotification : ProjectEntity
{
    public Guid RecipientUserId { get; set; }
    public NotificationKind Kind { get; set; }
    public Guid SubjectId { get; set; }
    public DateOnly TargetDate { get; set; }
    public DateTimeOffset? SentAt { get; private set; }
    public NotificationDeliveryStatus DeliveryStatus { get; private set; } = NotificationDeliveryStatus.Pending;

    public void MarkSent(DateTimeOffset sentAt)
    {
        SentAt = sentAt;
        DeliveryStatus = NotificationDeliveryStatus.Sent;
    }

    public void MarkFailed()
    {
        SentAt = null;
        DeliveryStatus = NotificationDeliveryStatus.Failed;
    }

    public void ResetDelivery()
    {
        SentAt = null;
        DeliveryStatus = NotificationDeliveryStatus.Pending;
    }
}
