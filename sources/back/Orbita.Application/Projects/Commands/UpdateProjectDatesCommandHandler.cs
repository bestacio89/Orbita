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

        var actor = await actorResolver.GetOrProvisionAsync(cancellationToken);
        if (actor is null)
        {
            return new Error("Unauthorized", "A validated Entra identity is required.").ToFailure<Result>();
        }

        if (!CanManageProject(project, actor.Id, currentUser.GlobalRoles))
        {
            return Error.NotFound("Project", request.ProjectId).ToFailure<Result>();
        }

        if (project.Version != request.ExpectedVersion)
        {
            return new Error("Conflict", "The project has changed. Reload it before applying this update.")
                .ToFailure<Result>();
        }

        try
        {
            project.SetDates(request.StartDate, request.ContractualEndDate, request.ForecastEndDate);
        }
        catch (BusinessRuleException exception)
        {
            return Error.Validation("Dates", exception.Message).ToFailure<Result>();
        }

        await projects.UpdateAsync(project, cancellationToken);
        return Result.Success();
    }

    private static bool CanManageProject(Project project, Guid userId, IEnumerable<string> roles) =>
        GlobalRoleCapabilityMatrix.HasCapability(roles, CapabilityNames.ProjectUpdate) &&
        ProjectAccessPolicy.IsProjectManager(project, userId);
}
