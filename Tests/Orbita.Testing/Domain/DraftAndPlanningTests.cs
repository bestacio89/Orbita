using Orbita.Domain.Common;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;
using Orbita.Domain.Tsp;
using Xunit;

namespace Orbita.Testing.Domain;

public sealed class DraftAndPlanningTests
{
    private static readonly DateTimeOffset CreatedAt = new(2026, 10, 30, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Draft_access_is_limited_to_author_and_invited_coauthor()
    {
        var author = Guid.NewGuid();
        var coAuthor = Guid.NewGuid();
        var outsider = Guid.NewGuid();
        var draft = ProjectDraft.Create("Client", "Project", null, DraftOrigin.Blank, "{}", author, CreatedAt);

        Assert.True(draft.CanAccess(author));
        Assert.False(draft.CanAccess(coAuthor));
        Assert.False(draft.CanAccess(outsider));

        draft.InviteCoAuthor(author, coAuthor, CreatedAt.AddMinutes(1));

        Assert.True(draft.CanAccess(coAuthor));
        Assert.Throws<BusinessRuleException>(() => draft.EnsureCanAccess(outsider));
    }

    [Fact]
    public void Only_draft_author_can_manage_coauthor_access()
    {
        var author = Guid.NewGuid();
        var coAuthor = Guid.NewGuid();
        var draft = ProjectDraft.Create("Client", "Project", null, DraftOrigin.Blank, "{}", author, CreatedAt);
        draft.InviteCoAuthor(author, coAuthor, CreatedAt.AddMinutes(1));

        Assert.Throws<BusinessRuleException>(() =>
            draft.InviteCoAuthor(coAuthor, Guid.NewGuid(), CreatedAt.AddMinutes(2)));
    }

    [Fact]
    public void Draft_coauthor_can_edit_content_but_not_change_ownership()
    {
        var author = Guid.NewGuid();
        var coAuthor = Guid.NewGuid();
        var draft = ProjectDraft.Create("Client", "Project", null, DraftOrigin.Blank, "{}", author, CreatedAt);
        draft.InviteCoAuthor(author, coAuthor, CreatedAt.AddMinutes(1));

        draft.UpdateContent(coAuthor, "{\"formatVersion\":1}", CreatedAt.AddMinutes(2));

        Assert.Equal("{\"formatVersion\":1}", draft.Content);
        Assert.Equal(coAuthor, draft.ModifiedByUserId);
    }

    [Fact]
    public void Person_identity_ignores_name_order_accents_and_case()
    {
        Assert.Equal(
            PersonIdentity.CreateKey("MARTÍN Camille"),
            PersonIdentity.CreateKey("camille martin"));
    }

    [Fact]
    public void Blank_tsp_draft_starts_with_a_versioned_format_and_empty_workload()
    {
        var draft = new TspDraft();

        Assert.Equal(1, draft.FormatVersion);
        Assert.Empty(draft.Tasks);
        Assert.Empty(draft.Profiles);
        Assert.Empty(draft.MonthlyPlanning);
    }
}
