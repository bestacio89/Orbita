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

public sealed class GetProjectByIdQueryHandler(
    IProjectRepository projects,
    CurrentActorResolver actorResolver,
    ICurrentUserContext currentUser)
    : IQueryHandler<GetProjectByIdQuery, Result<ProjectDetailsDto>>
{
    public async Task<Result<ProjectDetailsDto>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await projects.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null)
        {
            return Error.NotFound("Project", request.ProjectId).ToFailure<ProjectDetailsDto>();
        }

        var actor = await actorResolver.GetOrProvisionAsync(cancellationToken);
        var canRead = GlobalRoleCapabilityMatrix.HasCapability(currentUser.GlobalRoles, CapabilityNames.ProjectRead);
        if (actor is null || !canRead || !ProjectAccessPolicy.CanAccessProject(project, actor.Id))
        {
            return Error.NotFound("Project", request.ProjectId).ToFailure<ProjectDetailsDto>();
        }

        return ProjectDtoMapper.ToDetails(project).ToResult();
    }
}
