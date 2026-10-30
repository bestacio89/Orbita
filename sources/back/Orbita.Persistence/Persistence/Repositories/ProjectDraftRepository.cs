using Microsoft.EntityFrameworkCore;
using Orbita.Contracts.Persistence;
using Orbita.Domain.Entities;

namespace Orbita.Persistence.Repositories;

public sealed class ProjectDraftRepository(ApplicationDbContext dbContext) : IProjectDraftRepository
{
    public Task<ProjectDraft?> GetByIdAsync(Guid draftId, CancellationToken cancellationToken = default) =>
        dbContext.ProjectDrafts.SingleOrDefaultAsync(draft => draft.Id == draftId, cancellationToken);

    public async Task<IReadOnlyCollection<ProjectDraft>> GetAccessibleByUserAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await dbContext.ProjectDrafts
            .AsNoTracking()
            .Where(draft => draft.AuthorUserId == userId || draft.CoAuthorUserId == userId)
            .OrderByDescending(draft => draft.ModifiedAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(ProjectDraft draft, CancellationToken cancellationToken = default)
    {
        await dbContext.ProjectDrafts.AddAsync(draft, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ProjectDraft draft, CancellationToken cancellationToken = default)
    {
        draft.Version++;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ProjectDraft draft, CancellationToken cancellationToken = default)
    {
        dbContext.ProjectDrafts.Remove(draft);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
