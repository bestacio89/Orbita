using Microsoft.EntityFrameworkCore;
using Orbita.Contracts.Persistence;
using Orbita.Domain.Entities;

namespace Orbita.Persistence.Repositories;

public sealed class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        dbContext.Users.SingleOrDefaultAsync(user => user.Id == userId, cancellationToken);

    public Task<User?> GetByExternalIdentityAsync(Guid tenantId, Guid objectId, CancellationToken cancellationToken = default) =>
        dbContext.Users.SingleOrDefaultAsync(
            user => user.TenantId == tenantId && user.ObjectId == objectId,
            cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        user.Version++;
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
