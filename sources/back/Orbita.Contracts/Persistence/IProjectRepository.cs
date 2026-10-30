using Franz.Common.DependencyInjection;
using Orbita.Domain.Entities;

namespace Orbita.Contracts.Persistence;

public interface IProjectRepository : IScopedDependency
{
    Task<Project?> GetByIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<Project?> GetByUniqueKeyAsync(string uniqueKey, CancellationToken cancellationToken = default);
    Task AddAsync(Project project, CancellationToken cancellationToken = default);
    Task UpdateAsync(Project project, CancellationToken cancellationToken = default);
}
