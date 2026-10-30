using Orbita.Domain.Common;
using Orbita.Domain.Enums;

namespace Orbita.Domain.Entities;

/// <summary>Project-scoped access grant. Global application roles are not represented by this entity.</summary>
public sealed class ProjectAssignment : ProjectEntity
{
    private ProjectAssignment()
    {
    }

    internal ProjectAssignment(Guid projectId, Guid userId, ProjectRole role, DateTimeOffset assignedAt, Guid? grantedByUserId)
    {
        if (projectId == Guid.Empty)
        {
            throw new ArgumentException("A project identifier is required.", nameof(projectId));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("A user identifier is required.", nameof(userId));
        }

        ProjectId = projectId;
        UserId = userId;
        Role = role;
        AssignedAt = assignedAt;
        GrantedByUserId = grantedByUserId;
    }

    public Guid UserId { get; private set; }
    public ProjectRole Role { get; private set; }
    public DateTimeOffset AssignedAt { get; private set; }
    public Guid? GrantedByUserId { get; private set; }

    public void ChangeRole(ProjectRole role) => Role = role;
}
