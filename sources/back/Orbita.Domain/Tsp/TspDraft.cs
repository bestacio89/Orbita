namespace Orbita.Domain.Tsp;

/// <summary>
/// Editable semantic representation of a TSP. It is independent of Excel; import/export adapters translate
/// this model to and from the workbook format so authored and imported TSPs converge on the same project model.
/// </summary>
public sealed class TspDraft
{
    public int FormatVersion { get; set; } = 1;
    public TspProjectSheet Project { get; set; } = new();
    public decimal Discount { get; set; }
    public List<TspProfile> Profiles { get; set; } = [];
    public List<TspTask> Tasks { get; set; } = [];
    /// <summary>Profile identifier → yyyy-MM month → person-days.</summary>
    public Dictionary<string, Dictionary<string, decimal>> MonthlyPlanning { get; set; } = [];
    public List<TspInvoiceMilestone> InvoiceMilestones { get; set; } = [];
    public List<TspPurchase> Purchases { get; set; } = [];
    public List<TspSupplier> Suppliers { get; set; } = [];
    public List<TspExpense> Expenses { get; set; } = [];
    public List<TspMetric> Metrics { get; set; } = [];
    public List<string> Phases { get; set; } = [];
    public List<string> Steps { get; set; } = [];
}

public sealed class TspProjectSheet
{
    public string Client { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public string ClientContact { get; set; } = string.Empty;
    public string Agency { get; set; } = string.Empty;
    public string ServiceLine { get; set; } = string.Empty;
    public string Director { get; set; } = string.Empty;
    public string ServiceLineManager { get; set; } = string.Empty;
    public string SalesRepresentative { get; set; } = string.Empty;
    public string ContractType { get; set; } = "Forfait";
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}

public sealed class TspProfile
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LegalEntity { get; set; } = "FO";
    public decimal DailyRate { get; set; }
    public decimal DailyCostRate { get; set; }
    public decimal? CostRatePercentage { get; set; }
    public decimal Expenses { get; set; }
    public string Collaborator { get; set; } = string.Empty;
}

public sealed class TspTask
{
    public string Id { get; set; } = string.Empty;
    public string AxisCode { get; set; } = string.Empty;
    public string LotCode { get; set; } = string.Empty;
    public string Phase { get; set; } = string.Empty;
    public string Step { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    /// <summary>Profile identifier → sold person-days.</summary>
    public Dictionary<string, decimal> SoldEffortByProfile { get; set; } = [];
    public bool IsDeliverable { get; set; }
    public string Assumption { get; set; } = string.Empty;
    public bool IsSection { get; set; }
}

public sealed class TspInvoiceMilestone
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public string? Month { get; set; }
    public string? AxisCode { get; set; }
}

public sealed class TspPurchase
{
    public string Id { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal UnitCost { get; set; }
    public decimal Quantity { get; set; }
    public decimal ResaleAmount { get; set; }
    public string Comment { get; set; } = string.Empty;
}

public sealed class TspSupplier
{
    public string Id { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public decimal DailyCost { get; set; }
    public decimal PersonDays { get; set; }
    public decimal DailyRate { get; set; }
    public string Comment { get; set; } = string.Empty;
}

public sealed class TspExpense
{
    public string Id { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal UnitCost { get; set; }
    public decimal Quantity { get; set; }
    public decimal RebillAmount { get; set; }
    public string Comment { get; set; } = string.Empty;
}

public sealed class TspMetric
{
    public string Step { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
}

/// <summary>Read-only abstraction over workbook data; concrete XLSX parsing remains outside the Domain layer.</summary>
public interface ITspWorkbook
{
    IReadOnlyList<string> SheetNames { get; }
    ITspWorksheet? FindSheet(string name);
}

public interface ITspWorksheet
{
    string Name { get; }
    int MaxRow { get; }
    int MaxColumn { get; }
    object? GetCell(int row, int column);
}

public sealed record ExcelCellError(string Code);
