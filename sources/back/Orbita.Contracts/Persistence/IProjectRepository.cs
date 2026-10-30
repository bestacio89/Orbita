using Franz.Common.DependencyInjection;
using Orbita.Domain.Entities;

namespace Orbita.Contracts.Persistence;

public interface IProjectRepository : IScopedDependency
{
    Task<Project?> GetByIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<Project?> GetByUniqueKeyAsync(string uniqueKey, CancellationToken cancellationToken = default);

    /// <summary>Returns only projects with an explicit assignment for the specified user.</summary>
    Task<IReadOnlyCollection<Project>> GetAccessibleByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Returns all projects. Callers must first pass global capability authorization.</summary>
    Task<IReadOnlyCollection<Project>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Project project, CancellationToken cancellationToken = default);
    Task UpdateAsync(Project project, CancellationToken cancellationToken = default);
}
