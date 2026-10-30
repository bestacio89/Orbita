using Franz.Common.Business.Domain;
using Orbita.Domain.Common;
using Orbita.Domain.Enums;

namespace Orbita.Domain.ValueObjects;

/// <summary>Immutable project planning and alert configuration value object.</summary>
public sealed class ProjectSettings : ValueObject<ProjectSettings>
{
    public PlanningGrouping Grouping { get; init; } = PlanningGrouping.Axis;
    public string? FirstPlanningMonth { get; init; }
    public int MonthCount { get; init; } = 13;
    public int? ActionAlertDays { get; init; }
    public int? MilestoneAlertDays { get; init; }
    public int? WorkshopAlertDays { get; init; }
    public int? DeliverableAlertDays { get; init; }
    public DateOnly? LastProjectCommitteeDate { get; init; }
    public string? GovernanceInstances { get; init; }

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

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Grouping;
        yield return FirstPlanningMonth;
        yield return MonthCount;
        yield return ActionAlertDays;
        yield return MilestoneAlertDays;
        yield return WorkshopAlertDays;
        yield return DeliverableAlertDays;
        yield return LastProjectCommitteeDate;
        yield return GovernanceInstances;
    }
}
