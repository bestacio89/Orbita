using Orbita.Domain.Enums;

namespace Orbita.Domain.Entities;

/// <summary>Project axis identified by the stable code imported from the TSP.</summary>
public sealed class ProjectAxis : ProjectEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateOnly? DueDate { get; set; }
    public int Order { get; set; }
}

/// <summary>Optional display name for a TSP lot; task lot codes remain the source identity.</summary>
public sealed class ProjectLot : ProjectEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

/// <summary>Delivery phase reference.</summary>
public sealed class ProjectPhase : ProjectEntity
{
    public string Code { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsReferenceValue { get; set; } = true;
}

/// <summary>Delivery step reference.</summary>
public sealed class ProjectStep : ProjectEntity
{
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
}

/// <summary>Task type. Core types such as Workshop and Deliverable cannot be removed.</summary>
public sealed class ProjectTaskType : ProjectEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsCoreType { get; set; }
    public int Order { get; set; }
}

/// <summary>AI optimization lever or overrun cause offered by the project reference lists.</summary>
public sealed class AiReferenceValue : ProjectEntity
{
    public AiListKind ListKind { get; set; }
    public string Label { get; set; } = string.Empty;
    public int Order { get; set; }
}
