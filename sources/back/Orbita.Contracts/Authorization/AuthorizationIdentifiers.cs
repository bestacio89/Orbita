namespace Orbita.Contracts.Authorization;

/// <summary>Stable global role values issued by the configured identity provider.</summary>
public static class GlobalRoleNames
{
    public const string Manager = "Manager";
    public const string Admin = "Admin";
}

/// <summary>
/// Stable capability identifiers exposed by the current-user contract.
/// The API derives these values from validated global role claims; incoming capability claims are never trusted.
/// </summary>
public static class CapabilityNames
{
    public const string ProjectRead = "projects.read";
    public const string ProjectCreate = "projects.create";
    public const string ProjectUpdate = "projects.update";
    public const string ProjectArchive = "projects.archive";
    public const string ProjectManageAssignments = "projects.assignments.manage";
    public const string ProjectFinancialRead = "projects.financial.read";
    /// <summary>Granted to Manager according to the explicit role matrix; Admin does not inherit it implicitly.</summary>
    public const string ProjectFinancialExport = "projects.financial.export";
    public const string ProjectTaskUpdate = "projects.tasks.update";
    public const string ProjectTimeEntry = "projects.time.write";
    public const string ProjectGovernanceWrite = "projects.governance.write";
    public const string ProjectCommentWriteOwn = "projects.comments.write-own";
    public const string DraftReadOwn = "drafts.read-own";
    public const string DraftWriteOwn = "drafts.write-own";
}
