using Orbita.Domain.Calculations;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;
using Xunit;

namespace Orbita.Testing.Domain;

public sealed class ProjectWorkloadCalculatorTests
{
    private static readonly DateOnly Today = new(2026, 10, 30);
    private static readonly DateTimeOffset CreatedAt = new(2026, 10, 30, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Progress_is_weighted_by_planned_effort_of_validated_tasks()
    {
        var project = CreateProject();
        var profile = new ProjectProfile { Name = "Architect", LegalEntity = "FO" };
        project.Profiles.Add(profile);

        var validated = CreateTask(project.Id, "R1", profile.Id, 8m);
        validated.SetStatus(ProjectTaskStatus.Validated, Today);
        var inProgress = CreateTask(project.Id, "R2", profile.Id, 2m);
        inProgress.SetStatus(ProjectTaskStatus.InProgress, Today);
        inProgress.SetProgress(0.75m);
        project.Tasks.Add(validated);
        project.Tasks.Add(inProgress);

        var result = ProjectWorkloadCalculator.Calculate(project);

        Assert.Equal(10m, result.PlannedEffort);
        Assert.Equal(8m, result.ValidatedEffort);
        Assert.Equal(0.8m, result.Progress);
    }

    [Fact]
    public void Manual_remaining_effort_overrides_automatic_remaining_effort()
    {
        var project = CreateProject();
        var profile = new ProjectProfile { Name = "Consultant", LegalEntity = "FO" };
        project.Profiles.Add(profile);
        var task = CreateTask(project.Id, "R1", profile.Id, 5m);
        task.RemainingEffort = 7m;
        task.TimeEntries.Add(Entry(project.Id, task.Id, profile.Id, 2m));
        project.Tasks.Add(task);

        var result = ProjectWorkloadCalculator.Calculate(project);

        Assert.Equal(7m, Assert.Single(result.Tasks).RemainingEffort);
        Assert.Equal(7m, result.RemainingEffort);
        Assert.Equal(7m, Assert.Single(result.Profiles).RemainingEffort);
    }

    [Fact]
    public void Automatic_remaining_effort_is_allocated_using_each_profiles_unconsumed_workload()
    {
        var project = CreateProject();
        var overconsumedProfile = new ProjectProfile { Name = "Overconsumed", LegalEntity = "FO" };
        var remainingProfile = new ProjectProfile { Name = "Remaining", LegalEntity = "FO" };
        project.Profiles.AddRange([overconsumedProfile, remainingProfile]);

        var task = new WorkTask { ProjectId = project.Id, Reference = "R1", Title = "R1" };
        task.Efforts.AddRange([
            new TaskEffort { ProjectId = project.Id, TaskId = task.Id, ProjectProfileId = overconsumedProfile.Id, SoldEffort = 2m },
            new TaskEffort { ProjectId = project.Id, TaskId = task.Id, ProjectProfileId = remainingProfile.Id, SoldEffort = 2m }
        ]);
        task.TimeEntries.Add(Entry(project.Id, task.Id, overconsumedProfile.Id, 3m));
        project.Tasks.Add(task);

        var result = ProjectWorkloadCalculator.Calculate(project);

        Assert.Equal(1m, result.RemainingEffort);
        Assert.Equal(0m, result.Profiles.Single(profile => profile.ProjectProfileId == overconsumedProfile.Id).RemainingEffort);
        Assert.Equal(1m, result.Profiles.Single(profile => profile.ProjectProfileId == remainingProfile.Id).RemainingEffort);
    }

    [Fact]
    public void Cancelled_task_is_excluded_from_planned_progress_but_keeps_actual_effort()
    {
        var project = CreateProject();
        var profile = new ProjectProfile { Name = "Consultant", LegalEntity = "FO" };
        project.Profiles.Add(profile);
        var task = CreateTask(project.Id, "R1", profile.Id, 4m);
        task.ScopeChanges.Add(new ScopeChange
        {
            ProjectId = project.Id,
            TaskId = task.Id,
            Kind = ScopeChangeKind.Cancellation,
            OccurredAt = CreatedAt
        });
        task.TimeEntries.Add(Entry(project.Id, task.Id, profile.Id, 1.5m));
        project.Tasks.Add(task);

        var result = ProjectWorkloadCalculator.Calculate(project);

        Assert.Equal(0m, result.PlannedEffort);
        Assert.Equal(1.5m, result.ActualEffort);
        Assert.Equal(0m, result.RemainingEffort);
        Assert.True(Assert.Single(result.Tasks).IsCancelled);
    }

    [Fact]
    public void Calculation_is_pure_and_does_not_mutate_project_input()
    {
        var project = CreateProject();
        var profile = new ProjectProfile { Name = "Consultant", LegalEntity = "FO" };
        project.Profiles.Add(profile);
        var task = CreateTask(project.Id, "R1", profile.Id, 5m);
        task.SetStatus(ProjectTaskStatus.InProgress, Today);
        task.SetProgress(0.5m);
        task.TimeEntries.Add(Entry(project.Id, task.Id, profile.Id, 1m));
        project.Tasks.Add(task);

        var statusBefore = task.Status;
        var progressBefore = task.Progress;
        var remainingBefore = task.RemainingEffort;
        var first = ProjectWorkloadCalculator.Calculate(project);
        var second = ProjectWorkloadCalculator.Calculate(project);

        Assert.Equal(statusBefore, task.Status);
        Assert.Equal(progressBefore, task.Progress);
        Assert.Equal(remainingBefore, task.RemainingEffort);
        Assert.Equal(first.PlannedEffort, second.PlannedEffort);
        Assert.Equal(first.ActualEffort, second.ActualEffort);
        Assert.Equal(first.RemainingEffort, second.RemainingEffort);
        Assert.Equal(first.Progress, second.Progress);
    }

    [Fact]
    public void Daily_overload_is_detected_across_tasks_for_the_same_profile()
    {
        var project = CreateProject();
        var profile = new ProjectProfile { Name = "Consultant", LegalEntity = "FO" };
        project.Profiles.Add(profile);
        var first = CreateTask(project.Id, "R1", profile.Id, 2m);
        var second = CreateTask(project.Id, "R2", profile.Id, 2m);
        first.TimeEntries.Add(Entry(project.Id, first.Id, profile.Id, 0.6m));
        second.TimeEntries.Add(Entry(project.Id, second.Id, profile.Id, 0.5m));
        project.Tasks.AddRange([first, second]);

        var overload = Assert.Single(ProjectWorkloadCalculator.Calculate(project).DailyOverloads);

        Assert.Equal(1.1m, overload.RecordedPersonDays);
        Assert.Equal(2, overload.TaskIds.Count);
    }

    [Theory]
    [InlineData("ACHAT", 0)]
    [InlineData("CESSION", 0)]
    [InlineData("FO", 1)]
    public void Daily_overload_exemptions_follow_legal_entity(string legalEntity, int expectedOverloads)
    {
        var project = CreateProject();
        var profile = new ProjectProfile { Name = "Profile", LegalEntity = legalEntity };
        project.Profiles.Add(profile);
        var task = CreateTask(project.Id, "R1", profile.Id, 2m);
        task.TimeEntries.Add(Entry(project.Id, task.Id, profile.Id, 1.25m));
        project.Tasks.Add(task);

        Assert.Equal(expectedOverloads, ProjectWorkloadCalculator.Calculate(project).DailyOverloads.Count);
    }

    private static Project CreateProject() =>
        Project.CreateManually("Client", "Project", null, Today, Today.AddDays(30), null, null, CreatedAt);

    private static WorkTask CreateTask(Guid projectId, string reference, Guid profileId, decimal soldEffort)
    {
        var task = new WorkTask
        {
            ProjectId = projectId,
            Reference = reference,
            Title = reference
        };
        task.Efforts.Add(new TaskEffort
        {
            ProjectId = projectId,
            TaskId = task.Id,
            ProjectProfileId = profileId,
            SoldEffort = soldEffort
        });
        return task;
    }

    private static TimeEntry Entry(Guid projectId, Guid taskId, Guid profileId, decimal personDays) =>
        new()
        {
            ProjectId = projectId,
            TaskId = taskId,
            ProjectProfileId = profileId,
            WorkDate = Today,
            PersonDays = personDays
        };
}
