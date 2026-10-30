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

public sealed class DeleteProjectDraftCommandHandler(
    IProjectDraftRepository drafts,
    CurrentActorResolver actorResolver)
    : ICommandHandler<DeleteProjectDraftCommand, Result>
{
    public async Task<Result> Handle(DeleteProjectDraftCommand request, CancellationToken cancellationToken)
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
            return new Error("Forbidden", "Only the draft author can delete this draft.").ToFailure<Result>();
        }

        if (draft.Version != request.ExpectedVersion)
        {
            return new Error("Conflict", "The draft has changed. Reload it before applying this update.").ToFailure<Result>();
        }

        await drafts.DeleteAsync(draft, cancellationToken);
        return Result.Success();
    }
}
