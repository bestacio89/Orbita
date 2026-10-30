using Orbita.Contracts.DTOs.Projects;
using Orbita.Domain.Entities;

namespace Orbita.Application.Projects.Queries;

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
