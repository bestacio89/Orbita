#nullable enable

using System.Globalization;
using Orbita.Domain.Calculations.Reference.Tsp;

namespace Orbita.Domain.Tsp;

/// <summary>
/// Converts the reference-compatible workbook interpretation into Orbita's English domain read contract.
/// Profile identifiers are deterministic ordinals because the source workbook represents profiles by column order.
/// </summary>
public static class TspReadModelMapper
{
    public static TspReadModel Map(TspLu source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var profileIds = Enumerable.Range(0, source.Profiles.Count)
            .ToDictionary(index => index, ProfileId);

        return new TspReadModel
        {
            Project = new TspProjectSheet
            {
                Client = source.Fiche.Client ?? string.Empty,
                ProjectName = source.Fiche.Projet ?? string.Empty,
                Reference = source.Fiche.Reference ?? string.Empty,
                ClientContact = source.Fiche.Contact ?? string.Empty,
                Agency = source.Fiche.Agence ?? string.Empty,
                ServiceLine = source.Fiche.ServiceLine ?? string.Empty,
                Director = source.Fiche.Directeur ?? string.Empty,
                ServiceLineManager = source.Fiche.ResponsableSL ?? string.Empty,
                SalesRepresentative = source.Fiche.Commercial ?? string.Empty,
                ContractType = source.Fiche.TypeContrat ?? "Forfait"
            },
            ProjectFields = source.Fiche.Champs.Select(field => new TspProjectField
            {
                Label = field.Label,
                Value = field.Value,
                Key = field.Key
            }).ToList(),
            Period = new TspPeriod
            {
                StartDate = source.Periode.Start,
                EndDate = source.Periode.End
            },
            Discount = source.Remise,
            DiscountWasSpecified = source.RemiseSaisie,
            InitialRevenue = source.CaInitial,
            ManagementPercentage = source.PctPilotage,
            ProvisionPercentage = source.PctProvision,
            Profiles = source.Profiles.Select((profile, index) => new TspProfile
            {
                Id = ProfileId(index),
                Name = profile.Nom,
                LegalEntity = profile.Entite,
                DailyRate = profile.Tjm,
                DailyCostRate = profile.Cjm,
                CostRatePercentage = profile.Prcs,
                Expenses = profile.Frais,
                Collaborator = profile.Collaborateur
            }).ToList(),
            AgencyRates = source.Grille.Select((rate, index) => new TspAgencyRate
            {
                Order = index,
                Profile = rate.Profil,
                ParisDailyRate = rate.TjmParis,
                ParisDailyCostRate = rate.CjmParis,
                RegionalDailyRate = rate.TjmRegion,
                RegionalDailyCostRate = rate.CjmRegion
            }).ToList(),
            Tasks = source.Tasks.Select(task => new TspReadTask
            {
                SourceId = task.Uid,
                SourceRow = task.Ligne,
                AxisCode = task.Id,
                LotCode = task.Lot,
                Phase = task.Phase,
                Step = task.Etape,
                Title = task.Label,
                SoldEffortByProfile = task.Charges
                    .Select((effort, index) => new { effort, index })
                    .Where(item => profileIds.ContainsKey(item.index))
                    .ToDictionary(item => profileIds[item.index], item => item.effort, StringComparer.Ordinal),
                Assumption = task.Hypothese,
                DeliverableMarker = task.Livrable,
                IsSection = task.Section,
                HasDeliverableColumn = task.LivCol
            }).ToList(),
            PhasePlanning = new TspPhasePlanning
            {
                Source = source.PlanningPhases.Source,
                Rows = source.PlanningPhases.Rows.Select(row => new TspPhasePlanningRow
                {
                    Phase = row.Phase,
                    DailyCostRate = row.Cjm,
                    MonthlyValues = new Dictionary<string, decimal>(row.Vals, StringComparer.Ordinal)
                }).ToList()
            },
            Expenses = source.Frais.Select((expense, index) => new TspExpense
            {
                Id = $"expense-{index.ToString(CultureInfo.InvariantCulture)}",
                Description = expense.Libelle,
                UnitCost = expense.Cu,
                Quantity = expense.Qte,
                RebillAmount = expense.Refacture,
                Comment = expense.Commentaire
            }).ToList(),
            Purchases = source.Achats.Select((purchase, index) => new TspPurchase
            {
                Id = $"purchase-{index.ToString(CultureInfo.InvariantCulture)}",
                Description = purchase.Designation,
                Type = purchase.Type,
                UnitCost = purchase.Cu,
                Quantity = purchase.Qte,
                ResaleAmount = purchase.Revente,
                Comment = purchase.Commentaire
            }).ToList(),
            Suppliers = source.Prestas.Select((supplier, index) => new TspSupplier
            {
                Id = $"supplier-{index.ToString(CultureInfo.InvariantCulture)}",
                Description = supplier.Designation,
                Company = supplier.Societe,
                DailyCost = supplier.CoutJour,
                PersonDays = supplier.Nbj,
                DailyRate = supplier.Tjm,
                Comment = supplier.Commentaire
            }).ToList(),
            InvoiceMilestones = source.Echeancier.Select(milestone => new TspInvoiceMilestone
            {
                Id = milestone.N.ToString(CultureInfo.InvariantCulture),
                Name = milestone.Nom,
                Percentage = milestone.Pct,
                Month = milestone.Mois,
                AxisCode = milestone.AxeCode
            }).ToList(),
            Metrics = source.Metrique.Select(metric => new TspMetric
            {
                Step = metric.Etape,
                Percentage = metric.Pct
            }).ToList(),
            SoldPlanning = new TspProfilePlanning
            {
                Months = [.. source.PlanningRef.Months],
                Values = source.PlanningRef.Rows
                    .Select((values, index) => new { values, index })
                    .Where(item => profileIds.ContainsKey(item.index))
                    .ToDictionary(
                        item => profileIds[item.index],
                        item => source.PlanningRef.Months
                            .Select((month, monthIndex) => new { month, monthIndex })
                            .Where(month => month.monthIndex < item.values.Count)
                            .ToDictionary(month => month.month, month => item.values[month.monthIndex], StringComparer.Ordinal),
                        StringComparer.Ordinal)
            },
            ProfilePlanningSource = source.PlanningSource,
            References = new TspReadReferenceData
            {
                Phases = [.. source.Ref.Phases],
                Steps = [.. source.Ref.Etapes]
            },
            AxisNames = new Dictionary<string, string>(source.Axes, StringComparer.Ordinal),
            Dates = new TspProjectDates
            {
                StartDate = source.Dates.Debut,
                ContractualEndDate = source.Dates.FinContrat,
                ForecastEndDate = source.Dates.FinPrevue
            },
            Settings = new TspReadSettings
            {
                StartMonth = source.Settings.StartMonth,
                MonthCount = source.Settings.NbMonths,
                AiProfileIndex = source.Settings.IaProfile,
                Grouping = source.Settings.GroupBy
            },
            Warnings = [.. source.Warnings]
        };
    }

    private static string ProfileId(int index) => $"profile-{index.ToString(CultureInfo.InvariantCulture)}";
}
