using System.Text.RegularExpressions;
using Orbita.Domain.Common;
using Orbita.Domain.Enums;

namespace Orbita.Domain.Common;

/// <summary>
/// Controlled business vocabulary and compatibility rules used by project delivery and TSP interpretation.
/// Literal values remain French where they are persisted or exchanged with the French TSP workbook.
/// </summary>
public static partial class BusinessReferences
{
    public const string TaskStatusToDo = "À faire";
    public const string TaskStatusInProgress = "En cours";
    public const string TaskStatusBlocked = "Bloqué / En attente";
    public const string TaskStatusInAcceptance = "En recette";
    public const string TaskStatusValidated = "Validé";
    public const string TaskStatusCancelled = "Annulée";

    public static IReadOnlyList<string> TaskStatuses { get; } =
    [
        TaskStatusToDo, TaskStatusInProgress, TaskStatusBlocked, TaskStatusInAcceptance, TaskStatusValidated
    ];

    public static IReadOnlyDictionary<string, string> TaskStatusAliases { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["A faire"] = TaskStatusToDo,
            ["Test"] = TaskStatusInAcceptance
        };

    public static IReadOnlyList<decimal> ProgressSteps { get; } = [0m, 0.25m, 0.5m, 0.75m, 1m];

    public static IReadOnlyList<string> AiOptimizationLevers { get; } =
    [
        "Rédaction et génération de livrables",
        "Analyse et synthèse de données",
        "Scripts, paramétrage, automatisation",
        "Tests et recette automatisés",
        "Recherche et veille",
        "Réutilisation d’un actif existant",
        "Autre"
    ];

    public static IReadOnlyList<string> AiOverrunCauses { get; } =
    [
        "Qualité insuffisante, reprise manuelle",
        "Outil ou licence indisponible",
        "Données client inexploitables",
        "Montée en compétence",
        "Périmètre modifié",
        "Autre"
    ];

    public static IReadOnlyList<string> PurchaseStatuses { get; } = ["Commandé", "Reçu", "Facturé"];
    public static IReadOnlyList<string> ExpenseCategories { get; } = ["Transport", "Hébergement", "Repas", "Autre"];
    public static IReadOnlyList<string> LegalEntities { get; } = ["FO", "CESSION", "ACHAT", "BO", "ACHAT BO"];

    public static IReadOnlyList<string> RiskProbabilities { get; } = ["Faible", "Modérée", "Forte", "Très forte"];
    public static IReadOnlyList<string> RiskImpacts { get; } = ["Faible", "Moyenne", "Forte", "Critique"];
    public const string DefaultRiskProbability = "Modérée";
    public const string DefaultRiskImpact = "Moyenne";
    public static IReadOnlyList<string> RiskTypes { get; } = ["Retard", "Qualité", "Budget"];

    private static readonly IReadOnlyDictionary<string, string> LegacyRiskProbabilityAliases =
        new Dictionary<string, string>(StringComparer.Ordinal) { ["Moyenne"] = "Modérée" };

    private static readonly IReadOnlyDictionary<string, string> LegacyRiskImpactAliases =
        new Dictionary<string, string>(StringComparer.Ordinal) { ["Moyen"] = "Moyenne", ["Fort"] = "Forte" };

    public static string NormalizeRiskProbability(string? value) =>
        string.IsNullOrEmpty(value) ? DefaultRiskProbability : LegacyRiskProbabilityAliases.GetValueOrDefault(value, value);

    public static string NormalizeRiskImpact(string? value) =>
        string.IsNullOrEmpty(value) ? DefaultRiskImpact : LegacyRiskImpactAliases.GetValueOrDefault(value, value);

    public static string RiskSeverity(int criticality) =>
        criticality >= 8 ? "fort" : criticality >= 4 ? "moyen" : "faible";

    public const decimal DefaultBackOfficeCostRate = 450m;

    public static IReadOnlyList<string> DefaultPhases { get; } =
        ["PILOTAGE", "CONCEPTION", "CONSTRUCTION", "VALIDATION", "FINALISATION", "GARANTIE", "TMA", "PROVISION"];

    public static bool CanExceedOnePersonDay(string? legalEntity) =>
        Regex.IsMatch((legalEntity ?? string.Empty).ToUpperInvariant(), "^(ACHAT|CESSION)", RegexOptions.CultureInvariant);

    public static string DefaultAxisName(string code)
    {
        if (code == "0")
        {
            return "Pilotage (transverse)";
        }

        var match = AxisCode().Match(code);
        return match.Success ? $"Axe {match.Groups[1].Value}" : code;
    }

    /// <summary>
    /// Infers the core task type from the task title and TSP deliverable marker.
    /// When the workbook contains a deliverable column, that column is authoritative for deliverables.
    /// </summary>
    public static string DetectTaskType(string title, string? deliverableMarker, bool hasDeliverableColumn = false)
    {
        var normalizedTitle = (title ?? string.Empty).ToLowerInvariant();

        if (hasDeliverableColumn)
        {
            if (deliverableMarker == "oui")
            {
                return ProjectTaskTypeCodes.Deliverable;
            }

            return IsWorkshopTitle(normalizedTitle) ? ProjectTaskTypeCodes.Workshop : string.Empty;
        }

        if (Regex.IsMatch(normalizedTitle, @"^\s*\[\s*livrable", JavaScriptRegexOptions))
        {
            return ProjectTaskTypeCodes.Deliverable;
        }

        if (IsWorkshopTitle(normalizedTitle))
        {
            return ProjectTaskTypeCodes.Workshop;
        }

        if (Regex.IsMatch(normalizedTitle, @"^\s*livrable", JavaScriptRegexOptions) || deliverableMarker == "oui" ||
            Regex.IsMatch(normalizedTitle, @"\blivrable", JavaScriptRegexOptions))
        {
            return ProjectTaskTypeCodes.Deliverable;
        }

        return string.Empty;
    }

    private static bool IsWorkshopTitle(string title) =>
        Regex.IsMatch(title, @"^\s*\[\s*atelier", JavaScriptRegexOptions) ||
        Regex.IsMatch(title, @"^\s*(atelier|session|webinar|workshop)", JavaScriptRegexOptions) ||
        Regex.IsMatch(title, @"\batelier", JavaScriptRegexOptions);

    private const RegexOptions JavaScriptRegexOptions = RegexOptions.ECMAScript | RegexOptions.CultureInvariant;

    [GeneratedRegex(@"^axe\s*(\d+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex AxisCode();
}
