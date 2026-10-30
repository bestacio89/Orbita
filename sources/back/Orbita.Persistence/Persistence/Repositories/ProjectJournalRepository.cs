using Microsoft.EntityFrameworkCore;
using Orbita.Contracts.Persistence;
using Orbita.Domain.Entities;

namespace Orbita.Persistence.Repositories;

public sealed class ProjectJournalRepository(ApplicationDbContext dbContext) : IProjectJournalRepository
{
    public async Task AddAsync(ProjectJournalEntry entry, CancellationToken cancellationToken = default)
    {
        await dbContext.ProjectJournalEntries.AddAsync(entry, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ProjectJournalEntry>> GetRecentByProjectAsync(
        Guid projectId,
        int take,
        CancellationToken cancellationToken = default)
    {
        if (take is < 1 or > 500)
        {
            throw new ArgumentOutOfRangeException(nameof(take), "The journal page size must be between 1 and 500.");
        }

        return await dbContext.ProjectJournalEntries
            .AsNoTracking()
            .Where(entry => entry.ProjectId == projectId)
            .OrderByDescending(entry => entry.OccurredAt)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
