namespace Orbita.Domain.Common;

/// <summary>Controlled planning variance categories used for project and portfolio reporting.</summary>
public static class PlanningVarianceCauses
{
    public static IReadOnlyList<string> All { get; } =
        ["Client", "Scope", "Team", "Technical", "Estimation", "Other"];
}
