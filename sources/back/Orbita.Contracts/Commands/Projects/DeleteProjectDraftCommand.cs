using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;

namespace Orbita.Contracts.Commands.Projects;

public sealed record DeleteProjectDraftCommand(
    Guid DraftId,
    int ExpectedVersion) : ICommand<Result>;
