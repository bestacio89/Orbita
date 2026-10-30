#nullable enable

using Franz.Common.Mediator.Errors;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Results;
using Orbita.Application.Identity;
using Orbita.Contracts.DTOs.Projects;
using Orbita.Contracts.Persistence;
using Orbita.Contracts.Queries.Projects;
using Orbita.Domain.Entities;

namespace Orbita.Application.Projects.Drafts;

public sealed class ListProjectDraftsQueryHandler(
    IProjectDraftRepository drafts,
    CurrentActorResolver actorResolver)
    : IQueryHandler<ListProjectDraftsQuery, Result<IReadOnlyCollection<ProjectDraftSummaryDto>>>
{
    public async Task<Result<IReadOnlyCollection<ProjectDraftSummaryDto>>> Handle(
        ListProjectDraftsQuery request,
        CancellationToken cancellationToken)
    {
        var actor = await actorResolver.GetOrProvisionAsync(cancellationToken);
        if (actor is null)
        {
            return new Error("Unauthorized", "A validated Entra identity is required.").ToFailure<IReadOnlyCollection<ProjectDraftSummaryDto>>();
        }

        var accessible = await drafts.GetAccessibleByUserAsync(actor.Id, cancellationToken);
        var result = accessible
            .OrderByDescending(draft => draft.ModifiedAt)
            .ThenBy(draft => draft.Id)
            .Select(draft => ToSummary(draft, actor.Id))
            .ToArray();

        return ((IReadOnlyCollection<ProjectDraftSummaryDto>)result).ToResult();
    }

    internal static ProjectDraftSummaryDto ToSummary(ProjectDraft draft, Guid actorUserId) =>
        new(
            draft.Id,
            draft.Client,
            draft.ProjectName,
            draft.Reference,
            draft.Origin,
            draft.Source,
            draft.AuthorUserId,
            draft.CoAuthorUserId,
            draft.CreatedAt,
            draft.ModifiedAt,
            draft.ModifiedByUserId,
            draft.Version,
            draft.AuthorUserId == actorUserId);
}

public sealed class GetProjectDraftByIdQueryHandler(
    IProjectDraftRepository drafts,
    CurrentActorResolver actorResolver)
    : IQueryHandler<GetProjectDraftByIdQuery, Result<ProjectDraftDto>>
{
    public async Task<Result<ProjectDraftDto>> Handle(
        GetProjectDraftByIdQuery request,
        CancellationToken cancellationToken)
    {
        var actor = await actorResolver.GetOrProvisionAsync(cancellationToken);
        if (actor is null)
        {
            return new Error("Unauthorized", "A validated Entra identity is required.").ToFailure<ProjectDraftDto>();
        }

        var draft = await drafts.GetByIdAsync(request.DraftId, cancellationToken);
        if (draft is null || !draft.CanAccess(actor.Id))
        {
            return Error.NotFound("Project draft", request.DraftId).ToFailure<ProjectDraftDto>();
        }

        return new ProjectDraftDto(
            draft.Id,
            draft.Client,
            draft.ProjectName,
            draft.Reference,
            draft.Origin,
            draft.Source,
            draft.Content,
            draft.AuthorUserId,
            draft.CoAuthorUserId,
            draft.CreatedAt,
            draft.ModifiedAt,
            draft.ModifiedByUserId,
            draft.Version,
            draft.AuthorUserId == actor.Id).ToResult();
    }
}
