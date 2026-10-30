using Orbita.Domain.Common;
using Orbita.Domain.Entities;
using Xunit;

namespace Orbita.Testing.Domain;

public sealed class DomainFoundationTests
{
    [Theory]
    [InlineData("OP00123 - CLIENT - TSP - V3", "OP:00123")]
    [InlineData("op-00123 - CLIENT - TSP - V4", "OP:00123")]
    public void Project_identity_prefers_opportunity_number(string reference, string expected)
    {
        Assert.Equal(expected, ProjectIdentity.Create(reference, "Client", "Project"));
    }

    [Fact]
    public void Project_identity_ignores_proposal_version_when_using_reference()
    {
        var versionThree = ProjectIdentity.Create("CLIENT - TSP - V3", "Client", "Project");
        var versionFour = ProjectIdentity.Create("CLIENT - TSP - V4", "Client", "Project");

        Assert.Equal(versionThree, versionFour);
    }

    [Fact]
    public void Project_identity_falls_back_to_normalized_client_and_project_name()
    {
        var first = ProjectIdentity.Create(null, "Société Étoile", "Projet Alpha");
        var second = ProjectIdentity.Create(null, "SOCIETE ETOILE", "PROJET ALPHA");

        Assert.Equal(first, second);
    }

    [Fact]
    public void Text_normalization_removes_diacritics_and_collapses_whitespace()
    {
        Assert.Equal("camille martin", TextNormalizer.Normalize("  CAMILLE   MARTÍN "));
    }

    [Fact]
    public void Calendar_uses_iso_months_and_handles_year_boundaries()
    {
        Assert.Equal("2027-01", BusinessCalendar.AddMonths("2026-12", 1));
        Assert.Equal(-1, BusinessCalendar.MonthsBetween("2027-01", "2026-12"));
    }

    [Fact]
    public void Calendar_calculates_french_working_days_excluding_public_holidays()
    {
        Assert.Equal(4, BusinessCalendar.WorkingDaysBetween(
            new DateOnly(2026, 5, 4),
            new DateOnly(2026, 5, 8)));
    }

    [Fact]
    public void Domain_entity_receives_a_non_empty_guid_identity()
    {
        var entity = new TestDomainEntity();

        Assert.NotEqual(Guid.Empty, entity.Id);
    }

    private sealed class TestDomainEntity : DomainEntity
    {
    }
}
