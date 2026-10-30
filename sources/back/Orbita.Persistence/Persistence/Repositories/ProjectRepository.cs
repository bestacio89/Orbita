using Microsoft.EntityFrameworkCore;
using Orbita.Contracts.Persistence;
using Orbita.Domain.Entities;

namespace Orbita.Persistence.Repositories;

public sealed class ProjectRepository(ApplicationDbContext dbContext) : IProjectRepository
{
    public Task<Project?> GetByIdAsync(Guid projectId, CancellationToken cancellationToken = default) =>
        dbContext.Projects
            .Include(project => project.Assignments)
            .SingleOrDefaultAsync(project => project.Id == projectId, cancellationToken);

    public Task<Project?> GetByUniqueKeyAsync(string uniqueKey, CancellationToken cancellationToken = default) =>
        dbContext.Projects.SingleOrDefaultAsync(project => project.UniqueKey == uniqueKey, cancellationToken);

    public async Task<IReadOnlyCollection<Project>> GetAccessibleByUserAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await dbContext.Projects
            .AsNoTracking()
            .Include(project => project.Assignments)
            .Where(project => project.Assignments.Any(assignment => assignment.UserId == userId))
            .OrderBy(project => project.Client)
            .ThenBy(project => project.Name)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<Project>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Projects
            .AsNoTracking()
            .Include(project => project.Assignments)
            .OrderBy(project => project.Client)
            .ThenBy(project => project.Name)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Project project, CancellationToken cancellationToken = default)
    {
        await dbContext.Projects.AddAsync(project, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Project project, CancellationToken cancellationToken = default)
    {
        project.Version++;
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
