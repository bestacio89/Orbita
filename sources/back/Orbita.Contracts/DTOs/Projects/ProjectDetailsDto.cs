using Orbita.Domain.Enums;

namespace Orbita.Contracts.DTOs.Projects;

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
