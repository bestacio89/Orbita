using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;

namespace Orbita.Contracts.Commands.Projects;

public sealed record RemoveProjectAssignmentCommand(
    Guid ProjectId,
    Guid UserId,
    int ExpectedVersion) : ICommand<Result>;
