#nullable enable

using Franz.Common.Mediator.Errors;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Results;
using Orbita.Application.Identity;
using Orbita.Application.Projects.Drafts;
using Orbita.Contracts.Authorization;
using Orbita.Contracts.Commands.Projects;
using Orbita.Contracts.Persistence;
using Orbita.Domain.Common;
using Orbita.Domain.Entities;

namespace Orbita.Application.Projects.Drafts;

public sealed class InviteDraftCoAuthorCommandHandler(
    IProjectDraftRepository drafts,
    IUserRepository users,
    CurrentActorResolver actorResolver,
    TimeProvider timeProvider)
    : ICommandHandler<InviteDraftCoAuthorCommand, Result>
{
    public async Task<Result> Handle(InviteDraftCoAuthorCommand request, CancellationToken cancellationToken)
    {
        var actor = await actorResolver.GetOrProvisionAsync(cancellationToken);
        if (actor is null)
        {
            return new Error("Unauthorized", "A validated Entra identity is required.").ToFailure<Result>();
        }

        var draft = await drafts.GetByIdAsync(request.DraftId, cancellationToken);
        if (draft is null || !draft.CanAccess(actor.Id))
        {
            return Error.NotFound("Project draft", request.DraftId).ToFailure<Result>();
        }

        if (draft.AuthorUserId != actor.Id)
        {
            return new Error("Forbidden", "Only the draft author can manage co-author access.").ToFailure<Result>();
        }

        if (draft.Version != request.ExpectedVersion)
        {
            return new Error("Conflict", "The draft has changed. Reload it before applying this update.").ToFailure<Result>();
        }

        if (request.CoAuthorUserId == Guid.Empty || request.CoAuthorUserId == actor.Id)
        {
            return Error.Validation("CoAuthorUserId", "The co-author must be a different, valid user.").ToFailure<Result>();
        }

        var coAuthor = await users.GetByIdAsync(request.CoAuthorUserId, cancellationToken);
        if (coAuthor is null || !coAuthor.IsActive)
        {
            return Error.NotFound("Active user", request.CoAuthorUserId).ToFailure<Result>();
        }

        try
        {
            draft.InviteCoAuthor(actor.Id, coAuthor.Id, timeProvider.GetUtcNow());
            await drafts.UpdateAsync(draft, cancellationToken);
            return Result.Success();
        }
        catch (BusinessRuleException exception)
        {
            return Error.Validation("Draft", exception.Message).ToFailure<Result>();
        }
    }
}
