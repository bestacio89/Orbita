using Orbita.Domain.Calculations;
using Orbita.Domain.Calculations.Reference;
using Orbita.Domain.Tsp;
using Xunit;

namespace Orbita.Testing.Domain.Calculations;

public sealed class TspCalculationInputMapperTests
{
    [Fact]
    public void Tsp_read_model_maps_profile_order_and_effort_columns_deterministically()
    {
        var source = new TspReadModel
        {
            Profiles =
            [
                new TspProfile { Id = "profile-0", Name = "Manager", DailyRate = 800m, DailyCostRate = 400m },
                new TspProfile { Id = "profile-1", Name = "Consultant", DailyRate = 700m, DailyCostRate = 350m }
            ],
            Tasks =
            [
                new TspReadTask
                {
                    SourceId = "row-1",
                    AxisCode = "AXE1",
                    Title = "Implementation",
                    SoldEffortByProfile = new Dictionary<string, decimal>
                    {
                        ["profile-0"] = 1m,
                        ["profile-1"] = 2m
                    }
                }
            ],
            SoldPlanning = new TspProfilePlanning
            {
                Months = ["2026-11", "2026-12"],
                Values = new Dictionary<string, Dictionary<string, decimal>>
                {
                    ["profile-0"] = new() { ["2026-11"] = 1m, ["2026-12"] = 0m },
                    ["profile-1"] = new() { ["2026-11"] = 1m, ["2026-12"] = 1m }
                }
            },
            Dates = new TspProjectDates
            {
                StartDate = new DateOnly(2026, 11, 2),
                ContractualEndDate = new DateOnly(2026, 12, 31),
                ForecastEndDate = new DateOnly(2026, 12, 31)
            },
            Settings = new TspReadSettings { StartMonth = "2026-11", MonthCount = 2 }
        };

        var calculationInput = TspCalculationInputMapper.Map(source);

        Assert.Equal(["2026-11", "2026-12"], calculationInput.PlanningRef.Months);
        Assert.Equal([1m, 2m], Assert.Single(calculationInput.Tasks).Charges);
        Assert.Equal([1m, 0m], calculationInput.PlanningRef.Rows[0]);
        Assert.Equal([1m, 1m], calculationInput.PlanningRef.Rows[1]);
    }

    [Fact]
    public void Imported_tsp_read_model_can_be_calculated_without_workbook_dependencies()
    {
        var source = new TspReadModel
        {
            Profiles = [new TspProfile { Id = "profile-0", Name = "Manager", DailyRate = 800m, DailyCostRate = 400m }],
            Tasks =
            [
                new TspReadTask
                {
                    SourceId = "row-1",
                    AxisCode = "AXE1",
                    Title = "Implementation",
                    SoldEffortByProfile = new Dictionary<string, decimal> { ["profile-0"] = 2m }
                }
            ],
            Dates = new TspProjectDates
            {
                StartDate = new DateOnly(2026, 11, 2),
                ContractualEndDate = new DateOnly(2026, 12, 31),
                ForecastEndDate = new DateOnly(2026, 12, 31)
            },
            Settings = new TspReadSettings { StartMonth = "2026-11" }
        };

        var input = TspCalculationInputMapper.Map(source);
        var result = MoteurCalcul.Calculer(input, new DateOnly(2026, 10, 30));

        Assert.Equal(2m, Assert.Single(result.Tasks).Charge);
        Assert.Equal(2m, result.Synthese.Jours);
    }
}
