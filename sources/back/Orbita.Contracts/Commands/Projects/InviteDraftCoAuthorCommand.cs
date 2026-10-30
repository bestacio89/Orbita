using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;

namespace Orbita.Contracts.Commands.Projects;

public sealed record InviteDraftCoAuthorCommand(
    Guid DraftId,
    Guid CoAuthorUserId,
    int ExpectedVersion) : ICommand<Result>;
