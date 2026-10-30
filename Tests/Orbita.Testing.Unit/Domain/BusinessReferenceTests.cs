using Orbita.Domain.Common;
using Orbita.Domain.Enums;
using Xunit;

namespace Orbita.Testing.Domain;

public sealed class BusinessReferenceTests
{
    [Theory]
    [InlineData("Atelier de cadrage", "Atelier")]
    [InlineData("[Livrable] dossier d'architecture", "Livrable")]
    [InlineData("Analyse technique", "")]
    public void Task_type_detection_preserves_tsp_vocabulary(string title, string expected)
    {
        Assert.Equal(expected, BusinessReferences.DetectTaskType(title, null));
    }

    [Fact]
    public void Deliverable_column_is_authoritative_when_present()
    {
        Assert.Equal(string.Empty, BusinessReferences.DetectTaskType("Livrable - dossier", null, true));
        Assert.Equal("Livrable", BusinessReferences.DetectTaskType("Analyse", "oui", true));
    }

    [Fact]
    public void Legacy_risk_labels_are_normalized_without_changing_current_values()
    {
        Assert.Equal("Modérée", BusinessReferences.NormalizeRiskProbability("Moyenne"));
        Assert.Equal("Moyenne", BusinessReferences.NormalizeRiskImpact("Moyen"));
        Assert.Equal("Très forte", BusinessReferences.NormalizeRiskProbability("Très forte"));
    }

    [Fact]
    public void Paris_date_uses_paris_offset_instead_of_utc_calendar_date()
    {
        var instant = new DateTimeOffset(2026, 10, 29, 23, 30, 0, TimeSpan.Zero);

        Assert.Equal(new DateOnly(2026, 10, 30), ParisTimeZone.GetDate(instant));
    }

    [Fact]
    public void French_number_format_is_culture_independent()
    {
        Assert.Equal("1 234,5", FrenchNumberFormatter.PersonDays(1234.5m));
        Assert.Equal("1 235 €", FrenchNumberFormatter.Euros(1234.5m));
    }
}
