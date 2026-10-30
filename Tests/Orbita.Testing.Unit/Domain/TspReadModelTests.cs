using Orbita.Domain.Tsp;
using Xunit;

namespace Orbita.Testing.Domain;

public sealed class TspReadModelTests
{
    [Fact]
    public void Read_model_preserves_unrecognized_fields_and_warnings()
    {
        var model = new TspReadModel
        {
            ProjectFields =
            [
                new TspProjectField { Label = "Custom field", Value = "Source value", Key = null }
            ],
            Warnings = ["Unknown workbook tab: ARCHIVE"]
        };

        Assert.Null(model.ProjectFields[0].Key);
        Assert.Equal("Source value", model.ProjectFields[0].Value);
        Assert.Contains("Unknown workbook tab: ARCHIVE", model.Warnings);
    }

    [Fact]
    public void Profile_planning_is_keyed_by_profile_and_iso_month()
    {
        var planning = new TspProfilePlanning
        {
            Months = ["2026-11"],
            Values = new Dictionary<string, Dictionary<string, decimal>>
            {
                ["profile-1"] = new() { ["2026-11"] = 4.5m }
            }
        };

        Assert.Equal(4.5m, planning.Values["profile-1"]["2026-11"]);
    }
}
