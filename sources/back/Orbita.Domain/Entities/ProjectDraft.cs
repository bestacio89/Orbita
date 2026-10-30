using Orbita.Domain.Common;
using Orbita.Domain.Enums;

namespace Orbita.Domain.Entities;

/// <summary>
/// Private project draft containing a TSP being authored, imported or copied from an existing project.
/// Only its author and explicitly invited co-author can access it.
/// </summary>
public sealed class ProjectDraft : DomainEntity
{
    private ProjectDraft()
    {
    }

    public string Client { get; private set; } = string.Empty;
    public string ProjectName { get; private set; } = string.Empty;
    public string? Reference { get; private set; }
    public DraftOrigin Origin { get; private set; }
    public string? Source { get; private set; }
    /// <summary>Serialized TSP draft payload. Its schema is versioned by the payload format.</summary>
    public string Content { get; private set; } = string.Empty;
    public Guid AuthorUserId { get; private set; }
    public Guid? CoAuthorUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ModifiedAt { get; private set; }
    public Guid? ModifiedByUserId { get; private set; }

    public static ProjectDraft Create(
        string client,
        string projectName,
        string? reference,
        DraftOrigin origin,
        string content,
        Guid authorUserId,
        DateTimeOffset createdAt,
        string? source = null)
    {
        if (string.IsNullOrWhiteSpace(client) || string.IsNullOrWhiteSpace(projectName))
        {
            throw new BusinessRuleException("A draft requires both a client and a project name.");
        }

        if (authorUserId == Guid.Empty)
        {
            throw new BusinessRuleException("A draft author is required.");
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new BusinessRuleException("A draft must contain a TSP payload.");
        }

        return new ProjectDraft
        {
            Client = client.Trim(),
            ProjectName = projectName.Trim(),
            Reference = string.IsNullOrWhiteSpace(reference) ? null : reference.Trim(),
            Origin = origin,
            Content = content,
            AuthorUserId = authorUserId,
            CreatedAt = createdAt,
            ModifiedAt = createdAt,
            Source = string.IsNullOrWhiteSpace(source) ? null : source.Trim()
        };
    }

    public bool CanAccess(Guid userId) =>
        userId != Guid.Empty && (AuthorUserId == userId || CoAuthorUserId == userId);

    public bool CanEdit(Guid userId) => CanAccess(userId);

    public void EnsureCanAccess(Guid userId)
    {
        if (!CanAccess(userId))
        {
            throw new BusinessRuleException("This draft is not accessible to the current user.");
        }
    }

    public void EnsureCanEdit(Guid userId)
    {
        if (!CanEdit(userId))
        {
            throw new BusinessRuleException("Only the draft author or its invited co-author can edit this draft.");
        }
    }

    public void InviteCoAuthor(Guid actorUserId, Guid? coAuthorUserId, DateTimeOffset modifiedAt)
    {
        EnsureAuthor(actorUserId);
        EnsureModificationTimestamp(modifiedAt);

        if (coAuthorUserId == Guid.Empty || coAuthorUserId == AuthorUserId)
        {
            throw new BusinessRuleException("The co-author must be a different, valid user.");
        }

        CoAuthorUserId = coAuthorUserId;
        ModifiedAt = modifiedAt;
        ModifiedByUserId = actorUserId;
    }

    public void UpdateContent(Guid actorUserId, string content, DateTimeOffset modifiedAt)
    {
        EnsureCanEdit(actorUserId);
        EnsureModificationTimestamp(modifiedAt);

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new BusinessRuleException("A draft must contain a TSP payload.");
        }

        Content = content;
        ModifiedAt = modifiedAt;
        ModifiedByUserId = actorUserId;
    }

    private void EnsureAuthor(Guid actorUserId)
    {
        if (actorUserId != AuthorUserId)
        {
            throw new BusinessRuleException("Only the draft author can manage co-author access.");
        }
    }

    private void EnsureModificationTimestamp(DateTimeOffset modifiedAt)
    {
        if (modifiedAt < ModifiedAt)
        {
            throw new BusinessRuleException("A draft modification timestamp cannot move backwards.");
        }
    }
}
