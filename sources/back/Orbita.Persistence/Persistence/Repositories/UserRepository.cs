using Microsoft.EntityFrameworkCore;
using Orbita.Contracts.Persistence;
using Orbita.Domain.Entities;

namespace Orbita.Persistence.Repositories;

public sealed class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        dbContext.Users
            .Include(user => user.ExternalIdentities)
            .SingleOrDefaultAsync(user => user.Id == userId, cancellationToken);

    public Task<User?> GetByExternalIdentityAsync(
        string issuer,
        string subject,
        CancellationToken cancellationToken = default) =>
        dbContext.Users
            .Include(user => user.ExternalIdentities)
            .Where(user => user.ExternalIdentities.Any(identity =>
                identity.Issuer == issuer &&
                identity.Subject == subject))
            .SingleOrDefaultAsync(cancellationToken);

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
