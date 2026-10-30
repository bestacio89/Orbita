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

public sealed class RemoveDraftCoAuthorCommandHandler(
    IProjectDraftRepository drafts,
    CurrentActorResolver actorResolver,
    TimeProvider timeProvider)
    : ICommandHandler<RemoveDraftCoAuthorCommand, Result>
{
    public async Task<Result> Handle(RemoveDraftCoAuthorCommand request, CancellationToken cancellationToken)
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

        try
        {
            draft.InviteCoAuthor(actor.Id, null, timeProvider.GetUtcNow());
            await drafts.UpdateAsync(draft, cancellationToken);
            return Result.Success();
        }
        catch (BusinessRuleException exception)
        {
            return Error.Validation("Draft", exception.Message).ToFailure<Result>();
        }
    }
}

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
