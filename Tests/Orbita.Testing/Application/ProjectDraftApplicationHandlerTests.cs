using System.Text.Json;
using Orbita.Application.Identity;
using Orbita.Application.Projects.Drafts;
using Orbita.Contracts.Authorization;
using Orbita.Contracts.Commands.Projects;
using Orbita.Contracts.Persistence;
using Orbita.Contracts.Queries.Projects;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;
using Orbita.Domain.Tsp;
using Xunit;

namespace Orbita.Testing.Application;

public sealed class ProjectDraftApplicationHandlerTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 30, 13, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Manager_can_create_and_list_a_draft_with_canonical_metadata()
    {
        var harness = new Harness([GlobalRoleNames.Manager]);
        var create = new CreateProjectDraftCommand(
            "  Client A  ",
            "  Project A  ",
            " REF-1 ",
            DraftOrigin.Blank,
            Serialize(new TspProjectSheet()));

        var created = await new CreateProjectDraftCommandHandler(
            harness.Drafts, harness.ActorResolver, harness.CurrentUser, harness.TimeProvider)
            .Handle(create, CancellationToken.None);

        Assert.True(created.IsSuccess);
        var stored = Assert.Single(harness.Drafts.Items);
        Assert.Equal("Client A", stored.Client);
        Assert.Equal("Project A", stored.ProjectName);
        Assert.Equal("REF-1", stored.Reference);

        var listed = await new ListProjectDraftsQueryHandler(harness.Drafts, harness.ActorResolver)
            .Handle(new ListProjectDraftsQuery(), CancellationToken.None);

        Assert.True(listed.IsSuccess);
        var summary = Assert.Single(listed.Value);
        Assert.Equal(stored.Id, summary.Id);
        Assert.True(summary.IsAuthor);
    }

    [Fact]
    public async Task Coauthor_can_read_and_update_content_but_cannot_manage_invitation_or_delete()
    {
        var harness = new Harness([GlobalRoleNames.Manager]);
        var coAuthor = User.Provision(Guid.NewGuid(), Guid.NewGuid(), "Co", "Author", "co@example.test", Now);
        harness.Users.Seed(coAuthor);
        var draft = ProjectDraft.Create("Client", "Original", null, DraftOrigin.Blank, Serialize(new TspProjectSheet
        {
            Client = "Client",
            ProjectName = "Original"
        }), harness.Actor.Id, Now);
        draft.InviteCoAuthor(harness.Actor.Id, coAuthor.Id, Now.AddMinutes(1));
        draft.Version = 2;
        harness.Drafts.Seed(draft);
        harness.SwitchIdentity(coAuthor);

        var read = await new GetProjectDraftByIdQueryHandler(harness.Drafts, harness.ActorResolver)
            .Handle(new GetProjectDraftByIdQuery(draft.Id), CancellationToken.None);
        Assert.True(read.IsSuccess);
        Assert.Contains("Original", read.Value.Content, StringComparison.Ordinal);

        var update = await new UpdateProjectDraftContentCommandHandler(
            harness.Drafts, harness.ActorResolver, harness.TimeProvider)
            .Handle(new UpdateProjectDraftContentCommand(
                draft.Id,
                Serialize(new TspProjectSheet { Client = "Client", ProjectName = "Updated" }),
                draft.Version), CancellationToken.None);

        Assert.True(update.IsSuccess);
        Assert.Equal("Updated", draft.ProjectName);
        Assert.Equal(coAuthor.Id, draft.ModifiedByUserId);
        Assert.Equal(3, draft.Version);

        var invite = await new InviteDraftCoAuthorCommandHandler(
            harness.Drafts, harness.Users, harness.ActorResolver, harness.TimeProvider)
            .Handle(new InviteDraftCoAuthorCommand(draft.Id, Guid.NewGuid(), draft.Version), CancellationToken.None);
        Assert.False(invite.IsSuccess);

        var delete = await new DeleteProjectDraftCommandHandler(harness.Drafts, harness.ActorResolver)
            .Handle(new DeleteProjectDraftCommand(draft.Id, draft.Version), CancellationToken.None);
        Assert.False(delete.IsSuccess);
        Assert.Contains(draft, harness.Drafts.Items);
    }

    [Fact]
    public async Task Outsider_cannot_read_or_update_another_users_draft()
    {
        var harness = new Harness([GlobalRoleNames.Manager]);
        var draft = ProjectDraft.Create("Client", "Private", null, DraftOrigin.Blank, Serialize(new TspProjectSheet
        {
            Client = "Client",
            ProjectName = "Private"
        }), harness.Actor.Id, Now);
        harness.Drafts.Seed(draft);
        var outsider = User.Provision(Guid.NewGuid(), Guid.NewGuid(), "Outside", "User", "outside@example.test", Now);
        harness.Users.Seed(outsider);
        harness.SwitchIdentity(outsider);

        var read = await new GetProjectDraftByIdQueryHandler(harness.Drafts, harness.ActorResolver)
            .Handle(new GetProjectDraftByIdQuery(draft.Id), CancellationToken.None);
        var update = await new UpdateProjectDraftContentCommandHandler(
            harness.Drafts, harness.ActorResolver, harness.TimeProvider)
            .Handle(new UpdateProjectDraftContentCommand(draft.Id, Serialize(new TspProjectSheet()), draft.Version), CancellationToken.None);

        Assert.False(read.IsSuccess);
        Assert.False(update.IsSuccess);
        Assert.Equal("Private", draft.ProjectName);
    }

    [Fact]
    public async Task Stale_version_is_rejected_without_mutating_the_draft()
    {
        var harness = new Harness([GlobalRoleNames.Manager]);
        var draft = ProjectDraft.Create("Client", "Current", null, DraftOrigin.Blank, Serialize(new TspProjectSheet
        {
            Client = "Client",
            ProjectName = "Current"
        }), harness.Actor.Id, Now);
        draft.Version = 4;
        harness.Drafts.Seed(draft);

        var result = await new UpdateProjectDraftContentCommandHandler(
            harness.Drafts, harness.ActorResolver, harness.TimeProvider)
            .Handle(new UpdateProjectDraftContentCommand(
                draft.Id,
                Serialize(new TspProjectSheet { Client = "Client", ProjectName = "Stale" }),
                3), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Current", draft.ProjectName);
        Assert.Equal(4, draft.Version);
    }

    [Fact]
    public async Task Draft_creation_requires_the_global_draft_write_capability()
    {
        var harness = new Harness([]);
        var result = await new CreateProjectDraftCommandHandler(
            harness.Drafts, harness.ActorResolver, harness.CurrentUser, harness.TimeProvider)
            .Handle(new CreateProjectDraftCommand(
                "Client",
                "Project",
                null,
                DraftOrigin.Blank,
                Serialize(new TspProjectSheet())), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Empty(harness.Drafts.Items);
    }

    [Fact]
    public async Task Author_can_remove_coauthor_and_delete_the_draft()
    {
        var harness = new Harness([GlobalRoleNames.Manager]);
        var coAuthor = User.Provision(Guid.NewGuid(), Guid.NewGuid(), "Co", "Author", "co@example.test", Now);
        harness.Users.Seed(coAuthor);
        var draft = ProjectDraft.Create("Client", "Project", null, DraftOrigin.Blank, Serialize(new TspProjectSheet()), harness.Actor.Id, Now);
        draft.InviteCoAuthor(harness.Actor.Id, coAuthor.Id, Now.AddMinutes(1));
        draft.Version = 1;
        harness.Drafts.Seed(draft);

        var remove = await new RemoveDraftCoAuthorCommandHandler(
            harness.Drafts, harness.ActorResolver, harness.TimeProvider)
            .Handle(new RemoveDraftCoAuthorCommand(draft.Id, draft.Version), CancellationToken.None);

        Assert.True(remove.IsSuccess);
        Assert.Null(draft.CoAuthorUserId);
        Assert.Equal(2, draft.Version);

        var delete = await new DeleteProjectDraftCommandHandler(harness.Drafts, harness.ActorResolver)
            .Handle(new DeleteProjectDraftCommand(draft.Id, draft.Version), CancellationToken.None);

        Assert.True(delete.IsSuccess);
        Assert.Empty(harness.Drafts.Items);
    }

    [Fact]
    public async Task Invalid_tsp_json_is_rejected_before_a_draft_is_persisted()
    {
        var harness = new Harness([GlobalRoleNames.Manager]);
        var result = await new CreateProjectDraftCommandHandler(
            harness.Drafts, harness.ActorResolver, harness.CurrentUser, harness.TimeProvider)
            .Handle(new CreateProjectDraftCommand("Client", "Project", null, DraftOrigin.Blank, "{invalid"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Empty(harness.Drafts.Items);
    }

    private static string Serialize(TspProjectSheet project) =>
        JsonSerializer.Serialize(new TspDraft { Project = project }, new JsonSerializerOptions(JsonSerializerDefaults.Web));

    private sealed class Harness
    {
        private readonly TestCurrentUserContext _context;
        public Harness(IReadOnlyCollection<string> roles)
        {
            _context = new TestCurrentUserContext(roles);
            Users = new FakeUserRepository();
            Actor = User.Provision(_context.TenantId!.Value, _context.ObjectId!.Value, null, null, null, Now);
            Users.Seed(Actor);
            Drafts = new FakeProjectDraftRepository();
            TimeProvider = new FixedTimeProvider(Now);
            ActorResolver = new CurrentActorResolver(_context, Users, TimeProvider);
        }

        public FakeUserRepository Users { get; }
        public FakeProjectDraftRepository Drafts { get; }
        public User Actor { get; private set; }
        public TimeProvider TimeProvider { get; }
        public CurrentActorResolver ActorResolver { get; }

        public void SwitchIdentity(User user)
        {
            _context.SetIdentity(user.TenantId, user.ObjectId);
            Actor = user;
        }
    }

    private sealed class TestCurrentUserContext(IReadOnlyCollection<string> roles) : ICurrentUserContext
    {
        public bool IsAuthenticated => true;
        public Guid? UserId => null;
        public Guid? TenantId { get; private set; } = Guid.NewGuid();
        public Guid? ObjectId { get; private set; } = Guid.NewGuid();
        public string? FirstName => null;
        public string? LastName => null;
        public string? DisplayName => null;
        public string? Email => null;
        public IReadOnlyCollection<string> GlobalRoles => roles;

        public void SetIdentity(Guid tenantId, Guid objectId)
        {
            TenantId = tenantId;
            ObjectId = objectId;
        }
    }

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users = [];
        public void Seed(User user) => _users.Add(user);
        public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
            Task.FromResult(_users.SingleOrDefault(user => user.Id == userId));
        public Task<User?> GetByExternalIdentityAsync(Guid tenantId, Guid objectId, CancellationToken cancellationToken = default) =>
            Task.FromResult(_users.SingleOrDefault(user => user.TenantId == tenantId && user.ObjectId == objectId));
        public Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            _users.Add(user);
            return Task.CompletedTask;
        }
        public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            user.Version++;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeProjectDraftRepository : IProjectDraftRepository
    {
        public List<ProjectDraft> Items { get; } = [];
        public void Seed(ProjectDraft draft) => Items.Add(draft);
        public Task<ProjectDraft?> GetByIdAsync(Guid draftId, CancellationToken cancellationToken = default) =>
            Task.FromResult(Items.SingleOrDefault(draft => draft.Id == draftId));
        public Task<IReadOnlyCollection<ProjectDraft>> GetAccessibleByUserAsync(Guid userId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<ProjectDraft>>(Items
                .Where(draft => draft.CanAccess(userId))
                .OrderByDescending(draft => draft.ModifiedAt)
                .ToArray());
        public Task AddAsync(ProjectDraft draft, CancellationToken cancellationToken = default)
        {
            Items.Add(draft);
            return Task.CompletedTask;
        }
        public Task UpdateAsync(ProjectDraft draft, CancellationToken cancellationToken = default)
        {
            draft.Version++;
            return Task.CompletedTask;
        }
        public Task DeleteAsync(ProjectDraft draft, CancellationToken cancellationToken = default)
        {
            Items.Remove(draft);
            return Task.CompletedTask;
        }
    }
}
