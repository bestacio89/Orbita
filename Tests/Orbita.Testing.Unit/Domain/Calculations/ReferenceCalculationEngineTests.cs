using Orbita.Domain.Calculations.Reference;
using Xunit;

namespace Orbita.Testing.Domain.Calculations;

public sealed class ReferenceCalculationEngineTests
{
    private static readonly DateOnly ReferenceDate = new(2026, 10, 6);

    [Fact]
    public void Calculation_handles_an_empty_project_without_errors()
    {
        var project = new ProjetCalcul
        {
            Settings = new ParametresCalcul { StartMonth = "2026-11" },
            Dates = new DatesProjetCalcul
            {
                Debut = new DateOnly(2026, 11, 2),
                FinContrat = new DateOnly(2027, 2, 26),
                FinPrevue = new DateOnly(2027, 2, 26)
            }
        };

        var result = MoteurCalcul.Calculer(project, ReferenceDate);

        Assert.Empty(result.Tasks);
        Assert.All(result.Vigilance, item => Assert.Equal("ok", item.Niveau));
    }

    [Fact]
    public void Automatic_remaining_effort_is_calculated_per_profile_without_cross_profile_offset()
    {
        var project = new ProjetCalcul
        {
            Profiles =
            [
                new ProfilCalcul { Nom = "Project manager", Tjm = 800, Cjm = 400 },
                new ProfilCalcul { Nom = "Consultant", Tjm = 700, Cjm = 350 },
                new ProfilCalcul { Nom = "AI agent", Tjm = 800, Cjm = 50 }
            ],
            Tasks =
            [
                new TacheCalcul
                {
                    Uid = "task-1",
                    Id = "AXE1",
                    Phase = "CONSTRUCTION",
                    Label = "Task",
                    Charges = [1m, 2m, 0m]
                }
            ],
            Settings = new ParametresCalcul { StartMonth = "2026-09", IaProfile = 2 },
            Dates = new DatesProjetCalcul
            {
                Debut = new DateOnly(2026, 9, 1),
                FinContrat = new DateOnly(2026, 12, 31),
                FinPrevue = new DateOnly(2026, 12, 31)
            }
        };
        project.Track["task-1"] = new SuiviTacheCalcul
        {
            ConsoP = new Dictionary<int, Dictionary<string, decimal>>
            {
                [1] = new Dictionary<string, decimal>
                {
                    ["2026-09-01"] = 1m,
                    ["2026-09-02"] = 1m,
                    ["2026-09-03"] = 1m
                }
            }
        };

        var result = MoteurCalcul.Calculer(project, ReferenceDate);
        var task = Assert.Single(result.Tasks);

        Assert.Equal(1m, task.Raf);
        Assert.Equal(4m, task.Atterrissage);
        Assert.Equal(1m, task.Ecart);
    }

    [Fact]
    public void Validated_task_has_no_remaining_effort_even_when_manual_effort_was_entered()
    {
        var project = new ProjetCalcul
        {
            Profiles = [new ProfilCalcul { Nom = "Project manager", Tjm = 800, Cjm = 400 }],
            Tasks =
            [
                new TacheCalcul
                {
                    Uid = "task-1",
                    Id = "AXE1",
                    Label = "Task",
                    Charges = [2m]
                }
            ],
            Settings = new ParametresCalcul { StartMonth = "2026-09" },
            Dates = new DatesProjetCalcul
            {
                Debut = new DateOnly(2026, 9, 1),
                FinContrat = new DateOnly(2026, 12, 31),
                FinPrevue = new DateOnly(2026, 12, 31)
            }
        };
        project.Track["task-1"] = new SuiviTacheCalcul
        {
            Statut = "Validé",
            Avancement = 1m,
            Raf = 3m,
            ConsoP = new Dictionary<int, Dictionary<string, decimal>>
            {
                [0] = new Dictionary<string, decimal> { ["2026-09-01"] = 1m }
            }
        };

        var task = Assert.Single(MoteurCalcul.Calculer(project, ReferenceDate).Tasks);

        Assert.Equal(0m, task.Raf);
        Assert.Null(task.RafSaisi);
    }
}
