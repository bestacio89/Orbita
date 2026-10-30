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

public sealed class UpdateProjectDraftContentCommandHandler(
    IProjectDraftRepository drafts,
    CurrentActorResolver actorResolver,
    TimeProvider timeProvider)
    : ICommandHandler<UpdateProjectDraftContentCommand, Result>
{
    public async Task<Result> Handle(UpdateProjectDraftContentCommand request, CancellationToken cancellationToken)
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

        if (draft.Version != request.ExpectedVersion)
        {
            return new Error("Conflict", "The draft has changed. Reload it before applying this update.").ToFailure<Result>();
        }

        try
        {
            var payload = TspDraftPayload.ForUpdate(request.Content);
            draft.UpdateContent(
                actor.Id,
                payload.Client,
                payload.ProjectName,
                payload.Reference,
                payload.Content,
                timeProvider.GetUtcNow());

            await drafts.UpdateAsync(draft, cancellationToken);
            return Result.Success();
        }
        catch (BusinessRuleException exception)
        {
            return Error.Validation("Draft", exception.Message).ToFailure<Result>();
        }
    }
}
