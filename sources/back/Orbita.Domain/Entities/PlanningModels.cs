using Orbita.Domain.Enums;

namespace Orbita.Domain.Entities;

/// <summary>Forecast workload for a profile and month. Sold planning remains immutable.</summary>
public sealed class ForecastProfile : ProjectEntity
{
    public Guid ProjectProfileId { get; set; }
    public string Month { get; set; } = string.Empty;
    public decimal PersonDays { get; set; }
}

/// <summary>Forecast dates or labels attached to a planning row, identified by a stable row key.</summary>
public sealed class PlanningDate : ProjectEntity
{
    public string Key { get; set; } = string.Empty;
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Name { get; set; }
}

/// <summary>Project milestone represented by a date or a date range.</summary>
public sealed class ProjectMilestone : ProjectEntity
{
    public string Name { get; set; } = string.Empty;
    public MilestoneType Type { get; set; } = MilestoneType.Date;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool RequiresConfirmation { get; set; }
}

/// <summary>Planning row visibility/highlight preference. Hiding a row never removes its underlying delivery data.</summary>
public sealed class PlanningElement : ProjectEntity
{
    public PlanningElementKind Kind { get; set; }
    public string Key { get; set; } = string.Empty;
}

/// <summary>Immutable project planning snapshot used to compare forecast drift over time.</summary>
public sealed class ReferencePlan : ProjectEntity
{
    public DateOnly SnapshotDate { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? FrozenByUserId { get; set; }
    public List<ReferencePlanTaskLine> Lines { get; set; } = [];
}

/// <summary>Task values frozen into a reference plan.</summary>
public sealed class ReferencePlanTaskLine : ProjectEntity
{
    public Guid ReferencePlanId { get; set; }
    public Guid TaskId { get; set; }
    public DateOnly? PlannedStartDate { get; set; }
    public DateOnly? PlannedEndDate { get; set; }
    public decimal PlannedEffort { get; set; }
    public string? VarianceCause { get; set; }
    public string? VarianceExplanation { get; set; }
}

/// <summary>Planning dates for a row frozen with a reference plan.</summary>
public sealed class ReferencePlanDate : ProjectEntity
{
    public Guid ReferencePlanId { get; set; }
    public string Key { get; set; } = string.Empty;
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}
