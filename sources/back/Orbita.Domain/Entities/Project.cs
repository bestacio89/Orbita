using Orbita.Domain.Common;
using Orbita.Domain.Enums;
using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

/// <summary>
/// Project aggregate root. It owns project-scoped assignments and project configuration;
/// delivery, financial and governance entities are attached in their respective model slices.
/// </summary>
public sealed class Project : DomainEntity
{
    private readonly List<ProjectAssignment> _assignments = [];

    private Project()
    {
    }

    public string? Code { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Client { get; private set; } = string.Empty;
    public string? Reference { get; private set; }
    public string UniqueKey { get; private set; } = string.Empty;
    public string? Agency { get; private set; }
    public string? ServiceLine { get; private set; }
    public string? Director { get; private set; }
    public string? ServiceLineManager { get; private set; }
    public string? SalesRepresentative { get; private set; }
    public string? ClientContact { get; private set; }
    public string? ContractType { get; private set; }
    public ProjectStatus Status { get; private set; } = ProjectStatus.Preparation;
    public DateOnly? StartDate { get; private set; }
    public DateOnly? ContractualEndDate { get; private set; }
    public DateOnly? ForecastEndDate { get; private set; }
    public decimal Discount { get; private set; }
    public bool DiscountWasSpecified { get; private set; }
    public decimal? InitialRevenue { get; private set; }
    public decimal? ManagementPercentage { get; private set; }
    public decimal? ProvisionPercentage { get; private set; }
    public ProjectOrigin Origin { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? CreatedByUserId { get; private set; }
    public ProjectSettings Settings { get; private set; } = new();
    public List<ProjectTaskType> TaskTypes { get; private set; } = [];

    // Project-owned persistence collections. Application queries load only the navigations needed by each use case.
    public List<ProjectField> Fields { get; private set; } = [];
    public List<TspImportRecord> TspImports { get; private set; } = [];
    public List<ProjectProfile> Profiles { get; private set; } = [];
    public List<AgencyRate> AgencyRates { get; private set; } = [];
    public List<SoldProfilePlanning> SoldProfilePlanning { get; private set; } = [];
    public List<SoldPhasePlanning> SoldPhasePlanning { get; private set; } = [];
    public List<PurchaseBudgetLine> PurchaseBudget { get; private set; } = [];
    public List<ExpenseBudgetLine> ExpenseBudget { get; private set; } = [];
    public List<ProjectSupplier> Suppliers { get; private set; } = [];
    public List<ProjectMetric> Metrics { get; private set; } = [];
    public List<InvoiceMilestone> InvoiceMilestones { get; private set; } = [];
    public List<ProjectAxis> Axes { get; private set; } = [];
    public List<ProjectLot> Lots { get; private set; } = [];
    public List<ProjectPhase> Phases { get; private set; } = [];
    public List<ProjectStep> Steps { get; private set; } = [];
    public List<AiReferenceValue> AiReferenceValues { get; private set; } = [];
    public List<WorkTask> Tasks { get; private set; } = [];
    public List<ActualPurchase> ActualPurchases { get; private set; } = [];
    public List<ActualExpense> ActualExpenses { get; private set; } = [];
    public List<ProjectAction> Actions { get; private set; } = [];
    public List<ProjectDecision> Decisions { get; private set; } = [];
    public List<ProjectRisk> Risks { get; private set; } = [];
    public List<ProjectMilestone> Milestones { get; private set; } = [];
    public List<PlanningDate> PlanningDates { get; private set; } = [];
    public List<PlanningElement> PlanningElements { get; private set; } = [];
    public List<ReferencePlan> ReferencePlans { get; private set; } = [];
    public List<ForecastProfile> ForecastProfiles { get; private set; } = [];
    public List<IndicatorSnapshot> IndicatorSnapshots { get; private set; } = [];
    public List<ProjectNotification> Notifications { get; private set; } = [];
    public List<ProjectJournalEntry> JournalEntries { get; private set; } = [];

    public IReadOnlyCollection<ProjectAssignment> Assignments => _assignments.AsReadOnly();

    public static Project CreateManually(
        string client,
        string name,
        string? reference,
        DateOnly startDate,
        DateOnly contractualEndDate,
        string? contractType,
        Guid? createdByUserId,
        DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(client))
        {
            throw new BusinessRuleException("The client name is required.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleException("The project name is required.");
        }

        if (contractualEndDate < startDate)
        {
            throw new BusinessRuleException("The contractual end date cannot precede the project start date.");
        }

        var project = new Project
        {
            Client = client.Trim(),
            Name = name.Trim(),
            Reference = NormalizeOptional(reference),
            ContractType = NormalizeOptional(contractType) ?? "Forfait",
            StartDate = startDate,
            ContractualEndDate = contractualEndDate,
            ForecastEndDate = contractualEndDate,
            Status = ProjectStatus.Preparation,
            Origin = ProjectOrigin.ManualCreation,
            CreatedAt = createdAt,
            CreatedByUserId = createdByUserId,
            Settings = ProjectSettings.ForPeriod(startDate, contractualEndDate)
        };

        project.RecalculateUniqueKey();
        project.AddCoreTaskTypes();
        return project;
    }

    public ProjectStatus GetDisplayStatus(DateOnly today, bool hasTimeEntries)
    {
        if (Status == ProjectStatus.Archived)
        {
            return ProjectStatus.Archived;
        }

        return hasTimeEntries || (StartDate is { } start && start <= today)
            ? ProjectStatus.InProgress
            : ProjectStatus.Preparation;
    }

    public void SetDates(DateOnly? startDate, DateOnly? contractualEndDate, DateOnly? forecastEndDate)
    {
        if (startDate is { } start && contractualEndDate is { } contractualEnd && contractualEnd < start)
        {
            throw new BusinessRuleException("The contractual end date cannot precede the project start date.");
        }

        if (startDate is { } forecastStart && forecastEndDate is { } forecastEnd && forecastEnd < forecastStart)
        {
            throw new BusinessRuleException("The forecast end date cannot precede the project start date.");
        }

        StartDate = startDate;
        ContractualEndDate = contractualEndDate;
        ForecastEndDate = forecastEndDate;
    }

    public void UpdateCommercialIdentity(string? reference, string client, string name)
    {
        if (string.IsNullOrWhiteSpace(client))
        {
            throw new BusinessRuleException("The client name is required.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleException("The project name is required.");
        }

        Client = client.Trim();
        Name = name.Trim();
        Reference = NormalizeOptional(reference);
        RecalculateUniqueKey();
    }

    public void Archive() => Status = ProjectStatus.Archived;

    public void Unarchive() => Status = ProjectStatus.Preparation;

    public ProjectAssignment AssignUser(Guid userId, ProjectRole role, DateTimeOffset assignedAt, Guid? grantedByUserId)
    {
        if (_assignments.Any(assignment => assignment.UserId == userId))
        {
            throw new BusinessRuleException("A user can have only one project assignment.");
        }

        var assignment = new ProjectAssignment(Id, userId, role, assignedAt, grantedByUserId);
        _assignments.Add(assignment);
        return assignment;
    }

    public bool RemoveAssignment(Guid userId)
    {
        var assignment = _assignments.SingleOrDefault(candidate => candidate.UserId == userId);
        return assignment is not null && _assignments.Remove(assignment);
    }

    public void AddCoreTaskTypes()
    {
        var order = 0;
        foreach (var (code, name) in ProjectTaskTypeCodes.CoreTypes)
        {
            if (!TaskTypes.Any(taskType => taskType.Code == code))
            {
                TaskTypes.Add(new ProjectTaskType
                {
                    ProjectId = Id,
                    Code = code,
                    Name = name,
                    IsCoreType = true,
                    Order = order++
                });
            }
        }
    }

    public void RecalculateUniqueKey() => UniqueKey = ProjectIdentity.Create(Reference, Client, Name);

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
