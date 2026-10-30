using System.Globalization;
using Orbita.Domain.Common;
using Orbita.Domain.Enums;

namespace Orbita.Domain.Entities;

/// <summary>Scheduling and invitation state for a workshop task.</summary>
public sealed class Workshop : ProjectEntity
{
    public Guid TaskId { get; set; }
    public DateOnly? ScheduledDate { get; private set; }
    public TimeOnly? StartTime { get; private set; }
    public TimeOnly? EndTime { get; private set; }
    public bool InvitationSent { get; private set; }
    public DateOnly? InvitationSentOn { get; private set; }
    public List<WorkshopRecipient> Recipients { get; set; } = [];

    public void SetDate(DateOnly? date)
    {
        ScheduledDate = date;
        if (date is null)
        {
            InvitationSent = false;
            InvitationSentOn = null;
        }
    }

    public void SetTime(TimeOnly? start, TimeOnly? end)
    {
        if (start is null)
        {
            end = null;
        }

        if (start is not null && end is not null && end <= start)
        {
            throw new BusinessRuleException(
                $"Workshop end time ({end.Value.ToString("HH:mm", CultureInfo.InvariantCulture)}) must be after its start time ({start.Value.ToString("HH:mm", CultureInfo.InvariantCulture)}).");
        }

        StartTime = start;
        EndTime = end;
    }

    public void SetInvitationSent(bool sent, DateOnly today)
    {
        InvitationSent = sent;
        InvitationSentOn = sent ? InvitationSentOn ?? today : null;
    }

    public void SetInvitationSentDate(DateOnly sentOn, DateOnly today)
    {
        if (sentOn > today)
        {
            throw new BusinessRuleException("An invitation sent date cannot be in the future.");
        }

        InvitationSent = true;
        InvitationSentOn = sentOn;
    }

    public bool IsEmpty =>
        ScheduledDate is null &&
        StartTime is null &&
        EndTime is null &&
        !InvitationSent &&
        Recipients.Count == 0;
}

/// <summary>A person invited to a workshop.</summary>
public sealed class WorkshopRecipient : ProjectEntity
{
    public Guid WorkshopId { get; set; }
    public string NameOrEmail { get; set; } = string.Empty;
    public int Order { get; set; }
    public Guid? UserId { get; set; }
}

/// <summary>Delivery item linked to a workload task.</summary>
public sealed class Deliverable : ProjectEntity
{
    public Guid TaskId { get; set; }
    public DateOnly? ClientValidatedOn { get; set; }
    public string? StorageUri { get; set; }
}

/// <summary>
/// User-authored comment on a task, action, decision or risk.
/// Mutation is restricted to the original author; project role does not override ownership.
/// </summary>
public sealed class ProjectComment : ProjectEntity
{
    private ProjectComment()
    {
    }

    public string TargetType { get; private set; } = string.Empty;
    public Guid TargetId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid AuthorUserId { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public DateTimeOffset? ModifiedAt { get; private set; }

    public static ProjectComment Create(
        Guid projectId,
        string targetType,
        Guid targetId,
        Guid authorUserId,
        string text,
        DateTimeOffset createdAt)
    {
        if (projectId == Guid.Empty || targetId == Guid.Empty || authorUserId == Guid.Empty)
        {
            throw new ArgumentException("Project, target and author identifiers are required.");
        }

        if (string.IsNullOrWhiteSpace(targetType) || string.IsNullOrWhiteSpace(text))
        {
            throw new BusinessRuleException("A comment target and comment text are required.");
        }

        return new ProjectComment
        {
            ProjectId = projectId,
            TargetType = targetType.Trim(),
            TargetId = targetId,
            AuthorUserId = authorUserId,
            Text = text.Trim(),
            CreatedAt = createdAt
        };
    }

    public void Edit(Guid actorUserId, string text, DateTimeOffset modifiedAt)
    {
        EnsureAuthor(actorUserId);

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new BusinessRuleException("Comment text cannot be empty.");
        }

        if (modifiedAt < CreatedAt || (ModifiedAt is not null && modifiedAt < ModifiedAt))
        {
            throw new BusinessRuleException("A comment modification timestamp cannot move backwards.");
        }

        Text = text.Trim();
        ModifiedAt = modifiedAt;
    }

    public void EnsureAuthor(Guid actorUserId)
    {
        if (actorUserId == Guid.Empty || actorUserId != AuthorUserId)
        {
            throw new BusinessRuleException("Only the original comment author can modify or delete this comment.");
        }
    }
}

/// <summary>Weekly or monthly project delivery status point.</summary>
public sealed class WeeklyStatusPoint : ProjectEntity
{
    /// <summary>Reserved key, ISO week key or ISO month key.</summary>
    public string Key { get; set; } = string.Empty;
    /// <summary>Validated JSON payload owned by the application service.</summary>
    public string Content { get; set; } = "{}";
    public DateTimeOffset ModifiedAt { get; set; }
    public Guid? ModifiedByUserId { get; set; }
}

/// <summary>Project action tracked through the governance register.</summary>
public sealed class ProjectAction : ProjectEntity
{
    public string Reference { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public Guid? OwnerUserId { get; set; }
    public string? GovernanceInstance { get; set; }
    public DateOnly CreatedOn { get; set; }
    public DateOnly? TargetDate { get; set; }
    public decimal Progress { get; set; }
    public ActionStatus Status { get; set; } = ActionStatus.ToDo;
    public DateOnly? CompletedOn { get; set; }
    public string Comment { get; set; } = string.Empty;
    public Guid? CreatedByUserId { get; set; }
    public Guid? TaskId { get; set; }
}

/// <summary>Project decision recorded in the governance register.</summary>
public sealed class ProjectDecision : ProjectEntity
{
    public string Reference { get; set; } = string.Empty;
    public DateOnly DecisionDate { get; set; }
    public string? GovernanceInstance { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? DecisionMaker { get; set; }
    public string? Impact { get; set; }
    public DecisionStatus Status { get; set; } = DecisionStatus.PendingApproval;
    public string Comment { get; set; } = string.Empty;
    public Guid? CreatedByUserId { get; set; }
}

/// <summary>Project risk. Criticality is the product of the four-level probability and impact scores.</summary>
public sealed class ProjectRisk : ProjectEntity
{
    public string Reference { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public RiskProbability Probability { get; set; } = RiskProbability.Moderate;
    public RiskImpact Impact { get; set; } = RiskImpact.Medium;
    public RiskType? Type { get; set; }
    public string? Cause { get; set; }
    public string? Mitigation { get; set; }
    public string? ActionPlan { get; set; }
    public string? Owner { get; set; }
    public RiskStatus Status { get; set; } = RiskStatus.Open;
    public string Comment { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public Guid? ActionId { get; set; }

    public int Criticality => ((int)Probability + 1) * ((int)Impact + 1);
}
