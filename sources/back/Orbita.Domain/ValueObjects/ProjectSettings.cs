using Orbita.Domain.Common;
using Orbita.Domain.Enums;

namespace Orbita.Domain.ValueObjects;

/// <summary>Project-specific planning and alert configuration. Persisted as a dependent value object.</summary>
public sealed class ProjectSettings
{
    public PlanningGrouping Grouping { get; set; } = PlanningGrouping.Axis;

    /// <summary>First planning month in invariant yyyy-MM format.</summary>
    public string? FirstPlanningMonth { get; set; }

    public int MonthCount { get; set; } = 13;
    public int? ActionAlertDays { get; set; }
    public int? MilestoneAlertDays { get; set; }
    public int? WorkshopAlertDays { get; set; }
    public int? DeliverableAlertDays { get; set; }
    public DateOnly? LastProjectCommitteeDate { get; set; }

    /// <summary>Optional newline-delimited governance instances configured for this project.</summary>
    public string? GovernanceInstances { get; set; }

    public static ProjectSettings ForPeriod(DateOnly start, DateOnly? contractualEnd)
    {
        var firstMonth = BusinessCalendar.Month(start);
        var monthCount = contractualEnd is { } end
            ? Math.Max(13, BusinessCalendar.MonthsBetween(firstMonth, BusinessCalendar.Month(end)) + 4)
            : 13;

        return new ProjectSettings
        {
            FirstPlanningMonth = firstMonth,
            MonthCount = monthCount
        };
    }
}
