using System.Text;
using Orbita.Domain.Calculations.Reference.Common;
using Orbita.Persistence.Excel;
using Orbita.Testing.Shared;
using Xunit;

namespace Orbita.Testing.Infrastructure.Excel;

public sealed class TspWorkbookReaderTests
{
    private static readonly DateOnly ReferenceDate = new(2026, 10, 6);

    private static readonly string[] ComparedFields =
    [
        "fiche", "periode", "remise", "remiseSaisie", "caInitial", "pctPilotage", "pctProvision",
        "profiles", "grille", "tasks", "planningPhases", "frais", "achats", "prestas",
        "echeancier", "metrique", "planningRef", "planningSource", "ref", "axes", "dates",
        "settings", "warnings"
    ];

    [Theory]
    [InlineData("tsp-exemple-a")]
    [InlineData("tsp-exemple-b")]
    public void Workbook_reading_matches_the_reference_interpreter(string fixtureName)
    {
        using var workbook = File.OpenRead(OutilsTests.Donnees($"{fixtureName}.xlsx"));

        var result = new TspWorkbookReader().ReadReference(workbook, $"{fixtureName}.xlsx", ReferenceDate);
        var expected = OutilsTests.Lire($"{fixtureName}.lecture-attendue.json").AsObject();
        var actual = OutilsTests.VersJson(result).AsObject();

        foreach (var field in ComparedFields)
        {
            OutilsTests.VerifierIdentique(expected[field], actual[field], $"{fixtureName} > {field}");
        }
    }

    [Fact]
    public void Semantic_read_model_uses_stable_profile_keys_and_preserves_source_task_ids()
    {
        using var workbook = File.OpenRead(OutilsTests.Donnees("tsp-exemple-a.xlsx"));

        var result = new TspWorkbookReader().Read(workbook, "tsp-exemple-a.xlsx", ReferenceDate);

        Assert.NotEmpty(result.Profiles);
        Assert.Equal(Enumerable.Range(0, result.Profiles.Count).Select(index => $"profile-{index}"), result.Profiles.Select(profile => profile.Id));
        var task = result.Tasks.First(task => !task.IsSection);
        Assert.NotEmpty(task.SourceId);
        Assert.All(task.SoldEffortByProfile.Keys, key => Assert.Contains(result.Profiles, profile => profile.Id == key));
    }

    [Fact]
    public void Invalid_xlsx_payload_is_rejected_with_a_clear_domain_error()
    {
        using var invalidWorkbook = new MemoryStream(Encoding.UTF8.GetBytes("not an Excel workbook"));

        var exception = Assert.Throws<InvalidTspException>(() =>
            new TspWorkbookReader().Read(invalidWorkbook, "invalid.xlsx", ReferenceDate));

        Assert.Contains("Excel", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
