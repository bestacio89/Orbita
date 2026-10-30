using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;
using Orbita.Domain.Enums;

namespace Orbita.Contracts.Commands.Projects;

public sealed record CreateProjectDraftCommand(
    string Client,
    string ProjectName,
    string? Reference,
    DraftOrigin Origin,
    string Content) : ICommand<Result<Guid>>;
