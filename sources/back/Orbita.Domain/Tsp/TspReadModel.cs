namespace Orbita.Domain.Tsp;

/// <summary>
/// Semantic result of reading a commercial TSP workbook. It preserves recognized values, unrecognized project-sheet rows,
/// source-planning provenance and diagnostics without exposing Excel-specific implementation types.
/// </summary>
public sealed class TspReadModel
{
    public TspProjectSheet Project { get; init; } = new();
    public List<TspProjectField> ProjectFields { get; init; } = [];
    public TspPeriod Period { get; init; } = new();
    public decimal Discount { get; init; }
    public bool DiscountWasSpecified { get; init; }
    public decimal? InitialRevenue { get; init; }
    public decimal? ManagementPercentage { get; init; }
    public decimal? ProvisionPercentage { get; init; }
    public List<TspProfile> Profiles { get; init; } = [];
    public List<TspAgencyRate> AgencyRates { get; init; } = [];
    public List<TspReadTask> Tasks { get; init; } = [];
    public TspPhasePlanning PhasePlanning { get; init; } = new();
    public List<TspExpense> Expenses { get; init; } = [];
    public List<TspPurchase> Purchases { get; init; } = [];
    public List<TspSupplier> Suppliers { get; init; } = [];
    public List<TspInvoiceMilestone> InvoiceMilestones { get; init; } = [];
    public List<TspMetric> Metrics { get; init; } = [];
    public TspProfilePlanning SoldPlanning { get; init; } = new();
    /// <summary>Name of the workbook tab used for sold profile planning, or null when no planning tab was available.</summary>
    public string? ProfilePlanningSource { get; init; }
    public TspReadReferenceData References { get; init; } = new();
    public IReadOnlyDictionary<string, string> AxisNames { get; init; } = new Dictionary<string, string>();
    public TspProjectDates Dates { get; init; } = new();
    public TspReadSettings Settings { get; init; } = new();
    public List<string> Warnings { get; init; } = [];
}

public sealed class TspProjectField
{
    public string Label { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    /// <summary>Recognized semantic field key, or null for an unrecognized workbook row.</summary>
    public string? Key { get; init; }
}

public sealed class TspPeriod
{
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
}

public sealed class TspAgencyRate
{
    public int Order { get; init; }
    public string Profile { get; init; } = string.Empty;
    public decimal ParisDailyRate { get; init; }
    public decimal ParisDailyCostRate { get; init; }
    public decimal RegionalDailyRate { get; init; }
    public decimal RegionalDailyCostRate { get; init; }
}

public sealed class TspReadTask
{
    public int? SourceRow { get; init; }
    public string AxisCode { get; init; } = string.Empty;
    public string LotCode { get; init; } = string.Empty;
    public string Phase { get; init; } = string.Empty;
    public string Step { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public Dictionary<string, decimal> SoldEffortByProfile { get; init; } = [];
    public string Assumption { get; init; } = string.Empty;
    public string DeliverableMarker { get; init; } = string.Empty;
    public bool IsSection { get; init; }
    public bool HasDeliverableColumn { get; init; }
}

public sealed class TspPhasePlanning
{
    public List<TspPhasePlanningRow> Rows { get; init; } = [];
    public string? Source { get; init; }
}

public sealed class TspPhasePlanningRow
{
    public string Phase { get; init; } = string.Empty;
    /// <summary>Original cell value retained for diagnostics; calculations use only monthly values.</summary>
    public object? DailyCostRate { get; init; }
    public Dictionary<string, decimal> MonthlyValues { get; init; } = [];
}

public sealed class TspProfilePlanning
{
    public List<string> Months { get; init; } = [];
    /// <summary>Profile identifier → month → sold person-days.</summary>
    public Dictionary<string, Dictionary<string, decimal>> Values { get; init; } = [];
}

public sealed class TspReadReferenceData
{
    public List<string> Phases { get; init; } = [];
    public List<string> Steps { get; init; } = [];
}

public sealed class TspProjectDates
{
    public DateOnly? StartDate { get; init; }
    public DateOnly? ContractualEndDate { get; init; }
    public DateOnly? ForecastEndDate { get; init; }
}

public sealed class TspReadSettings
{
    public string? StartMonth { get; init; }
    public int MonthCount { get; init; } = 13;
    public int AiProfileIndex { get; init; } = -1;
    public string Grouping { get; init; } = "id";
}
