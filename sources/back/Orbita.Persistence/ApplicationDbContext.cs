using Franz.Common.Business.Domain;
using Franz.Common.EntityFramework;
using Franz.Common.Mediator.Dispatchers;
using Microsoft.EntityFrameworkCore;
using Orbita.Domain.Entities;

namespace Orbita.Persistence;

public sealed class ApplicationDbContext : DbContextBase
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IDispatcher dispatcher) : base(options, dispatcher)
    {
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectAssignment> ProjectAssignments => Set<ProjectAssignment>();
    public DbSet<ProjectDraft> ProjectDrafts => Set<ProjectDraft>();
    public DbSet<User> Users => Set<User>();
    public DbSet<ExternalIdentity> ExternalIdentities => Set<ExternalIdentity>();
    public DbSet<ProjectJournalEntry> ProjectJournalEntries => Set<ProjectJournalEntry>();

    public DbSet<Absence> Absences => Set<Absence>();
    public DbSet<ProjectAddedPerson> ProjectAddedPeople => Set<ProjectAddedPerson>();
    public DbSet<ActualPurchase> ActualPurchases => Set<ActualPurchase>();
    public DbSet<ActualExpense> ActualExpenses => Set<ActualExpense>();
    public DbSet<ExpenseImport> ExpenseImports => Set<ExpenseImport>();
    public DbSet<IndicatorSnapshot> IndicatorSnapshots => Set<IndicatorSnapshot>();
    public DbSet<ProjectNotification> ProjectNotifications => Set<ProjectNotification>();
    public DbSet<Workshop> Workshops => Set<Workshop>();
    public DbSet<WorkshopRecipient> WorkshopRecipients => Set<WorkshopRecipient>();
    public DbSet<Deliverable> Deliverables => Set<Deliverable>();
    public DbSet<ProjectComment> ProjectComments => Set<ProjectComment>();
    public DbSet<WeeklyStatusPoint> WeeklyStatusPoints => Set<WeeklyStatusPoint>();
    public DbSet<ProjectAction> ProjectActions => Set<ProjectAction>();
    public DbSet<ProjectDecision> ProjectDecisions => Set<ProjectDecision>();
    public DbSet<ProjectRisk> ProjectRisks => Set<ProjectRisk>();
    public DbSet<ForecastProfile> ForecastProfiles => Set<ForecastProfile>();
    public DbSet<PlanningDate> PlanningDates => Set<PlanningDate>();
    public DbSet<ProjectMilestone> ProjectMilestones => Set<ProjectMilestone>();
    public DbSet<PlanningElement> PlanningElements => Set<PlanningElement>();
    public DbSet<ReferencePlan> ReferencePlans => Set<ReferencePlan>();
    public DbSet<ReferencePlanTaskLine> ReferencePlanTaskLines => Set<ReferencePlanTaskLine>();
    public DbSet<ReferencePlanDate> ReferencePlanDates => Set<ReferencePlanDate>();
    public DbSet<ProjectProfile> ProjectProfiles => Set<ProjectProfile>();
    public DbSet<AgencyRate> AgencyRates => Set<AgencyRate>();
    public DbSet<SoldProfilePlanning> SoldProfilePlannings => Set<SoldProfilePlanning>();
    public DbSet<SoldPhasePlanning> SoldPhasePlannings => Set<SoldPhasePlanning>();
    public DbSet<SoldPhaseMonth> SoldPhaseMonths => Set<SoldPhaseMonth>();
    public DbSet<PurchaseBudgetLine> PurchaseBudgetLines => Set<PurchaseBudgetLine>();
    public DbSet<ExpenseBudgetLine> ExpenseBudgetLines => Set<ExpenseBudgetLine>();
    public DbSet<ProjectSupplier> ProjectSuppliers => Set<ProjectSupplier>();
    public DbSet<ProjectMetric> ProjectMetrics => Set<ProjectMetric>();
    public DbSet<InvoiceMilestone> InvoiceMilestones => Set<InvoiceMilestone>();
    public DbSet<InvoiceMilestonePhase> InvoiceMilestonePhases => Set<InvoiceMilestonePhase>();
    public DbSet<ProjectField> ProjectFields => Set<ProjectField>();
    public DbSet<TspImportRecord> TspImportRecords => Set<TspImportRecord>();
    public DbSet<ProjectAxis> ProjectAxes => Set<ProjectAxis>();
    public DbSet<ProjectLot> ProjectLots => Set<ProjectLot>();
    public DbSet<ProjectPhase> ProjectPhases => Set<ProjectPhase>();
    public DbSet<ProjectStep> ProjectSteps => Set<ProjectStep>();
    public DbSet<ProjectTaskType> ProjectTaskTypes => Set<ProjectTaskType>();
    public DbSet<AiReferenceValue> AiReferenceValues => Set<AiReferenceValue>();
    public DbSet<WorkTask> WorkTasks => Set<WorkTask>();
    public DbSet<TaskEffort> TaskEfforts => Set<TaskEffort>();
    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();
    public DbSet<ScopeChange> ScopeChanges => Set<ScopeChange>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // DomainEntity and ProjectEntity are code-reuse bases, not persistence inheritance.
        // Detach each concrete entity from the CLR base hierarchy so EF creates one table per
        // domain entity instead of a shared TPH/TPT hierarchy.
        var domainEntityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(entityType =>
                typeof(DomainEntity).IsAssignableFrom(entityType.ClrType) &&
                entityType.ClrType != typeof(DomainEntity) &&
                entityType.ClrType != typeof(ProjectEntity))
            .ToArray();

        foreach (var entityType in domainEntityTypes)
        {
            modelBuilder.Entity(entityType.ClrType).HasBaseType((Type?)null);
        }

        modelBuilder.Ignore<ProjectEntity>();
        modelBuilder.Ignore<DomainEntity>();
        modelBuilder.Ignore<Entity<Guid>>();

        // Every Franz-backed domain entity uses the same optimistic-concurrency contract.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.FindProperty(nameof(DomainEntity.Version)) is not null)
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(DomainEntity.Version))
                    .IsConcurrencyToken();
            }

            if (typeof(ProjectEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasIndex(nameof(ProjectEntity.ProjectId));
            }
        }
    }
}
