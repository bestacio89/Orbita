using Orbita.Domain.Common;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;
using Xunit;

namespace Orbita.Testing.Domain;

public sealed class GovernanceModelTests
{
    private static readonly DateTimeOffset CreatedAt = new(2026, 10, 30, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Comment_can_only_be_edited_by_its_original_author()
    {
        var authorId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var comment = ProjectComment.Create(
            Guid.NewGuid(),
            "Task",
            Guid.NewGuid(),
            authorId,
            "Initial comment",
            CreatedAt);

        Assert.Throws<BusinessRuleException>(() =>
            comment.Edit(otherUserId, "Changed by another project member", CreatedAt.AddMinutes(1)));

        comment.Edit(authorId, "Updated by author", CreatedAt.AddMinutes(1));

        Assert.Equal("Updated by author", comment.Text);
        Assert.Equal(CreatedAt.AddMinutes(1), comment.ModifiedAt);
    }

    [Fact]
    public void Comment_author_rule_is_independent_of_project_role()
    {
        var authorId = Guid.NewGuid();
        var comment = ProjectComment.Create(
            Guid.NewGuid(),
            "Task",
            Guid.NewGuid(),
            authorId,
            "Private authorship",
            CreatedAt);

        // A manager or administrator has no special override in the domain ownership rule.
        Assert.Throws<BusinessRuleException>(() => comment.EnsureAuthor(Guid.NewGuid()));
    }

    [Fact]
    public void Clearing_workshop_date_clears_invitation_state()
    {
        var workshop = new Workshop();
        workshop.SetDate(new DateOnly(2026, 11, 5));
        workshop.SetInvitationSent(true, new DateOnly(2026, 10, 30));

        workshop.SetDate(null);

        Assert.False(workshop.InvitationSent);
        Assert.Null(workshop.InvitationSentOn);
    }

    [Fact]
    public void Workshop_end_time_must_follow_start_time()
    {
        var workshop = new Workshop();

        Assert.Throws<BusinessRuleException>(() =>
            workshop.SetTime(new TimeOnly(15, 0), new TimeOnly(14, 0)));
    }

    [Fact]
    public void Risk_criticality_is_probability_times_impact()
    {
        var risk = new ProjectRisk
        {
            Probability = RiskProbability.High,
            Impact = RiskImpact.Critical
        };

        Assert.Equal(12, risk.Criticality);
    }
}
