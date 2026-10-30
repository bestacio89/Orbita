using Orbita.Domain.Entities;
using Orbita.Domain.Enums;
using Orbita.Domain.Tsp;
using Xunit;

namespace Orbita.Testing.Domain;

public sealed class FinancialAndImportTests
{
    [Fact]
    public void Blank_tsp_uses_template_reference_lists_and_normalizes_project_fields()
    {
        var draft = TspTemplateDefaults.CreateBlank("  Acme ", " Platform ", " OP00123 ");

        Assert.Equal("Acme", draft.Project.Client);
        Assert.Equal("Platform", draft.Project.ProjectName);
        Assert.Equal("OP00123", draft.Project.Reference);
        Assert.NotEmpty(draft.Phases);
        Assert.NotEmpty(draft.Steps);
        Assert.NotEmpty(draft.Metrics);
    }

    [Fact]
    public void Notification_delivery_transitions_are_explicit()
    {
        var notification = new ProjectNotification();
        var sentAt = new DateTimeOffset(2026, 10, 30, 10, 0, 0, TimeSpan.Zero);

        notification.MarkSent(sentAt);

        Assert.Equal(NotificationDeliveryStatus.Sent, notification.DeliveryStatus);
        Assert.Equal(sentAt, notification.SentAt);

        notification.MarkFailed();

        Assert.Equal(NotificationDeliveryStatus.Failed, notification.DeliveryStatus);
        Assert.Null(notification.SentAt);

        notification.ResetDelivery();

        Assert.Equal(NotificationDeliveryStatus.Pending, notification.DeliveryStatus);
    }

    [Fact]
    public void Actual_expense_defaults_to_other_category()
    {
        var expense = new ActualExpense();

        Assert.Equal(ExpenseCategory.Other, expense.Category);
    }
}
