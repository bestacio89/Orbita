using Franz.Common.DependencyInjection;
using Orbita.Domain.Entities;

namespace Orbita.Contracts.Persistence;

public interface IProjectDraftRepository : IScopedDependency
{
    Task<ProjectDraft?> GetByIdAsync(Guid draftId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ProjectDraft>> GetAccessibleByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(ProjectDraft draft, CancellationToken cancellationToken = default);
    Task UpdateAsync(ProjectDraft draft, CancellationToken cancellationToken = default);
    Task DeleteAsync(ProjectDraft draft, CancellationToken cancellationToken = default);
}
