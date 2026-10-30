namespace Orbita.Domain.Tsp;

/// <summary>Reference lists and defaults from the commercial TSP workbook template.</summary>
public static class TspTemplateDefaults
{
    public static IReadOnlyList<string> Phases { get; } =
    [
        "PILOTAGE", "DESIGN", "BUILD", "VALIDATION", "FINALIZATION", "WARRANTY", "MAINTENANCE", "CONTINGENCY"
    ];

    public static IReadOnlyList<string> Steps { get; } =
    [
        "Project management", "Current-state analysis", "High-level design", "Detailed technical design",
        "Detailed functional design", "UX and graphic design", "Acceptance test plan", "Development and unit tests",
        "Integration tests / Internal acceptance", "Delivery / Installation / Implementation", "Pilot",
        "General rollout", "Support", "Content migration", "Acceptance support", "Training",
        "Warranty (three months after acceptance)", "Risk contingency", "Out of standard scope"
    ];

    public static IReadOnlyList<(string Step, decimal Percentage)> Metrics { get; } =
    [
        ("Project management", 0.20m),
        ("High-level design", 0.05m),
        ("Detailed functional design", 0.25m),
        ("Detailed technical design", 0.20m),
        ("Acceptance test plan", 0.15m),
        ("Development and unit tests", 1.00m),
        ("Integration tests / Internal acceptance", 0.20m),
        ("Delivery / Installation / Implementation", 0.10m),
        ("Acceptance support", 0.10m),
        ("Warranty (three months after acceptance)", 0.06m)
    ];

    public static IReadOnlyList<string> SuggestedProfiles { get; } =
    [
        "Junior Consultant", "Change Consultant", "Technical Consultant", "Data Consultant", "Senior Expert",
        "Architect", "Senior Architect", "Project Manager", "Senior Project Manager", "Project Director",
        "Functional Consultant", "Senior Functional Consultant", "Functional Expert", "Lead", "Manager",
        "Experienced Developer", "Junior Developer", "Tester"
    ];

    public static IReadOnlyList<string> PurchaseTypes { get; } =
    [
        "Microsoft CSP", "JetBrains", "Software", "ManageEngine", "Hardware", "Skillisy", "Rescue", "Run2C", "Support - MCO"
    ];

    public static TspDraft CreateBlank(string client, string projectName, string? reference)
    {
        if (string.IsNullOrWhiteSpace(client))
        {
            throw new ArgumentException("A client name is required.", nameof(client));
        }

        if (string.IsNullOrWhiteSpace(projectName))
        {
            throw new ArgumentException("A project name is required.", nameof(projectName));
        }

        return new TspDraft
        {
            Project = new TspProjectSheet
            {
                Client = client.Trim(),
                ProjectName = projectName.Trim(),
                Reference = reference?.Trim() ?? string.Empty
            },
            Phases = Phases.ToList(),
            Steps = Steps.ToList(),
            Metrics = Metrics
                .Select(metric => new TspMetric { Step = metric.Step, Percentage = metric.Percentage })
                .ToList()
        };
    }
}
