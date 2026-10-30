namespace Orbita.Contracts.Authorization;

/// <summary>
/// Deterministic capability mapping for validated global identity-provider roles.
/// Project assignment and resource ownership checks remain mandatory after this global capability check.
/// </summary>
public static class GlobalRoleCapabilityMatrix
{
    private static readonly IReadOnlyDictionary<string, IReadOnlySet<string>> CapabilitiesByRole =
        new Dictionary<string, IReadOnlySet<string>>(StringComparer.Ordinal)
        {
            [GlobalRoleNames.Manager] = new HashSet<string>(StringComparer.Ordinal)
            {
                CapabilityNames.ProjectRead,
                CapabilityNames.ProjectCreate,
                CapabilityNames.ProjectUpdate,
                CapabilityNames.ProjectArchive,
                CapabilityNames.ProjectManageAssignments,
                CapabilityNames.ProjectFinancialRead,
                CapabilityNames.ProjectFinancialExport,
                CapabilityNames.ProjectTaskUpdate,
                CapabilityNames.ProjectTimeEntry,
                CapabilityNames.ProjectGovernanceWrite,
                CapabilityNames.ProjectCommentWriteOwn,
                CapabilityNames.DraftReadOwn,
                CapabilityNames.DraftWriteOwn
            },
            [GlobalRoleNames.Admin] = new HashSet<string>(StringComparer.Ordinal)
            {
                CapabilityNames.ProjectRead,
                CapabilityNames.ProjectCreate,
                CapabilityNames.ProjectUpdate,
                CapabilityNames.ProjectArchive,
                CapabilityNames.ProjectManageAssignments,
                CapabilityNames.ProjectFinancialRead,
                CapabilityNames.ProjectTaskUpdate,
                CapabilityNames.ProjectTimeEntry,
                CapabilityNames.ProjectGovernanceWrite,
                CapabilityNames.ProjectCommentWriteOwn,
                CapabilityNames.DraftReadOwn,
                CapabilityNames.DraftWriteOwn
            }
        };

    /// <summary>
    /// Resolves the union of capabilities for validated global roles.
    /// Unknown roles are ignored; capability claims supplied by a caller are never accepted as input.
    /// </summary>
    public static IReadOnlyCollection<string> Resolve(IEnumerable<string>? validatedGlobalRoles)
    {
        if (validatedGlobalRoles is null)
        {
            return Array.Empty<string>();
        }

        return validatedGlobalRoles
            .Where(CapabilitiesByRole.ContainsKey)
            .SelectMany(role => CapabilitiesByRole[role])
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
    }

    public static bool HasCapability(IEnumerable<string>? validatedGlobalRoles, string capability) =>
        Resolve(validatedGlobalRoles).Contains(capability, StringComparer.Ordinal);
}

/// <summary>Version of the descriptive current-user capability contract.</summary>
public static class CurrentUserContract
{
    public const int Version = 1;
}
