using Orbita.Domain.Common;
using Orbita.Domain.Enums;

namespace Orbita.Domain.Entities;

/// <summary>Workload row imported from the TSP or added during project delivery.</summary>
public sealed class WorkTask : ProjectEntity
{
    public string Reference { get; set; } = string.Empty;
    public int Position { get; set; }
    public ElementOrigin Origin { get; set; } = ElementOrigin.Tsp;

    // Immutable source-workbook values.
    public int? TspRow { get; set; }
    public string TspAxisCode { get; set; } = string.Empty;
    public string TspLot { get; set; } = string.Empty;
    public string TspPhase { get; set; } = string.Empty;
    public string TspStep { get; set; } = string.Empty;
    public string TspTitle { get; set; } = string.Empty;
    public string Assumption { get; set; } = string.Empty;
    public string DeliverableMarker { get; set; } = string.Empty;
    public bool IsSection { get; set; }
    public bool HasDeliverableColumn { get; set; }

    // Mutable delivery values.
    public string? Title { get; set; }
    public Guid? AxisId { get; set; }
    public Guid? PhaseId { get; set; }
    public string? Lot { get; set; }
    public Guid? TaskTypeId { get; set; }
    public bool TaskTypeWasExplicitlySet { get; set; }
    public ProjectTaskStatus? Status { get; private set; }
    public decimal? Progress { get; private set; }
    public decimal? RemainingEffort { get; set; }
    public DateOnly? PlannedStartDate { get; private set; }
    public DateOnly? PlannedEndDate { get; private set; }
    public string Comment { get; set; } = string.Empty;
    public DateOnly? ValidatedOn { get; private set; }
    public string? AiOptimizationType { get; set; }
    public string? AiComment { get; set; }
    public decimal? AiRevisedEffort { get; set; }
    public Guid? InvoiceMilestoneId { get; set; }

    public List<TaskEffort> Efforts { get; set; } = [];
    public List<TimeEntry> TimeEntries { get; set; } = [];
    public List<ScopeChange> ScopeChanges { get; set; } = [];

    public bool IsCancelled => LatestCancellationOrReinstatement()?.Kind == ScopeChangeKind.Cancellation;

    public ScopeChange? LatestCancellationOrReinstatement() =>
        ScopeChanges
            .Where(change => change.Kind is ScopeChangeKind.Cancellation or ScopeChangeKind.Reinstatement)
            .OrderBy(change => change.OccurredAt)
            .LastOrDefault();

    public void EnsureEditable()
    {
        if (IsCancelled)
        {
            throw new BusinessRuleException("A cancelled task must be reinstated before it can be modified.");
        }
    }

    /// <summary>
    /// Changes task status. Validation sets progress to 100%; leaving validation resets progress to 75%,
    /// or to 0% when the task returns to ToDo.
    /// </summary>
    public bool SetStatus(ProjectTaskStatus status, DateOnly today)
    {
        EnsureEditable();
        var wasValidated = Status == ProjectTaskStatus.Validated;
        var progressWasReduced = false;

        if (status == ProjectTaskStatus.Validated)
        {
            Progress = 1m;
            RemainingEffort = null;
            ValidatedOn ??= today;
        }
        else
        {
            ValidatedOn = null;
            if (wasValidated && (Progress ?? 0m) >= 1m)
            {
                Progress = status == ProjectTaskStatus.ToDo ? 0m : 0.75m;
                progressWasReduced = true;
            }
        }

        Status = status;
        return progressWasReduced;
    }

    /// <summary>Updates progress. Reaching 100% requests validation but does not validate automatically.</summary>
    public bool SetProgress(decimal progress)
    {
        EnsureEditable();

        if (progress is < 0m or > 1m)
        {
            throw new BusinessRuleException("Task progress must be between 0% and 100%.");
        }

        if (Status == ProjectTaskStatus.Validated && progress < 1m)
        {
            Status = ProjectTaskStatus.InProgress;
            ValidatedOn = null;
        }

        Progress = progress;
        return progress == 1m && Status != ProjectTaskStatus.Validated;
    }

    /// <summary>Updates forecast dates. Dates remain editable even when the task is cancelled.</summary>
    public void SetPlannedDates(DateOnly? startDate, DateOnly? endDate)
    {
        if (startDate is not null && endDate is not null && endDate < startDate)
        {
            throw new BusinessRuleException("The planned end date cannot precede the planned start date.");
        }

        PlannedStartDate = startDate;
        PlannedEndDate = endDate;
    }
}

/// <summary>Sold and additional effort assigned to a project profile for one task.</summary>
public sealed class TaskEffort : ProjectEntity
{
    public Guid TaskId { get; set; }
    public Guid ProjectProfileId { get; set; }
    public decimal SoldEffort { get; set; }
    public decimal AdditionalEffort { get; set; }
    public string? AdditionReason { get; set; }
    public DateTimeOffset? AddedAt { get; set; }
    public Guid? AddedByUserId { get; set; }
}

/// <summary>Actual effort recorded by one project profile on one task and day.</summary>
public sealed class TimeEntry : ProjectEntity
{
    public Guid TaskId { get; set; }
    public Guid ProjectProfileId { get; set; }
    public DateOnly WorkDate { get; set; }
    public decimal PersonDays { get; set; }
    public DateTimeOffset RecordedAt { get; set; }
    public Guid? RecordedByUserId { get; set; }
}

/// <summary>Audit record for scope changes: task additions, effort additions, cancellation, reinstatement and reassignment.</summary>
public sealed class ScopeChange : ProjectEntity
{
    public Guid TaskId { get; set; }
    public Guid? ProjectProfileId { get; set; }
    public Guid? SourceProjectProfileId { get; set; }
    public ScopeChangeKind Kind { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public Guid? ActorUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public bool IsContractAmendment { get; set; }
    public decimal PersonDays { get; set; }
    public Guid? DecisionId { get; set; }
}
