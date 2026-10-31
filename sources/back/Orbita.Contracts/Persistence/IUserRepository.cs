using Franz.Common.DependencyInjection;
using Orbita.Domain.Entities;

namespace Orbita.Contracts.Persistence;

public interface IUserRepository : IScopedDependency
{
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User?> GetByExternalIdentityAsync(string issuer, string subject, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
}
