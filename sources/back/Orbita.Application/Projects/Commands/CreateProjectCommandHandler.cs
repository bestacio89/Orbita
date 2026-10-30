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
            return new Error("Forbidden", "The current identity cannot create projects.").ToFailure<Guid>();
        }

        var actor = await actorResolver.GetOrProvisionAsync(cancellationToken);
        if (actor is null)
        {
            return new Error("Unauthorized", "A validated Entra identity is required.").ToFailure<Guid>();
        }

        Project project;
        try
        {
            project = Project.CreateManually(
                request.Client,
                request.Name,
                request.Reference,
                request.StartDate,
                request.ContractualEndDate,
                request.ContractType,
                actor.Id,
                timeProvider.GetUtcNow());
        }
        catch (BusinessRuleException exception)
        {
            return Error.Validation("Project", exception.Message).ToFailure<Guid>();
        }

        if (await projects.GetByUniqueKeyAsync(project.UniqueKey, cancellationToken) is not null)
        {
            return new Error("Conflict", "A project with the same commercial identity already exists.").ToFailure<Guid>();
        }

        project.AssignUser(actor.Id, ProjectRole.ProjectManager, timeProvider.GetUtcNow(), actor.Id);
        await projects.AddAsync(project, cancellationToken);
        return project.Id.ToResult();
    }
}
