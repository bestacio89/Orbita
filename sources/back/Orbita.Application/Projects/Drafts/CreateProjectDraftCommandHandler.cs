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

public sealed class CreateProjectDraftCommandHandler(
    IProjectDraftRepository drafts,
    CurrentActorResolver actorResolver,
    ICurrentUserContext currentUser,
    TimeProvider timeProvider)
    : ICommandHandler<CreateProjectDraftCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProjectDraftCommand request, CancellationToken cancellationToken)
    {
        if (!GlobalRoleCapabilityMatrix.HasCapability(currentUser.GlobalRoles, CapabilityNames.DraftWriteOwn))
        {
            return new Error("Forbidden", "The current identity cannot create project drafts.").ToFailure<Guid>();
        }

        var actor = await actorResolver.GetOrProvisionAsync(cancellationToken);
        if (actor is null)
        {
            return "A validated Entra identity is required.".ToFailure<Guid>();
        }

        try
        {
            var payload = TspDraftPayload.ForCreation(
                request.Content,
                request.Client,
                request.ProjectName,
                request.Reference);

            var draft = ProjectDraft.Create(
                payload.Client,
                payload.ProjectName,
                payload.Reference,
                request.Origin,
                payload.Content,
                actor.Id,
                timeProvider.GetUtcNow());

            await drafts.AddAsync(draft, cancellationToken);
            return draft.Id.ToResult();
        }
        catch (BusinessRuleException exception)
        {
            return Error.Validation("Draft", exception.Message).ToFailure<Guid>();
        }
    }
}
