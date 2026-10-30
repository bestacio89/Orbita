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
