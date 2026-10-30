#nullable enable

using System.Text.Json;
using Orbita.Domain.Common;
using Orbita.Domain.Tsp;

namespace Orbita.Application.Projects.Drafts;

/// <summary>
/// Validates and canonicalizes the versioned TSP JSON boundary before draft persistence.
/// Detailed business validation remains in the TSP analysis/calculation workflow.
/// </summary>
internal static class TspDraftPayload
{
    private const int MaximumPayloadCharacters = 2_000_000;
    private const int MaximumProfiles = 60;
    private const int MaximumTasks = 3_000;
    private const int MaximumListEntries = 300;
    private const int MaximumMonths = 120;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        MaxDepth = 64
    };

    public static NormalizedTspDraft ForCreation(
        string content,
        string client,
        string projectName,
        string? reference)
    {
        var draft = Deserialize(content);
        draft.Project.Client = RequireText(client, "Client");
        draft.Project.ProjectName = RequireText(projectName, "Project name");
        draft.Project.Reference = reference?.Trim() ?? string.Empty;
        return ToNormalized(draft);
    }

    public static NormalizedTspDraft ForUpdate(string content)
    {
        var draft = Deserialize(content);
        var client = RequireText(draft.Project.Client, "Client");
        var projectName = RequireText(draft.Project.ProjectName, "Project name");
        return ToNormalized(draft withProject(client, projectName));
    }

    private static TspDraft withProject(TspDraft draft, string client, string projectName)
    {
        draft.Project.Client = client;
        draft.Project.ProjectName = projectName;
        draft.Project.Reference = string.IsNullOrWhiteSpace(draft.Project.Reference)
            ? string.Empty
            : draft.Project.Reference.Trim();
        return draft;
    }

    private static TspDraft Deserialize(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new BusinessRuleException("A TSP draft payload is required.");
        }

        if (content.Length > MaximumPayloadCharacters)
        {
            throw new BusinessRuleException($"A TSP draft cannot exceed {MaximumPayloadCharacters:N0} characters.");
        }

        TspDraft draft;
        try
        {
            draft = JsonSerializer.Deserialize<TspDraft>(content, JsonOptions)
                ?? throw new BusinessRuleException("The TSP draft payload cannot be null.");
        }
        catch (JsonException exception)
        {
            throw new BusinessRuleException($"The TSP draft payload is not valid JSON: {exception.Message}");
        }

        if (draft.FormatVersion != 1)
        {
            throw new BusinessRuleException($"Unsupported TSP draft format version: {draft.FormatVersion}.");
        }

        if (draft.Project is null ||
            draft.Profiles is null ||
            draft.Tasks is null ||
            draft.MonthlyPlanning is null ||
            draft.InvoiceMilestones is null ||
            draft.Purchases is null ||
            draft.Suppliers is null ||
            draft.Expenses is null ||
            draft.Metrics is null ||
            draft.Phases is null ||
            draft.Steps is null)
        {
            throw new BusinessRuleException("The TSP draft payload contains a missing required section.");
        }

        if (draft.Profiles.Count > MaximumProfiles ||
            draft.Tasks.Count > MaximumTasks ||
            draft.InvoiceMilestones.Count > MaximumListEntries ||
            draft.Purchases.Count > MaximumListEntries ||
            draft.Suppliers.Count > MaximumListEntries ||
            draft.Expenses.Count > MaximumListEntries ||
            draft.Metrics.Count > MaximumListEntries ||
            draft.Phases.Count > MaximumListEntries ||
            draft.Steps.Count > MaximumListEntries ||
            draft.MonthlyPlanning.Count > MaximumProfiles ||
            draft.MonthlyPlanning.Values.Any(months => months is null || months.Count > MaximumMonths))
        {
            throw new BusinessRuleException("The TSP draft payload exceeds the supported collection limits.");
        }

        return draft;
    }

    private static NormalizedTspDraft ToNormalized(TspDraft draft)
    {
        var normalized = JsonSerializer.Serialize(draft, JsonOptions);
        return new NormalizedTspDraft(
            normalized,
            draft.Project.Client,
            draft.Project.ProjectName,
            string.IsNullOrWhiteSpace(draft.Project.Reference) ? null : draft.Project.Reference.Trim());
    }

    private static string RequireText(string? value, string field)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new BusinessRuleException($"{field} is required.");
        }

        if (normalized.Length > 400)
        {
            throw new BusinessRuleException($"{field} cannot exceed 400 characters.");
        }

        return normalized;
    }
}

internal sealed record NormalizedTspDraft(
    string Content,
    string Client,
    string ProjectName,
    string? Reference);
