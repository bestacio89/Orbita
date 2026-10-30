#nullable enable

using Franz.Common.Mediator.Errors;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Results;
using Orbita.Application.Identity;
using Orbita.Contracts.Authorization;
using Orbita.Contracts.DTOs.Projects;
using Orbita.Contracts.Persistence;
using Orbita.Contracts.Queries.Projects;
using Orbita.Domain.Entities;
using Orbita.Domain.Services;

namespace Orbita.Application.Projects.Queries;

public sealed class ListProjectsQueryHandler(
    IProjectRepository projects,
    CurrentActorResolver actorResolver,
    ICurrentUserContext currentUser)
    : IQueryHandler<ListProjectsQuery, Result<IReadOnlyCollection<ProjectSummaryDto>>>
{
    public async Task<Result<IReadOnlyCollection<ProjectSummaryDto>>> Handle(ListProjectsQuery request, CancellationToken cancellationToken)
    {
        if (request.Page < 1 || request.PageSize is < 1 or > 100)
        {
            return Error.Validation("Pagination", "Page must be positive and page size must be between 1 and 100.").ToFailure<IReadOnlyCollection<ProjectSummaryDto>>();
        }

        var actor = await actorResolver.GetOrProvisionAsync(cancellationToken);
        if (actor is null)
        {
            return new Error("Unauthorized", "A validated Entra identity is required.").ToFailure<IReadOnlyCollection<ProjectSummaryDto>>();
        }

        if (!GlobalRoleCapabilityMatrix.HasCapability(currentUser.GlobalRoles, CapabilityNames.ProjectRead))
        {
            return new Error("Forbidden", "The current identity cannot read projects.").ToFailure<IReadOnlyCollection<ProjectSummaryDto>>();
        }

        var accessible = await projects.GetAccessibleByUserAsync(actor.Id, cancellationToken);

        var filtered = accessible
            .Where(project => string.IsNullOrWhiteSpace(request.Search) ||
                project.Client.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                project.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                (project.Reference?.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ?? false))
            .OrderBy(project => project.Client, StringComparer.OrdinalIgnoreCase)
            .ThenBy(project => project.Name, StringComparer.OrdinalIgnoreCase)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(ProjectDtoMapper.ToSummary)
            .ToArray();

        return ((IReadOnlyCollection<ProjectSummaryDto>)filtered).ToResult();
    }
}
