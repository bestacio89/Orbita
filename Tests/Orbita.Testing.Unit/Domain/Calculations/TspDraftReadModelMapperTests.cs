using Orbita.Domain.Calculations;
using Orbita.Domain.Calculations.Reference;
using Orbita.Domain.Tsp;
using Xunit;

namespace Orbita.Testing.Domain.Calculations;

public sealed class TspDraftReadModelMapperTests
{
    [Fact]
    public void Authored_draft_and_imported_read_model_share_the_same_calculation_input()
    {
        var draft = new TspDraft
        {
            Project = new TspProjectSheet
            {
                Client = "Example client",
                ProjectName = "Orbita implementation",
                Reference = "OP-123",
                StartDate = new DateOnly(2026, 11, 2),
                EndDate = new DateOnly(2026, 12, 31)
            },
            Profiles =
            [
                new TspProfile { Id = "profile-manager", Name = "Manager", DailyRate = 800m, DailyCostRate = 400m },
                new TspProfile { Id = "profile-ai", Name = "Agent IA", DailyRate = 800m, DailyCostRate = 50m }
            ],
            Tasks =
            [
                new TspTask
                {
                    Id = "task-1",
                    AxisCode = "AXE1",
                    Phase = "CONSTRUCTION",
                    Title = "Implementation",
                    SoldEffortByProfile = new Dictionary<string, decimal>
                    {
                        ["profile-manager"] = 2m,
                        ["profile-ai"] = 1m
                    },
                    IsDeliverable = true
                }
            ],
            MonthlyPlanning = new Dictionary<string, Dictionary<string, decimal>>
            {
                ["profile-manager"] = new() { ["2026-11"] = 2m },
                ["profile-ai"] = new() { ["2026-11"] = 1m }
            },
            Phases = ["CONSTRUCTION"],
            Steps = ["Development"]
        };

        var readModel = TspDraftReadModelMapper.Map(draft);
        var calculationInput = TspCalculationInputMapper.Map(readModel);
        var result = MoteurCalcul.Calculer(calculationInput, new DateOnly(2026, 10, 30));

        Assert.Equal("Example client", readModel.Project.Client);
        Assert.Equal(1, readModel.Settings.AiProfileIndex);
        Assert.True(Assert.Single(readModel.Tasks).HasDeliverableColumn);
        Assert.Equal([2m, 1m], Assert.Single(calculationInput.Tasks).Charges);
        Assert.Equal(3m, result.Synthese.Jours);
    }
}
