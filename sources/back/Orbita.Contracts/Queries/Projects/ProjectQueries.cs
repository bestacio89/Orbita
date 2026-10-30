using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;
using Orbita.Contracts.DTOs.Projects;

namespace Orbita.Contracts.Queries.Projects;

public sealed record GetProjectByIdQuery(Guid ProjectId)
    : IQuery<Result<ProjectDetailsDto>>;

public sealed record ListProjectsQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 50)
    : IQuery<Result<IReadOnlyCollection<ProjectSummaryDto>>>;

public sealed record GetCurrentUserQuery
    : IQuery<Result<CurrentUserDto>>;

public sealed record GetProjectDraftByIdQuery(Guid DraftId)
    : IQuery<Result<ProjectDraftDto>>;
