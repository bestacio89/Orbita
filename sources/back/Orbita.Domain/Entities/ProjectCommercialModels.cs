using Orbita.Domain.Common;

namespace Orbita.Domain.Entities;

/// <summary>A commercial staffing profile sold for a project or added during delivery.</summary>
public sealed class ProjectProfile : ProjectEntity
{
    public int Rank { get; set; }
    public string Name { get; set; } = string.Empty;
    /// <summary>Business entity code, for example FO, BO, CESSION, or ACHAT.</summary>
    public string LegalEntity { get; set; } = "FO";
    public decimal DailyRate { get; set; }
    public decimal DailyCostRate { get; set; }
    public decimal? CostRatePercentage { get; set; }
    public decimal Expenses { get; set; }
    public string Collaborator { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public Enums.ElementOrigin Origin { get; set; } = Enums.ElementOrigin.Tsp;
    public DateTimeOffset? AddedAt { get; set; }
    public Guid? AddedByUserId { get; set; }
    public string? AdditionReason { get; set; }
    public bool IsAiAgent { get; set; }
}

/// <summary>Agency reference rates imported from the commercial workbook.</summary>
public sealed class AgencyRate : ProjectEntity
{
    public int Order { get; set; }
    public string Profile { get; set; } = string.Empty;
    public decimal ParisDailyRate { get; set; }
    public decimal ParisDailyCostRate { get; set; }
    public decimal RegionalDailyRate { get; set; }
    public decimal RegionalDailyCostRate { get; set; }
}

/// <summary>Sold monthly workload for one project profile. Unique by project, profile and month.</summary>
public sealed class SoldProfilePlanning : ProjectEntity
{
    public Guid ProjectProfileId { get; set; }
    public string Month { get; set; } = string.Empty;
    public decimal PersonDays { get; set; }
}

/// <summary>Sold workload grouped by delivery phase.</summary>
public sealed class SoldPhasePlanning : ProjectEntity
{
    public string Phase { get; set; } = string.Empty;
    public int Order { get; set; }
    public string? EnteredDailyCostRate { get; set; }
}

/// <summary>Monthly workload belonging to a sold phase plan.</summary>
public sealed class SoldPhaseMonth : ProjectEntity
{
    public Guid SoldPhasePlanningId { get; set; }
    public string Month { get; set; } = string.Empty;
    public decimal PersonDays { get; set; }
}

/// <summary>Purchase budget line imported from the TSP.</summary>
public sealed class PurchaseBudgetLine : ProjectEntity
{
    public int Order { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal UnitCost { get; set; }
    public decimal Quantity { get; set; }
    public decimal ResaleAmount { get; set; }
    public string Comment { get; set; } = string.Empty;
}

/// <summary>Expense budget line imported from the TSP.</summary>
public sealed class ExpenseBudgetLine : ProjectEntity
{
    public int Order { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal UnitCost { get; set; }
    public decimal Quantity { get; set; }
    public decimal RebillAmount { get; set; }
    public string Comment { get; set; } = string.Empty;
}

/// <summary>External supplier planned in the TSP.</summary>
public sealed class ProjectSupplier : ProjectEntity
{
    public int Order { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public decimal DailyCost { get; set; }
    public decimal PersonDays { get; set; }
    public decimal DailyRate { get; set; }
    public string Comment { get; set; } = string.Empty;
}

/// <summary>Expected workload distribution by delivery step.</summary>
public sealed class ProjectMetric : ProjectEntity
{
    public int Order { get; set; }
    public string Step { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
}

/// <summary>Commercial invoice milestone and its billing/payment lifecycle.</summary>
public sealed class InvoiceMilestone : ProjectEntity
{
    public int Number { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public string? TspMonth { get; set; }
    public DateOnly? PlannedDate { get; set; }
    public DateOnly? InvoicedOn { get; private set; }
    public DateOnly? PaidOn { get; private set; }
    /// <summary>Amount frozen when the milestone is first invoiced.</summary>
    public decimal? InvoicedAmount { get; private set; }
    public string Comment { get; set; } = string.Empty;
    public string? TriggerAxisCode { get; set; }

    public bool IsSelectable => InvoicedOn is null;

    public void Invoice(DateOnly invoiceDate, decimal amount, DateOnly today)
    {
        if (invoiceDate > today)
        {
            throw new BusinessRuleException("An invoice date cannot be in the future.");
        }

        if (amount < 0)
        {
            throw new BusinessRuleException("The invoiced amount cannot be negative.");
        }

        if (PaidOn is { } paidOn && paidOn < invoiceDate)
        {
            throw new BusinessRuleException("The invoice date cannot be later than its payment date.");
        }

        if (InvoicedOn is null)
        {
            InvoicedAmount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
        }

        InvoicedOn = invoiceDate;
    }

    public void RecordPayment(DateOnly? paymentDate, DateOnly today)
    {
        if (paymentDate is null)
        {
            PaidOn = null;
            return;
        }

        if (InvoicedOn is null)
        {
            throw new BusinessRuleException("The invoice must be issued before recording a payment.");
        }

        if (paymentDate > today)
        {
            throw new BusinessRuleException("A payment date cannot be in the future.");
        }

        if (paymentDate < InvoicedOn)
        {
            throw new BusinessRuleException("A payment date cannot precede the invoice date.");
        }

        PaidOn = paymentDate;
    }

    public void CancelInvoice()
    {
        InvoicedOn = null;
        PaidOn = null;
        InvoicedAmount = null;
    }
}

/// <summary>Links an invoice milestone to a delivery phase.</summary>
public sealed class InvoiceMilestonePhase : ProjectEntity
{
    public Guid InvoiceMilestoneId { get; set; }
    public Guid PhaseId { get; set; }
}
