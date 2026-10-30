using Franz.Common.Business.Domain;

namespace Orbita.Domain.Entities;

/// <summary>
/// Base class for persisted domain entities. Franz.Common owns identity and audit lifecycle;
/// the domain retains an explicit optimistic-concurrency version used by application workflows.
/// </summary>
public abstract class DomainEntity : Entity<Guid>
{
    protected DomainEntity() : base(Guid.NewGuid())
    {
    }

    protected DomainEntity(Guid id) : base(id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("An entity identifier cannot be empty.", nameof(id));
        }
    }

    /// <summary>Optimistic-concurrency token incremented by the persistence boundary.</summary>
    public int Version { get; set; }
}

/// <summary>Base class for entities whose visibility and lifecycle are scoped to one project.</summary>
public abstract class ProjectEntity : DomainEntity
{
    protected ProjectEntity()
    {
    }

    protected ProjectEntity(Guid id) : base(id)
    {
    }

    public Guid ProjectId { get; set; }
}
