using Orbita.Application.Identity;
using Orbita.Application.Projects.Commands;
using Orbita.Contracts.Authorization;
using Orbita.Contracts.Commands.Projects;
using Orbita.Contracts.Persistence;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;
using Xunit;

namespace Orbita.Testing.Application;

public sealed class ProjectCommandHandlerTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 30, 13, 30, 0, TimeSpan.Zero);
    private static readonly DateOnly Start = new(2026, 11, 1);
    private static readonly DateOnly End = new(2027, 1, 31);

    [Fact]
    public async Task Unassigned_project_manager_gets_not_found_before_stale_version_is_evaluated()
    {
        var harness = new Harness([]);
        var project = CreateProject();
        project.AssignUser(Guid.NewGuid(), ProjectRole.ProjectManager, Now, null);
        harness.Projects.Seed(project);

        var result = await new UpdateProjectDatesCommandHandler(
            harness.Projects,
            harness.ActorResolver,
            harness.CurrentUser)
            .Handle(new UpdateProjectDatesCommand(
                project.Id,
                ExpectedVersion: 99,
                StartDate: Start,
                ContractualEndDate: End,
                ForecastEndDate: End.AddDays(15)), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("NotFound", result.Error?.Code);
        Assert.Equal(0, project.Version);
        Assert.Equal(End, project.ForecastEndDate);
    }

    [Fact]
    public async Task Assigned_project_manager_receives_conflict_for_stale_version()
    {
        var harness = new Harness([]);
        var project = CreateProject();
        project.AssignUser(harness.Actor.Id, ProjectRole.ProjectManager, Now, harness.Actor.Id);
        harness.Projects.Seed(project);

        var result = await new UpdateProjectDatesCommandHandler(
            harness.Projects,
            harness.ActorResolver,
            harness.CurrentUser)
            .Handle(new UpdateProjectDatesCommand(
                project.Id,
                ExpectedVersion: 99,
                StartDate: Start,
                ContractualEndDate: End,
                ForecastEndDate: End.AddDays(15)), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Conflict", result.Error?.Code);
        Assert.Equal(0, project.Version);
        Assert.Equal(End, project.ForecastEndDate);
    }

    [Fact]
    public async Task Assigned_project_manager_can_update_dates_with_the_current_version()
    {
        var harness = new Harness([]);
        var project = CreateProject();
        project.AssignUser(harness.Actor.Id, ProjectRole.ProjectManager, Now, harness.Actor.Id);
        harness.Projects.Seed(project);
        var forecastEnd = End.AddDays(15);

        var result = await new UpdateProjectDatesCommandHandler(
            harness.Projects,
            harness.ActorResolver,
            harness.CurrentUser)
            .Handle(new UpdateProjectDatesCommand(
                project.Id,
                ExpectedVersion: 0,
                StartDate: Start,
                ContractualEndDate: End,
                ForecastEndDate: forecastEnd), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(forecastEnd, project.ForecastEndDate);
        Assert.Equal(1, project.Version);
    }

    [Fact]
    public async Task Project_manager_cannot_assign_an_inactive_user()
    {
        var harness = new Harness([]);
        var project = CreateProject();
        project.AssignUser(harness.Actor.Id, ProjectRole.ProjectManager, Now, harness.Actor.Id);
        harness.Projects.Seed(project);

        var inactive = User.Provision(Guid.NewGuid(), Guid.NewGuid(), "Inactive", "User", "inactive@example.test", Now);
        inactive.Deactivate();
        harness.Users.Seed(inactive);

        var result = await new AssignProjectUserCommandHandler(
            harness.Projects,
            harness.Users,
            harness.ActorResolver,
            harness.CurrentUser,
            harness.TimeProvider)
            .Handle(new AssignProjectUserCommand(
                project.Id,
                inactive.Id,
                ProjectRole.Contributor,
                ExpectedVersion: 0), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("NotFound", result.Error?.Code);
        Assert.DoesNotContain(project.Assignments, assignment => assignment.UserId == inactive.Id);
        Assert.Equal(0, project.Version);
    }

    private static Project CreateProject() =>
        Project.CreateManually("Client", "Project", null, Start, End, "Forfait", null, Now);

    private sealed class Harness(IReadOnlyCollection<string> roles)
    {
        private readonly TestCurrentUserContext _context = new(roles);
        public FakeUserRepository Users { get; } = new();
        public FakeProjectRepository Projects { get; } = new();
        public User Actor { get; } = User.Provision(
            Guid.NewGuid(), Guid.NewGuid(), null, null, null, Now);
        public TimeProvider TimeProvider { get; } = new FixedTimeProvider(Now);
        public ICurrentUserContext CurrentUser => _context;
        public CurrentActorResolver ActorResolver { get; }

        public Harness(IReadOnlyCollection<string> roles) : this(roles, initialize: true)
        {
        }

        private Harness(IReadOnlyCollection<string> roles, bool initialize) : this(roles)
        {
            _context.SetIdentity(Actor.TenantId, Actor.ObjectId);
            Users.Seed(Actor);
            ActorResolver = new CurrentActorResolver(_context, Users, TimeProvider);
        }
    }

    private sealed class TestCurrentUserContext(IReadOnlyCollection<string> roles) : ICurrentUserContext
    {
        public bool IsAuthenticated => true;
        public Guid? UserId => null;
        public Guid? TenantId { get; private set; }
        public Guid? ObjectId { get; private set; }
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

    private sealed class FakeProjectRepository : IProjectRepository
    {
        public List<Project> Items { get; } = [];
        public void Seed(Project project) => Items.Add(project);
        public Task<Project?> GetByIdAsync(Guid projectId, CancellationToken cancellationToken = default) =>
            Task.FromResult(Items.SingleOrDefault(project => project.Id == projectId));
        public Task<Project?> GetByUniqueKeyAsync(string uniqueKey, CancellationToken cancellationToken = default) =>
            Task.FromResult(Items.SingleOrDefault(project => project.UniqueKey == uniqueKey));
        public Task<IReadOnlyCollection<Project>> GetAccessibleByUserAsync(Guid userId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<Project>>(Items.Where(project => project.Assignments.Any(assignment => assignment.UserId == userId)).ToArray());
        public Task<IReadOnlyCollection<Project>> GetAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<Project>>(Items.ToArray());
        public Task AddAsync(Project project, CancellationToken cancellationToken = default)
        {
            Items.Add(project);
            return Task.CompletedTask;
        }
        public Task UpdateAsync(Project project, CancellationToken cancellationToken = default)
        {
            project.Version++;
            return Task.CompletedTask;
        }
    }
}
