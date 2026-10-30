using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;
using Orbita.Domain.Enums;

namespace Orbita.Contracts.Commands.Projects;

public sealed record CreateProjectCommand(
    string Client,
    string Name,
    string? Reference,
    DateOnly StartDate,
    DateOnly ContractualEndDate,
    string? ContractType) : ICommand<Result<Guid>>;

public sealed record UpdateProjectDatesCommand(
    Guid ProjectId,
    int ExpectedVersion,
    DateOnly? StartDate,
    DateOnly? ContractualEndDate,
    DateOnly? ForecastEndDate) : ICommand<Result>;

public sealed record ArchiveProjectCommand(
    Guid ProjectId,
    int ExpectedVersion) : ICommand<Result>;

public sealed record AssignProjectUserCommand(
    Guid ProjectId,
    Guid UserId,
    ProjectRole Role,
    int ExpectedVersion) : ICommand<Result>;

public sealed record RemoveProjectAssignmentCommand(
    Guid ProjectId,
    Guid UserId,
    int ExpectedVersion) : ICommand<Result>;
