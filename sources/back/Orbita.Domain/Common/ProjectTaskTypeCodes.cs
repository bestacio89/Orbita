namespace Orbita.Domain.Common;

/// <summary>Core task-type codes retained by the French TSP workbook contract.</summary>
public static class ProjectTaskTypeCodes
{
    public const string Workshop = "Atelier";
    public const string Deliverable = "Livrable";

    public static IReadOnlyList<(string Code, string Name)> CoreTypes { get; } =
    [
        (Workshop, "Atelier"),
        (Deliverable, "Livrable")
    ];
}
