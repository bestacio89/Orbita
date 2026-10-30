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
public enum ProjectTaskStatus { ToDo, InProgress, Blocked, InAcceptance, Validated }
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

