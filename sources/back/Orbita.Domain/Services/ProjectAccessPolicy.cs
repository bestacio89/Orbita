using Orbita.Domain.Entities;
using Orbita.Domain.Enums;

namespace Orbita.Domain.Services;

/// <summary>
/// Project-scoped authorization rules. Global Manager/Admin capabilities are evaluated separately
/// by the application/API boundary; these rules never grant access based on a global role.
/// </summary>
public static class ProjectAccessPolicy
{
    public static bool CanAccessProject(Project project, Guid userId)
    {
        ArgumentNullException.ThrowIfNull(project);
        return userId != Guid.Empty && project.Assignments.Any(assignment => assignment.UserId == userId);
    }

    public static bool IsProjectManager(Project project, Guid userId)
    {
        ArgumentNullException.ThrowIfNull(project);
        return userId != Guid.Empty && project.Assignments.Any(assignment =>
            assignment.UserId == userId && assignment.Role == ProjectRole.ProjectManager);
    }

    public static bool CanUpdateDelivery(Project project, Guid userId)
    {
        ArgumentNullException.ThrowIfNull(project);
        return userId != Guid.Empty && project.Assignments.Any(assignment =>
            assignment.UserId == userId &&
            assignment.Role is ProjectRole.ProjectManager or ProjectRole.Contributor);
    }
}
