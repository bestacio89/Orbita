#nullable enable

using Franz.Common.Mediator.Errors;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Results;
using Orbita.Application.Identity;
using Orbita.Contracts.Authorization;
using Orbita.Contracts.Commands.Projects;
using Orbita.Contracts.Persistence;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;
using Orbita.Domain.Services;

namespace Orbita.Application.Projects.Commands;

public sealed class CreateProjectCommandHandler(
    IProjectRepository projects,
    CurrentActorResolver actorResolver,
    ICurrentUserContext currentUser,
    TimeProvider timeProvider)
    : ICommandHandler<CreateProjectCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        if (!GlobalRoleCapabilityMatrix.HasCapability(currentUser.GlobalRoles, CapabilityNames.ProjectCreate))
        {
            return "The current identity cannot create projects.".ToFailure<Guid>();
        }

        var actor = await actorResolver.GetOrProvisionAsync(cancellationToken);
        if (actor is null)
        {
            return "A validated Entra identity is required.".ToFailure<Guid>();
        }

        var project = Project.CreateManually(
            request.Client,
            request.Name,
            request.Reference,
            request.StartDate,
            request.ContractualEndDate,
            request.ContractType,
            actor.Id,
            timeProvider.GetUtcNow());

        if (await projects.GetByUniqueKeyAsync(project.UniqueKey, cancellationToken) is not null)
        {
            return "A project with the same commercial identity already exists.".ToFailure<Guid>();
        }

        project.AssignUser(actor.Id, ProjectRole.ProjectManager, timeProvider.GetUtcNow(), actor.Id);
        await projects.AddAsync(project, cancellationToken);
        return project.Id.ToResult();
    }
}

public sealed class UpdateProjectDatesCommandHandler(
    IProjectRepository projects,
    CurrentActorResolver actorResolver,
    ICurrentUserContext currentUser)
    : ICommandHandler<UpdateProjectDatesCommand, Result>
{
    public async Task<Result> Handle(UpdateProjectDatesCommand request, CancellationToken cancellationToken)
    {
        var project = await projects.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null)
        {
            return Error.NotFound("Project", request.ProjectId).ToFailure<Result>();
        }

        if (project.Version != request.ExpectedVersion)
        {
            return "The project has changed. Reload it before applying this update.".ToFailure<Result>();
        }

        var actor = await actorResolver.GetOrProvisionAsync(cancellationToken);
        if (actor is null || !CanManageProject(project, actor.Id, currentUser.GlobalRoles))
        {
            return "The current identity cannot update this project.".ToFailure<Result>();
        }

        project.SetDates(request.StartDate, request.ContractualEndDate, request.ForecastEndDate);
        await projects.UpdateAsync(project, cancellationToken);
        return Result.Success();
    }

    private static bool CanManageProject(Project project, Guid userId, IEnumerable<string> roles) =>
        GlobalRoleCapabilityMatrix.HasCapability(roles, CapabilityNames.ProjectUpdate) ||
        ProjectAccessPolicy.IsProjectManager(project, userId);
}

public sealed class ArchiveProjectCommandHandler(
    IProjectRepository projects,
    ICurrentUserContext currentUser)
    : ICommandHandler<ArchiveProjectCommand, Result>
{
    public async Task<Result> Handle(ArchiveProjectCommand request, CancellationToken cancellationToken)
    {
        if (!GlobalRoleCapabilityMatrix.HasCapability(currentUser.GlobalRoles, CapabilityNames.ProjectArchive))
        {
            return "The current identity cannot archive projects.".ToFailure<Result>();
        }

        var project = await projects.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null)
        {
            return Error.NotFound("Project", request.ProjectId).ToFailure<Result>();
        }

        if (project.Version != request.ExpectedVersion)
        {
            return "The project has changed. Reload it before applying this update.".ToFailure<Result>();
        }

        project.Archive();
        await projects.UpdateAsync(project, cancellationToken);
        return Result.Success();
    }
}

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

        if (project.Version != request.ExpectedVersion)
        {
            return "The project has changed. Reload it before applying this update.".ToFailure<Result>();
        }

        var actor = await actorResolver.GetOrProvisionAsync(cancellationToken);
        if (actor is null || !CanManageAssignments(project, actor.Id, currentUser.GlobalRoles))
        {
            return "The current identity cannot manage this project's assignments.".ToFailure<Result>();
        }

        if (await users.GetByIdAsync(request.UserId, cancellationToken) is null)
        {
            return Error.NotFound("User", request.UserId).ToFailure<Result>();
        }

        project.AssignUser(request.UserId, request.Role, timeProvider.GetUtcNow(), actor.Id);
        await projects.UpdateAsync(project, cancellationToken);
        return Result.Success();
    }

    private static bool CanManageAssignments(Project project, Guid userId, IEnumerable<string> roles) =>
        GlobalRoleCapabilityMatrix.HasCapability(roles, CapabilityNames.ProjectManageAssignments) ||
        ProjectAccessPolicy.IsProjectManager(project, userId);
}

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

        if (project.Version != request.ExpectedVersion)
        {
            return "The project has changed. Reload it before applying this update.".ToFailure<Result>();
        }

        var actor = await actorResolver.GetOrProvisionAsync(cancellationToken);
        if (actor is null ||
            !GlobalRoleCapabilityMatrix.HasCapability(currentUser.GlobalRoles, CapabilityNames.ProjectManageAssignments) &&
            !ProjectAccessPolicy.IsProjectManager(project, actor.Id))
        {
            return "The current identity cannot manage this project's assignments.".ToFailure<Result>();
        }

        if (!project.RemoveAssignment(request.UserId))
        {
            return Error.NotFound("Project assignment", request.UserId).ToFailure<Result>();
        }

        await projects.UpdateAsync(project, cancellationToken);
        return Result.Success();
    }
}
