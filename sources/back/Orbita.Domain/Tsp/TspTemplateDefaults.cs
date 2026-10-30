namespace Orbita.Domain.Tsp;

/// <summary>Reference lists and defaults from the commercial TSP workbook template.</summary>
public static class TspTemplateDefaults
{
    // These values are retained verbatim because they are business data in the external French TSP workbook format.
    public static IReadOnlyList<string> Phases { get; } =
    [
        "PILOTAGE", "CONCEPTION", "CONSTRUCTION", "VALIDATION", "FINALISATION", "GARANTIE", "TMA", "PROVISION"
    ];

    public static IReadOnlyList<string> Steps { get; } =
    [
        "Pilotage projet", "Analyse de l'existant", "Conception générale", "Conception technique détaillée",
        "Conception fonctionnelle détaillée", "Ergonomie/Graphisme", "Cahier de recette", "Développement et tests unitaires",
        "Tests d'intégration / Recette interne", "Livraison / installation / Mise en œuvre", "Pilote",
        "Généralisation", "Support", "Reprise de contenu", "Assistance recette", "Formation",
        "Garantie (suite recette - 3 mois)", "Provision pour risque", "Hors abaques"
    ];

    public static IReadOnlyList<(string Step, decimal Percentage)> Metrics { get; } =
    [
        ("Pilotage projet", 0.20m), ("Conception générale", 0.05m),
        ("Conception fonctionnelle détaillée", 0.25m), ("Conception technique détaillée", 0.20m),
        ("Cahier de recette", 0.15m), ("Développement et tests unitaires", 1.00m),
        ("Tests d'intégration / Recette interne", 0.20m), ("Livraison / installation / Mise en œuvre", 0.10m),
        ("Assistance recette", 0.10m), ("Garantie (suite recette - 3 mois)", 0.06m)
    ];

    public static IReadOnlyList<string> SuggestedProfiles { get; } =
    [
        "Consultant Junior", "Consultant Change", "Consultant Technique", "Consultant Data", "Expert senior",
        "Architecte", "Architecte Senior", "Chef de Projet", "Chef de Projet Senior", "Directeur de projet",
        "Consultant Fonctionnel", "Consultant Fonctionnel Senior", "Expert fonctionnel", "Référent", "Manager",
        "Dev Confirmé", "Dev Junior", "Testeur"
    ];

    public static IReadOnlyList<string> PurchaseTypes { get; } =
    [ "CSP Microsoft", "JetBrains", "Logiciel", "ManageEngine", "Matériel", "Skillisy", "Rescue", "Run2C", "Support - MCO" ];

    public static TspDraft CreateBlank(string client, string projectName, string? reference)
    {
        if (string.IsNullOrWhiteSpace(client)) throw new ArgumentException("A client name is required.", nameof(client));
        if (string.IsNullOrWhiteSpace(projectName)) throw new ArgumentException("A project name is required.", nameof(projectName));
        return new TspDraft
        {
            Project = new TspProjectSheet { Client = client.Trim(), ProjectName = projectName.Trim(), Reference = reference?.Trim() ?? string.Empty },
            Phases = Phases.ToList(), Steps = Steps.ToList(),
            Metrics = Metrics.Select(metric => new TspMetric { Step = metric.Step, Percentage = metric.Percentage }).ToList()
        };
    }
}