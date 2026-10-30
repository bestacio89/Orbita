#nullable enable

using System.Globalization;
using Orbita.Domain.Calculations.Reference;
using Orbita.Domain.Tsp;

namespace Orbita.Domain.Calculations;

/// <summary>
/// Builds the deterministic calculation snapshot from the semantic TSP read contract.
/// This is the single bridge between workbook ingestion and the reference-compatible calculation engine.
/// </summary>
public static class TspCalculationInputMapper
{
    public static ProjetCalcul Map(TspReadModel source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var months = source.SoldPlanning.Months
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToList();

        var profiles = source.Profiles.Select(profile => new ProfilCalcul
        {
            Nom = profile.Name,
            Entite = profile.LegalEntity,
            Tjm = profile.DailyRate,
            Cjm = profile.DailyCostRate,
            Prcs = profile.CostRatePercentage,
            Frais = profile.Expenses,
            Collaborateur = profile.Collaborator
        }).ToList();

        var tasks = source.Tasks.Select(task => new TacheCalcul
        {
            Uid = task.SourceId,
            Ligne = task.SourceRow,
            Id = task.AxisCode,
            Lot = task.LotCode,
            Phase = task.Phase,
            Etape = task.Step,
            Label = task.Title,
            Charges = source.Profiles
                .Select(profile => task.SoldEffortByProfile.GetValueOrDefault(profile.Id))
                .ToList(),
            Section = task.IsSection,
            Livrable = task.DeliverableMarker,
            Hypothese = task.Assumption,
            LivCol = task.HasDeliverableColumn
        }).ToList();

        var planningRows = source.Profiles.Select(profile =>
        {
            source.SoldPlanning.Values.TryGetValue(profile.Id, out var values);
            return months.Select(month => values?.GetValueOrDefault(month) ?? 0m).ToList();
        }).ToList();

        var phasePlanning = new PlanningPhasesCalcul
        {
            Rows = source.PhasePlanning.Rows.Select(row => new LignePlanningPhaseCalcul
            {
                Phase = row.Phase,
                Vals = new Dictionary<string, decimal>(row.MonthlyValues, StringComparer.Ordinal)
            }).ToList()
        };

        var invoiceMilestones = source.InvoiceMilestones
            .Select((milestone, index) =>
            {
                var number = int.TryParse(milestone.Id, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
                    ? parsed
                    : index + 1;
                return new EcheanceCalcul
                {
                    N = number,
                    Nom = milestone.Name,
                    Pct = milestone.Percentage,
                    Mois = milestone.Month,
                    AxeCode = milestone.AxisCode
                };
            })
            .ToList();

        var startMonth = source.Settings.StartMonth
            ?? months.FirstOrDefault()
            ?? source.Dates.StartDate?.ToString("yyyy-MM", CultureInfo.InvariantCulture);

        return new ProjetCalcul
        {
            Remise = source.Discount,
            Profiles = profiles,
            Tasks = tasks,
            Settings = new ParametresCalcul
            {
                StartMonth = startMonth,
                NbMonths = source.Settings.MonthCount,
                IaProfile = source.Settings.AiProfileIndex,
                GroupBy = source.Settings.Grouping
            },
            Dates = new DatesProjetCalcul
            {
                Debut = source.Dates.StartDate,
                FinContrat = source.Dates.ContractualEndDate,
                FinPrevue = source.Dates.ForecastEndDate
            },
            Ref = new ReferentielCalcul
            {
                Phases = [.. source.References.Phases],
                Etapes = [.. source.References.Steps]
            },
            Axes = new Dictionary<string, string>(source.AxisNames, StringComparer.Ordinal),
            Lots = source.Tasks
                .Select(task => task.LotCode)
                .Where(lot => !string.IsNullOrWhiteSpace(lot))
                .Distinct(StringComparer.Ordinal)
                .ToDictionary(lot => lot, lot => lot, StringComparer.Ordinal),
            Frais = source.Expenses.Select(expense => new FraisBudgetCalcul
            {
                Libelle = expense.Description,
                Cu = expense.UnitCost,
                Qte = expense.Quantity,
                Refacture = expense.RebillAmount,
                Commentaire = expense.Comment
            }).ToList(),
            Achats = source.Purchases.Select(purchase => new AchatBudgetCalcul
            {
                Designation = purchase.Description,
                Type = purchase.Type,
                Cu = purchase.UnitCost,
                Qte = purchase.Quantity,
                Revente = purchase.ResaleAmount,
                Commentaire = purchase.Comment
            }).ToList(),
            Prestas = source.Suppliers.Select(supplier => new PrestataireCalcul
            {
                Designation = supplier.Description,
                Societe = supplier.Company,
                CoutJour = supplier.DailyCost,
                Nbj = supplier.PersonDays,
                Tjm = supplier.DailyRate,
                Commentaire = supplier.Comment
            }).ToList(),
            Metrique = source.Metrics.Select(metric => new MetriqueCalcul
            {
                Etape = metric.Step,
                Pct = metric.Percentage
            }).ToList(),
            PlanningRef = new PlanningProfilsCalcul
            {
                Months = months,
                Rows = planningRows
            },
            PlanningPhases = phasePlanning,
            Echeancier = invoiceMilestones
        };
    }
}
