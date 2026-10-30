#nullable enable

using System.Globalization;

namespace Orbita.Domain.Tsp;

/// <summary>
/// Converts an authored TSP draft into the same semantic read contract used by imported workbooks.
/// This keeps both authoring paths on one calculation-input mapping boundary.
/// </summary>
public static class TspDraftReadModelMapper
{
    public static TspReadModel Map(TspDraft source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var months = source.MonthlyPlanning.Values
            .SelectMany(values => values.Keys)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToList();

        var hasDeliverableColumn = source.Tasks.Any(task => task.IsDeliverable);
        var aiProfileIndex = source.Profiles.FindIndex(profile =>
            string.Equals(profile.Name, "Agent IA", StringComparison.OrdinalIgnoreCase));

        return new TspReadModel
        {
            Project = new TspProjectSheet
            {
                Client = source.Project.Client,
                ProjectName = source.Project.ProjectName,
                Reference = source.Project.Reference,
                ClientContact = source.Project.ClientContact,
                Agency = source.Project.Agency,
                ServiceLine = source.Project.ServiceLine,
                Director = source.Project.Director,
                ServiceLineManager = source.Project.ServiceLineManager,
                SalesRepresentative = source.Project.SalesRepresentative,
                ContractType = source.Project.ContractType
            },
            Period = new TspPeriod
            {
                StartDate = source.Project.StartDate,
                EndDate = source.Project.EndDate
            },
            Discount = source.Discount,
            Profiles = source.Profiles.Select(profile => new TspProfile
            {
                Id = profile.Id,
                Name = profile.Name,
                LegalEntity = profile.LegalEntity,
                DailyRate = profile.DailyRate,
                DailyCostRate = profile.DailyCostRate,
                CostRatePercentage = profile.CostRatePercentage,
                Expenses = profile.Expenses,
                Collaborator = profile.Collaborator
            }).ToList(),
            Tasks = source.Tasks.Select(task => new TspReadTask
            {
                SourceId = task.Id,
                AxisCode = task.AxisCode,
                LotCode = task.LotCode,
                Phase = task.Phase,
                Step = task.Step,
                Title = task.Title,
                SoldEffortByProfile = new Dictionary<string, decimal>(task.SoldEffortByProfile, StringComparer.Ordinal),
                Assumption = task.Assumption,
                DeliverableMarker = task.IsDeliverable ? "oui" : string.Empty,
                IsSection = task.IsSection,
                HasDeliverableColumn = hasDeliverableColumn
            }).ToList(),
            Expenses = source.Expenses.Select(expense => new TspExpense
            {
                Id = expense.Id,
                Description = expense.Description,
                UnitCost = expense.UnitCost,
                Quantity = expense.Quantity,
                RebillAmount = expense.RebillAmount,
                Comment = expense.Comment
            }).ToList(),
            Purchases = source.Purchases.Select(purchase => new TspPurchase
            {
                Id = purchase.Id,
                Description = purchase.Description,
                Type = purchase.Type,
                UnitCost = purchase.UnitCost,
                Quantity = purchase.Quantity,
                ResaleAmount = purchase.ResaleAmount,
                Comment = purchase.Comment
            }).ToList(),
            Suppliers = source.Suppliers.Select(supplier => new TspSupplier
            {
                Id = supplier.Id,
                Description = supplier.Description,
                Company = supplier.Company,
                DailyCost = supplier.DailyCost,
                PersonDays = supplier.PersonDays,
                DailyRate = supplier.DailyRate,
                Comment = supplier.Comment
            }).ToList(),
            InvoiceMilestones = source.InvoiceMilestones.Select(milestone => new TspInvoiceMilestone
            {
                Id = milestone.Id,
                Name = milestone.Name,
                Percentage = milestone.Percentage,
                Month = milestone.Month,
                AxisCode = milestone.AxisCode
            }).ToList(),
            Metrics = source.Metrics.Select(metric => new TspMetric
            {
                Step = metric.Step,
                Percentage = metric.Percentage
            }).ToList(),
            SoldPlanning = new TspProfilePlanning
            {
                Months = months,
                Values = source.Profiles.ToDictionary(
                    profile => profile.Id,
                    profile => source.MonthlyPlanning.TryGetValue(profile.Id, out var values)
                        ? new Dictionary<string, decimal>(values, StringComparer.Ordinal)
                        : new Dictionary<string, decimal>(StringComparer.Ordinal),
                    StringComparer.Ordinal)
            },
            ProfilePlanningSource = source.MonthlyPlanning.Count > 0 ? "PLANNING" : null,
            References = new TspReadReferenceData
            {
                Phases = [.. source.Phases],
                Steps = [.. source.Steps]
            },
            AxisNames = source.Tasks
                .Select(task => task.AxisCode)
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .Distinct(StringComparer.Ordinal)
                .ToDictionary(code => code, BusinessAxisName, StringComparer.Ordinal),
            Dates = new TspProjectDates
            {
                StartDate = source.Project.StartDate,
                ContractualEndDate = source.Project.EndDate,
                ForecastEndDate = source.Project.EndDate
            },
            Settings = new TspReadSettings
            {
                StartMonth = months.FirstOrDefault()
                    ?? source.Project.StartDate?.ToString("yyyy-MM", CultureInfo.InvariantCulture),
                MonthCount = months.Count > 0 ? months.Count : 13,
                AiProfileIndex = aiProfileIndex,
                Grouping = "id"
            }
        };
    }

    private static string BusinessAxisName(string code) =>
        Orbita.Domain.Calculations.Reference.Common.ReferentielsMetier.NomAxeParDefaut(code);
}
