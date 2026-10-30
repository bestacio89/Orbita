using Orbita.Domain.Enums;

namespace Orbita.Contracts.DTOs.Projects;

public sealed record ProjectSettingsDto(
    PlanningGrouping Grouping,
    string? FirstPlanningMonth,
    int MonthCount,
    int? ActionAlertDays,
    int? MilestoneAlertDays,
    int? WorkshopAlertDays,
    int? DeliverableAlertDays,
    DateOnly? LastProjectCommitteeDate);
