using Orbita.Domain.Enums;

namespace Orbita.Contracts.DTOs.Projects;

public sealed record ProjectSummaryDto(
    Guid Id,
    string Client,
    string Name,
    string? Reference,
    ProjectStatus Status,
    DateOnly? StartDate,
    DateOnly? ContractualEndDate,
    DateOnly? ForecastEndDate,
    int Version);

public sealed record ProjectDetailsDto(
    Guid Id,
    string Client,
    string Name,
    string? Reference,
    string UniqueKey,
    string? Agency,
    string? ServiceLine,
    string? Director,
    string? ServiceLineManager,
    string? SalesRepresentative,
    string? ClientContact,
    string? ContractType,
    ProjectStatus Status,
    ProjectOrigin Origin,
    DateOnly? StartDate,
    DateOnly? ContractualEndDate,
    DateOnly? ForecastEndDate,
    decimal Discount,
    decimal? InitialRevenue,
    int Version,
    ProjectSettingsDto Settings,
    IReadOnlyCollection<ProjectAssignmentDto> Assignments);

public sealed record ProjectSettingsDto(
    PlanningGrouping Grouping,
    string? FirstPlanningMonth,
    int MonthCount,
    int? ActionAlertDays,
    int? MilestoneAlertDays,
    int? WorkshopAlertDays,
    int? DeliverableAlertDays,
    DateOnly? LastProjectCommitteeDate);

public sealed record ProjectAssignmentDto(
    Guid UserId,
    ProjectRole Role,
    DateTimeOffset AssignedAt,
    Guid? GrantedByUserId);

public sealed record CurrentUserDto(
    int ContractVersion,
    Guid UserId,
    Guid TenantId,
    Guid ObjectId,
    string DisplayName,
    string Email,
    IReadOnlyCollection<string> GlobalRoles,
    IReadOnlyCollection<string> Capabilities);

public sealed record ProjectDraftDto(
    Guid Id,
    string Client,
    string ProjectName,
    string? Reference,
    DraftOrigin Origin,
    Guid AuthorUserId,
    Guid? CoAuthorUserId,
    DateTimeOffset CreatedAt,
    DateTimeOffset ModifiedAt,
    int Version);
