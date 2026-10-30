using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;

namespace Orbita.Contracts.Commands.Projects;

public sealed record UpdateProjectDraftContentCommand(
    Guid DraftId,
    string Content,
    int ExpectedVersion) : ICommand<Result>;
