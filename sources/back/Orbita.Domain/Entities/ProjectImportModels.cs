namespace Orbita.Domain.Entities;

/// <summary>Free-form project-sheet field retained from the source TSP workbook.</summary>
public sealed class ProjectField : ProjectEntity
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    /// <summary>Recognized semantic key, or null when the field is custom.</summary>
    public string? Key { get; set; }
    public int Order { get; set; }
}

/// <summary>Trace of a TSP import, including workbook diagnostics and source planning tabs.</summary>
public sealed class TspImportRecord : ProjectEntity
{
    public string FileName { get; set; } = string.Empty;
    public DateTimeOffset ImportedAt { get; set; }
    public Guid? ImportedByUserId { get; set; }
    /// <summary>Import observations preserved for review, one per line.</summary>
    public string ObservedPoints { get; set; } = string.Empty;
    public string? ProfilePlanningSource { get; set; }
    public string? PhasePlanningSource { get; set; }
    public Guid? HiddenByUserId { get; set; }
}
