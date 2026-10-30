using Orbita.Domain.Entities;
using Orbita.Domain.Enums;
using Orbita.Domain.Services;
using Xunit;

namespace Orbita.Testing.Domain;

public sealed class ProjectAccessPolicyTests
{
    private static readonly DateOnly Start = new(2026, 11, 2);
    private static readonly DateTimeOffset CreatedAt = new(2026, 10, 30, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Project_manager_can_only_access_projects_where_explicitly_assigned()
    {
        var managerId = Guid.NewGuid();
        var firstProject = CreateProject("First");
        var secondProject = CreateProject("Second");
        firstProject.AssignUser(managerId, ProjectRole.ProjectManager, CreatedAt, null);
        secondProject.AssignUser(Guid.NewGuid(), ProjectRole.ProjectManager, CreatedAt, null);

        Assert.True(ProjectAccessPolicy.CanAccessProject(firstProject, managerId));
        Assert.True(ProjectAccessPolicy.IsProjectManager(firstProject, managerId));
        Assert.False(ProjectAccessPolicy.CanAccessProject(secondProject, managerId));
        Assert.False(ProjectAccessPolicy.IsProjectManager(secondProject, managerId));
    }

    [Theory]
    [InlineData(ProjectRole.ProjectManager, true)]
    [InlineData(ProjectRole.Contributor, true)]
    [InlineData(ProjectRole.Steering, false)]
    [InlineData(ProjectRole.Reader, false)]
    public void Delivery_mutation_is_limited_to_managers_and_contributors(ProjectRole role, bool expected)
    {
        var userId = Guid.NewGuid();
        var project = CreateProject("Project");
        project.AssignUser(userId, role, CreatedAt, null);

        Assert.Equal(expected, ProjectAccessPolicy.CanUpdateDelivery(project, userId));
    }

    private static Project CreateProject(string name) =>
        Project.CreateManually("Client", name, null, Start, Start.AddDays(30), null, null, CreatedAt);
}
