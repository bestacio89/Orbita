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
        var canReadAll = GlobalRoleCapabilityMatrix.HasCapability(currentUser.GlobalRoles, CapabilityNames.ProjectRead);
        if (actor is null || (!canReadAll && !ProjectAccessPolicy.CanAccessProject(project, actor.Id)))
        {
            return Error.NotFound("Project", request.ProjectId).ToFailure<ProjectDetailsDto>();
        }

        return ProjectDtoMapper.ToDetails(project).ToResult();
    }
}

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
            return "Page must be positive and page size must be between 1 and 100.".ToFailure<IReadOnlyCollection<ProjectSummaryDto>>();
        }

        var actor = await actorResolver.GetOrProvisionAsync(cancellationToken);
        if (actor is null)
        {
            return "A validated Entra identity is required.".ToFailure<IReadOnlyCollection<ProjectSummaryDto>>();
        }

        var canReadAll = GlobalRoleCapabilityMatrix.HasCapability(currentUser.GlobalRoles, CapabilityNames.ProjectRead);
        var accessible = canReadAll
            ? await projects.GetAllAsync(cancellationToken)
            : await projects.GetAccessibleByUserAsync(actor.Id, cancellationToken);

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

internal static class ProjectDtoMapper
{
    public static ProjectSummaryDto ToSummary(Project project) =>
        new(
            project.Id,
            project.Client,
            project.Name,
            project.Reference,
            project.Status,
            project.StartDate,
            project.ContractualEndDate,
            project.ForecastEndDate,
            project.Version);

    public static ProjectDetailsDto ToDetails(Project project) =>
        new(
            project.Id,
            project.Client,
            project.Name,
            project.Reference,
            project.UniqueKey,
            project.Agency,
            project.ServiceLine,
            project.Director,
            project.ServiceLineManager,
            project.SalesRepresentative,
            project.ClientContact,
            project.ContractType,
            project.Status,
            project.Origin,
            project.StartDate,
            project.ContractualEndDate,
            project.ForecastEndDate,
            project.Discount,
            project.InitialRevenue,
            project.Version,
            new ProjectSettingsDto(
                project.Settings.Grouping,
                project.Settings.FirstPlanningMonth,
                project.Settings.MonthCount,
                project.Settings.ActionAlertDays,
                project.Settings.MilestoneAlertDays,
                project.Settings.WorkshopAlertDays,
                project.Settings.DeliverableAlertDays,
                project.Settings.LastProjectCommitteeDate),
            project.Assignments.Select(assignment => new ProjectAssignmentDto(
                assignment.UserId,
                assignment.Role,
                assignment.AssignedAt,
                assignment.GrantedByUserId)).ToArray());
}
