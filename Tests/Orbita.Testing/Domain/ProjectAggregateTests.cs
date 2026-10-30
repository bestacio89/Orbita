using Orbita.Domain.Common;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;
using Xunit;

namespace Orbita.Testing.Domain;

public sealed class ProjectAggregateTests
{
    private static readonly DateOnly Start = new(2026, 11, 2);
    private static readonly DateOnly End = new(2027, 2, 26);
    private static readonly DateTimeOffset CreatedAt = new(2026, 10, 30, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Manual_project_creation_establishes_identity_dates_and_default_settings()
    {
        var project = Project.CreateManually(
            "  Acme  ",
            "  Platform Modernization ",
            " OP00123 - ACME - TSP - V3 ",
            Start,
            End,
            null,
            Guid.NewGuid(),
            CreatedAt);

        Assert.Equal("Acme", project.Client);
        Assert.Equal("Platform Modernization", project.Name);
        Assert.Equal("OP:00123", project.UniqueKey);
        Assert.Equal(Start, project.StartDate);
        Assert.Equal(End, project.ContractualEndDate);
        Assert.Equal(End, project.ForecastEndDate);
        Assert.Equal("Forfait", project.ContractType);
        Assert.Equal(new[] { "Atelier", "Livrable" }, project.TaskTypes.Select(taskType => taskType.Code).ToArray());
        Assert.Equal(ProjectOrigin.ManualCreation, project.Origin);
        Assert.Equal(ProjectStatus.Preparation, project.Status);
        Assert.Equal("2026-11", project.Settings.FirstPlanningMonth);
        Assert.True(project.Settings.MonthCount >= 13);
    }

    [Theory]
    [InlineData("", "Project")]
    [InlineData("Client", " ")]
    public void Manual_project_creation_requires_client_and_name(string client, string name)
    {
        Assert.Throws<BusinessRuleException>(() =>
            Project.CreateManually(client, name, null, Start, End, null, null, CreatedAt));
    }

    [Fact]
    public void Manual_project_creation_rejects_invalid_date_range()
    {
        Assert.Throws<BusinessRuleException>(() =>
            Project.CreateManually("Client", "Project", null, End, Start, null, null, CreatedAt));
    }

    [Fact]
    public void Display_status_is_derived_from_start_date_or_recorded_work()
    {
        var project = Project.CreateManually("Client", "Project", null, Start, End, null, null, CreatedAt);

        Assert.Equal(ProjectStatus.Preparation, project.GetDisplayStatus(Start.AddDays(-1), false));
        Assert.Equal(ProjectStatus.InProgress, project.GetDisplayStatus(Start.AddDays(-1), true));
        Assert.Equal(ProjectStatus.InProgress, project.GetDisplayStatus(Start, false));
    }

    [Fact]
    public void Archived_status_takes_precedence_over_derived_status()
    {
        var project = Project.CreateManually("Client", "Project", null, Start, End, null, null, CreatedAt);
        project.Archive();

        Assert.Equal(ProjectStatus.Archived, project.GetDisplayStatus(End.AddDays(10), true));
    }

    [Fact]
    public void Project_assignment_is_unique_per_user()
    {
        var project = Project.CreateManually("Client", "Project", null, Start, End, null, null, CreatedAt);
        var userId = Guid.NewGuid();

        var assignment = project.AssignUser(userId, ProjectRole.ProjectManager, CreatedAt, null);

        Assert.Equal(project.Id, assignment.ProjectId);
        Assert.Equal(userId, assignment.UserId);
        Assert.Throws<BusinessRuleException>(() =>
            project.AssignUser(userId, ProjectRole.Contributor, CreatedAt, null));
    }

    [Fact]
    public void User_identity_requires_both_tenant_and_object_ids()
    {
        Assert.Throws<ArgumentException>(() =>
            User.Provision(Guid.Empty, Guid.NewGuid(), "Ada", "Lovelace", "ada@example.test", CreatedAt));
        Assert.Throws<ArgumentException>(() =>
            User.Provision(Guid.NewGuid(), Guid.Empty, "Ada", "Lovelace", "ada@example.test", CreatedAt));
    }

    [Fact]
    public void User_profile_changes_do_not_change_external_identity()
    {
        var tenantId = Guid.NewGuid();
        var objectId = Guid.NewGuid();
        var user = User.Provision(tenantId, objectId, "Ada", "Lovelace", "ada@example.test", CreatedAt);

        user.UpdateProfile("Augusta", "King", "augusta@example.test");

        Assert.Equal(tenantId, user.TenantId);
        Assert.Equal(objectId, user.ObjectId);
        Assert.Equal("Augusta King", user.DisplayName);
    }
}
