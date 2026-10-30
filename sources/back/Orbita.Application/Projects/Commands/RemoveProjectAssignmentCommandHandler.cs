#nullable enable

using Franz.Common.Mediator.Errors;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Results;
using Orbita.Application.Identity;
using Orbita.Contracts.Authorization;
using Orbita.Contracts.Commands.Projects;
using Orbita.Contracts.Persistence;
using Orbita.Domain.Common;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;
using Orbita.Domain.Services;

namespace Orbita.Application.Projects.Commands;

public sealed class RemoveProjectAssignmentCommandHandler(
    IProjectRepository projects,
    CurrentActorResolver actorResolver,
    ICurrentUserContext currentUser)
    : ICommandHandler<RemoveProjectAssignmentCommand, Result>
{
    public async Task<Result> Handle(RemoveProjectAssignmentCommand request, CancellationToken cancellationToken)
    {
        var project = await projects.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null)
        {
            return Error.NotFound("Project", request.ProjectId).ToFailure<Result>();
        }

        var actor = await actorResolver.GetOrProvisionAsync(cancellationToken);
        if (actor is null)
        {
            return new Error("Unauthorized", "A validated Entra identity is required.").ToFailure<Result>();
        }

        if (!GlobalRoleCapabilityMatrix.HasCapability(currentUser.GlobalRoles, CapabilityNames.ProjectManageAssignments) ||
            !ProjectAccessPolicy.IsProjectManager(project, actor.Id))
        {
            return Error.NotFound("Project", request.ProjectId).ToFailure<Result>();
        }

        if (project.Version != request.ExpectedVersion)
        {
            return new Error("Conflict", "The project has changed. Reload it before applying this update.")
                .ToFailure<Result>();
        }

        if (!project.RemoveAssignment(request.UserId))
        {
            return Error.NotFound("Project assignment", request.UserId).ToFailure<Result>();
        }

        await projects.UpdateAsync(project, cancellationToken);
        return Result.Success();
    }
}
