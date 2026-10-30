using Franz.Common.DependencyInjection;
using Orbita.Domain.Entities;

namespace Orbita.Contracts.Persistence;

public interface IProjectJournalRepository : IScopedDependency
{
    Task AddAsync(ProjectJournalEntry entry, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ProjectJournalEntry>> GetRecentByProjectAsync(
        Guid projectId,
        int take,
        CancellationToken cancellationToken = default);
}
