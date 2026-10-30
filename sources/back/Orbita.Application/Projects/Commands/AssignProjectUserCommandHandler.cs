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

public sealed class AssignProjectUserCommandHandler(
    IProjectRepository projects,
    IUserRepository users,
    CurrentActorResolver actorResolver,
    ICurrentUserContext currentUser,
    TimeProvider timeProvider)
    : ICommandHandler<AssignProjectUserCommand, Result>
{
    public async Task<Result> Handle(AssignProjectUserCommand request, CancellationToken cancellationToken)
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

        if (!CanManageAssignments(project, actor.Id, currentUser.GlobalRoles))
        {
            return Error.NotFound("Project", request.ProjectId).ToFailure<Result>();
        }

        if (project.Version != request.ExpectedVersion)
        {
            return new Error("Conflict", "The project has changed. Reload it before applying this update.")
                .ToFailure<Result>();
        }

        var assignedUser = await users.GetByIdAsync(request.UserId, cancellationToken);
        if (assignedUser is null || !assignedUser.IsActive)
        {
            return Error.NotFound("Active user", request.UserId).ToFailure<Result>();
        }

        try
        {
            project.AssignUser(request.UserId, request.Role, timeProvider.GetUtcNow(), actor.Id);
        }
        catch (BusinessRuleException exception)
        {
            return Error.Validation("Assignment", exception.Message).ToFailure<Result>();
        }

        await projects.UpdateAsync(project, cancellationToken);
        return Result.Success();
    }

    private static bool CanManageAssignments(Project project, Guid userId, IEnumerable<string> roles) =>
        GlobalRoleCapabilityMatrix.HasCapability(roles, CapabilityNames.ProjectManageAssignments) &&
        ProjectAccessPolicy.IsProjectManager(project, userId);
}
