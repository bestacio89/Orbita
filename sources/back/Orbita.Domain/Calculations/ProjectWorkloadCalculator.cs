using Orbita.Domain.Common;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;

namespace Orbita.Domain.Calculations;

/// <summary>
/// Deterministic workload calculation over a project snapshot.
/// No persistence, clock, or external service access is permitted in this calculation boundary.
/// </summary>
public static class ProjectWorkloadCalculator
{
    public static ProjectWorkloadResult Calculate(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);

        var profiles = project.Profiles.ToDictionary(profile => profile.Id);
        var tasks = project.Tasks
            .Where(task => !task.IsSection)
            .Select(CalculateTask)
            .ToList();

        var remainingByProfile = new Dictionary<Guid, decimal>();
        foreach (var task in project.Tasks.Where(task => !task.IsSection && !task.IsCancelled && task.Status != ProjectTaskStatus.Validated))
        {
            var plannedByProfile = task.Efforts
                .GroupBy(effort => effort.ProjectProfileId)
                .ToDictionary(group => group.Key, group => group.Sum(effort => effort.SoldEffort + effort.AdditionalEffort));
            var actualByProfile = task.TimeEntries
                .GroupBy(entry => entry.ProjectProfileId)
                .ToDictionary(group => group.Key, group => group.Sum(entry => entry.PersonDays));
            var taskPlanned = plannedByProfile.Values.Sum();
            var taskActual = actualByProfile.Values.Sum();
            var taskRemaining = task.RemainingEffort ?? Math.Max(0m, taskPlanned - taskActual);
            var residualByProfile = plannedByProfile.ToDictionary(
                pair => pair.Key,
                pair => Math.Max(0m, pair.Value - actualByProfile.GetValueOrDefault(pair.Key)));
            var residualTotal = residualByProfile.Values.Sum();

            foreach (var (profileId, profilePlanned) in plannedByProfile)
            {
                var allocationWeight = residualTotal > 0m
                    ? residualByProfile[profileId] / residualTotal
                    : taskPlanned > 0m ? profilePlanned / taskPlanned : 0m;
                var profileRemaining = taskRemaining * allocationWeight;

                remainingByProfile[profileId] = remainingByProfile.GetValueOrDefault(profileId) + profileRemaining;
            }
        }

        var profileResults = project.Profiles
            .Select(profile =>
            {
                var profileEfforts = project.Tasks
                    .Where(task => !task.IsSection && !task.IsCancelled)
                    .SelectMany(task => task.Efforts)
                    .Where(effort => effort.ProjectProfileId == profile.Id)
                    .ToList();

                var planned = profileEfforts.Sum(effort => effort.SoldEffort + effort.AdditionalEffort);
                var actual = project.Tasks
                    .Where(task => !task.IsSection)
                    .SelectMany(task => task.TimeEntries)
                    .Where(entry => entry.ProjectProfileId == profile.Id)
                    .Sum(entry => entry.PersonDays);

                return new CalculatedProfileWorkload(
                    profile.Id,
                    profile.Name,
                    profile.LegalEntity,
                    planned,
                    profileEfforts.Sum(effort => effort.AdditionalEffort),
                    actual,
                    remainingByProfile.GetValueOrDefault(profile.Id),
                    profile.DailyRate,
                    profile.DailyCostRate);
            })
            .ToList();

        var overloads = project.Tasks
            .Where(task => !task.IsSection)
            .SelectMany(task => task.TimeEntries.Select(entry => new { Task = task, Entry = entry }))
            .GroupBy(item => new { item.Entry.ProjectProfileId, item.Entry.WorkDate })
            .Select(group =>
            {
                if (!profiles.TryGetValue(group.Key.ProjectProfileId, out var profile) ||
                    BusinessReferences.CanExceedOnePersonDay(profile.LegalEntity))
                {
                    return null;
                }

                var total = group.Sum(item => item.Entry.PersonDays);
                return total > 1m
                    ? new DailyWorkloadOverload(
                        group.Key.ProjectProfileId,
                        profile.Name,
                        group.Key.WorkDate,
                        total,
                        group.Select(item => item.Task.Id).Distinct().ToArray())
                    : null;
            })
            .Where(overload => overload is not null)
            .Cast<DailyWorkloadOverload>()
            .OrderBy(overload => overload.WorkDate)
            .ThenBy(overload => overload.ProfileName, StringComparer.Ordinal)
            .ToList();

        var activeTasks = tasks.Where(task => !task.IsCancelled).ToList();
        var plannedTotal = activeTasks.Sum(task => task.PlannedEffort);
        var validatedEffort = activeTasks
            .Where(task => task.Status == ProjectTaskStatus.Validated)
            .Sum(task => task.PlannedEffort);
        var actualTotal = tasks.Sum(task => task.ActualEffort);
        var remainingTotal = activeTasks.Sum(task => task.RemainingEffort);
        var progress = plannedTotal > 0m ? validatedEffort / plannedTotal : 0m;

        return new ProjectWorkloadResult(
            tasks,
            profileResults,
            overloads,
            plannedTotal,
            actualTotal,
            remainingTotal,
            validatedEffort,
            progress);
    }

    private static CalculatedTaskWorkload CalculateTask(WorkTask task)
    {
        var planned = task.Efforts.Sum(effort => effort.SoldEffort + effort.AdditionalEffort);
        var actual = task.TimeEntries.Sum(entry => entry.PersonDays);
        var cancelled = task.IsCancelled;
        var remaining = cancelled || task.Status == ProjectTaskStatus.Validated
            ? 0m
            : task.RemainingEffort ?? Math.Max(0m, planned - actual);

        return new CalculatedTaskWorkload(
            task.Id,
            task.Reference,
            task.Title ?? task.TspTitle,
            task.Status,
            cancelled,
            planned,
            task.Efforts.Sum(effort => effort.SoldEffort),
            task.Efforts.Sum(effort => effort.AdditionalEffort),
            actual,
            remaining,
            task.Progress ?? (task.Status == ProjectTaskStatus.Validated ? 1m : 0m),
            task.PlannedStartDate,
            task.PlannedEndDate,
            task.ValidatedOn);
    }
}

public sealed record ProjectWorkloadResult(
    IReadOnlyList<CalculatedTaskWorkload> Tasks,
    IReadOnlyList<CalculatedProfileWorkload> Profiles,
    IReadOnlyList<DailyWorkloadOverload> DailyOverloads,
    decimal PlannedEffort,
    decimal ActualEffort,
    decimal RemainingEffort,
    decimal ValidatedEffort,
    decimal Progress);

public sealed record CalculatedTaskWorkload(
    Guid TaskId,
    string Reference,
    string Title,
    ProjectTaskStatus? Status,
    bool IsCancelled,
    decimal PlannedEffort,
    decimal SoldEffort,
    decimal AdditionalEffort,
    decimal ActualEffort,
    decimal RemainingEffort,
    decimal DeclaredProgress,
    DateOnly? PlannedStartDate,
    DateOnly? PlannedEndDate,
    DateOnly? ValidatedOn);

public sealed record CalculatedProfileWorkload(
    Guid ProjectProfileId,
    string ProfileName,
    string LegalEntity,
    decimal PlannedEffort,
    decimal AdditionalEffort,
    decimal ActualEffort,
    decimal RemainingEffort,
    decimal DailyRate,
    decimal DailyCostRate);

public sealed record DailyWorkloadOverload(
    Guid ProjectProfileId,
    string ProfileName,
    DateOnly WorkDate,
    decimal RecordedPersonDays,
    IReadOnlyList<Guid> TaskIds);
