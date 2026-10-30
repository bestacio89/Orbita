using Orbita.Domain.Common;
using Orbita.Domain.Enums;

namespace Orbita.Domain.Entities;

/// <summary>Person-level absence, independent of any project. One entry is visible across all assigned projects.</summary>
public sealed class Absence : DomainEntity
{
    public string PersonKey { get; set; } = string.Empty;
    public string PersonName { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public DateOnly Date { get; set; }
    public AbsencePeriod Period { get; set; } = AbsencePeriod.FullDay;
    public DateTimeOffset RecordedAt { get; set; }
    public Guid? RecordedByUserId { get; set; }
}

/// <summary>Person manually added to a project's absence view, outside its TSP profiles.</summary>
public sealed class ProjectAddedPerson : ProjectEntity
{
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
}
