namespace Orbita.Domain.Entities;

/// <summary>Project-scoped audit journal entry describing a business mutation.</summary>
public sealed class ProjectJournalEntry : ProjectEntity
{
    private ProjectJournalEntry()
    {
    }

    public DateTimeOffset OccurredAt { get; private set; }
    public Guid? UserId { get; private set; }
    public string EntityType { get; private set; } = string.Empty;
    public Guid? EntityId { get; private set; }
    public string MemberName { get; private set; } = string.Empty;
    public string? PreviousValue { get; private set; }
    public string? NewValue { get; private set; }

    public static ProjectJournalEntry Create(
        Guid projectId,
        DateTimeOffset occurredAt,
        Guid? userId,
        string entityType,
        Guid? entityId,
        string memberName,
        string? previousValue,
        string? newValue)
    {
        if (projectId == Guid.Empty)
        {
            throw new ArgumentException("A project identifier is required.", nameof(projectId));
        }

        if (string.IsNullOrWhiteSpace(entityType))
        {
            throw new ArgumentException("The changed entity type is required.", nameof(entityType));
        }

        if (string.IsNullOrWhiteSpace(memberName))
        {
            throw new ArgumentException("The changed member name is required.", nameof(memberName));
        }

        var entry = new ProjectJournalEntry
        {
            ProjectId = projectId,
            OccurredAt = occurredAt,
            UserId = userId,
            EntityType = entityType.Trim(),
            EntityId = entityId,
            MemberName = memberName.Trim(),
            PreviousValue = previousValue,
            NewValue = newValue
        };

        return entry;
    }
}
