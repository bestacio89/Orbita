namespace Orbita.Domain.Enums;

/// <summary>Project-level assignment. Global application roles are modeled separately.</summary>
public enum ProjectRole
{
    Contributor,
    ProjectManager,
    Steering,
    Reader
}

public enum ProjectStatus { Preparation, InProgress, Archived }
public enum ProjectOrigin { TspImport, ManualCreation, TspAuthoring }
public enum DraftOrigin { Blank, TspImport, ExistingProjectCopy }
public enum ElementOrigin { Tsp, Added }
public enum TaskStatus { ToDo, InProgress, Blocked, InAcceptance, Validated }
public enum ScopeChangeKind { NewTask, AdditionalEffort, Cancellation, Reinstatement, Reassignment }
public enum ActionStatus { ToDo, InProgress, Done, Cancelled }
public enum DecisionStatus { Approved, PendingApproval, Rejected }
public enum RiskProbability { Low, Moderate, High, VeryHigh }
public enum RiskImpact { Low, Medium, High, Critical }
public enum RiskType { Delay, Quality, Budget }
public enum MilestoneType { Date, Period, Week }
public enum PlanningElementKind { Highlighted, Hidden }
public enum AbsencePeriod { FullDay, Morning, Afternoon }
public enum RiskStatus { Open, Mitigated, Occurred, Closed }
public enum PurchaseStatus { Ordered, Received, Invoiced }
public enum ExpenseCategory { Transport, Accommodation, Meal, Other }
public enum IndicatorSnapshotKind { SteeringCommittee, ProjectCommittee, Monthly }
public enum AiListKind { OptimizationLever, OverrunCause }
public enum PlanningGrouping { Axis, Lot, Phase }
public enum NotificationKind { Action, Deliverable, Workshop, InvoiceMilestone }
public enum NotificationDeliveryStatus { Pending, Sent, Failed }

/// <summary>Stable global role identifiers supplied by validated identity-provider role claims.</summary>
public static class GlobalRoleNames
{
    public const string Manager = "Manager";
    public const string Admin = "Admin";
}

/// <summary>Stable capability identifiers. The API derives them from validated global roles and rechecks access per operation.</summary>
public static class CapabilityNames
{
    public const string ProjectRead = "projects.read";
    public const string ProjectCreate = "projects.create";
    public const string ProjectUpdate = "projects.update";
    public const string ProjectArchive = "projects.archive";
    public const string ProjectManageAssignments = "projects.assignments.manage";
    public const string ProjectFinancialRead = "projects.financial.read";
    public const string ProjectFinancialExport = "projects.financial.export";
    public const string ProjectTaskUpdate = "projects.tasks.update";
    public const string ProjectTimeEntry = "projects.time.write";
    public const string ProjectGovernanceWrite = "projects.governance.write";
    public const string ProjectCommentWriteOwn = "projects.comments.write-own";
    public const string DraftReadOwn = "drafts.read-own";
    public const string DraftWriteOwn = "drafts.write-own";
}
