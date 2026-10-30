using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;
using Orbita.Domain.Enums;

namespace Orbita.Contracts.Commands.Projects;

public sealed record AssignProjectUserCommand(
    Guid ProjectId,
    Guid UserId,
    ProjectRole Role,
    int ExpectedVersion) : ICommand<Result>;
