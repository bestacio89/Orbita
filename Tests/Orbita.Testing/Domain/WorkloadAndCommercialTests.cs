using Orbita.Domain.Common;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;
using Orbita.Domain.Services;
using Xunit;

namespace Orbita.Testing.Domain;

public sealed class WorkloadAndCommercialTests
{
    private static readonly DateOnly Today = new(2026, 10, 30);

    [Fact]
    public void Validating_a_task_sets_progress_to_one_hundred_percent()
    {
        var task = new WorkTask();
        task.SetProgress(0.5m);

        task.SetStatus(ProjectTaskStatus.Validated, Today);

        Assert.Equal(1m, task.Progress);
        Assert.Equal(Today, task.ValidatedOn);
    }

    [Theory]
    [InlineData(ProjectTaskStatus.InProgress, 0.75)]
    [InlineData(ProjectTaskStatus.InAcceptance, 0.75)]
    [InlineData(ProjectTaskStatus.ToDo, 0)]
    public void Leaving_validated_status_reduces_progress(ProjectTaskStatus nextStatus, double expectedProgress)
    {
        var task = new WorkTask();
        task.SetStatus(ProjectTaskStatus.Validated, Today);

        var reduced = task.SetStatus(nextStatus, Today);

        Assert.True(reduced);
        Assert.Equal((decimal)expectedProgress, task.Progress);
        Assert.Null(task.ValidatedOn);
    }

    [Fact]
    public void Reaching_one_hundred_percent_does_not_automatically_validate_a_task()
    {
        var task = new WorkTask();
        task.SetStatus(ProjectTaskStatus.InProgress, Today);

        Assert.True(task.SetProgress(1m));
        Assert.Equal(ProjectTaskStatus.InProgress, task.Status);
    }

    [Fact]
    public void Cancelled_task_cannot_be_updated_until_reinstated()
    {
        var task = new WorkTask();
        task.ScopeChanges.Add(new ScopeChange
        {
            Kind = ScopeChangeKind.Cancellation,
            OccurredAt = Today.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)
        });

        Assert.True(task.IsCancelled);
        Assert.Throws<BusinessRuleException>(() => task.SetStatus(ProjectTaskStatus.InProgress, Today));

        task.ScopeChanges.Add(new ScopeChange
        {
            Kind = ScopeChangeKind.Reinstatement,
            OccurredAt = Today.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)
        });

        Assert.False(task.IsCancelled);
    }

    [Fact]
    public void Planned_task_dates_must_be_consistent()
    {
        var task = new WorkTask();

        Assert.Throws<BusinessRuleException>(() =>
            task.SetPlannedDates(new DateOnly(2026, 11, 10), new DateOnly(2026, 11, 1)));
    }

    [Theory]
    [InlineData("FO", 1.0, false)]
    [InlineData("FO", 1.25, true)]
    [InlineData("ACHAT", 2.0, false)]
    [InlineData("CESSION", 3.0, false)]
    public void Daily_effort_limit_exempts_purchase_and_assignment_profiles(string legalEntity, double total, bool shouldReject)
    {
        var profile = new ProjectProfile { Name = "Consultant", LegalEntity = legalEntity };

        var exception = Record.Exception(() => DailyEffortPolicy.EnsureWithinDailyLimit(profile, (decimal)total));

        Assert.Equal(shouldReject, exception is BusinessRuleException);
    }

    [Fact]
    public void Invoice_amount_is_frozen_on_first_invoice_and_payment_follows_invoice()
    {
        var milestone = new InvoiceMilestone();

        milestone.Invoice(Today, 12_500m, Today);
        milestone.Invoice(Today.AddDays(-1), 15_000m, Today);

        Assert.Equal(12_500m, milestone.InvoicedAmount);
        Assert.False(milestone.IsSelectable);
        Assert.Throws<BusinessRuleException>(() => milestone.RecordPayment(Today.AddDays(-2), Today));

        milestone.RecordPayment(Today, Today);
        Assert.Equal(Today, milestone.PaidOn);
    }

    [Fact]
    public void Invoice_dates_cannot_be_in_the_future()
    {
        var milestone = new InvoiceMilestone();

        Assert.Throws<BusinessRuleException>(() => milestone.Invoice(Today.AddDays(1), 100m, Today));
    }
}
