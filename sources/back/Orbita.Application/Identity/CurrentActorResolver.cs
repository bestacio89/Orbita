#nullable enable

using Orbita.Contracts.Authorization;
using Orbita.Contracts.Persistence;
using Orbita.Domain.Entities;

namespace Orbita.Application.Identity;

/// <summary>Resolves a validated Entra identity to its persisted Orbita user.</summary>
public sealed class CurrentActorResolver(
    ICurrentUserContext currentUser,
    IUserRepository users,
    TimeProvider timeProvider)
{
    public async Task<User?> GetOrProvisionAsync(CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated ||
            currentUser.TenantId is not { } tenantId ||
            currentUser.ObjectId is not { } objectId)
        {
            return null;
        }

        var user = await users.GetByExternalIdentityAsync(tenantId, objectId, cancellationToken);
        if (user is null)
        {
            user = User.Provision(
                tenantId,
                objectId,
                currentUser.FirstName,
                currentUser.LastName,
                currentUser.Email,
                timeProvider.GetUtcNow());

            await users.AddAsync(user, cancellationToken);
            return user;
        }

        if (!user.IsActive)
        {
            return null;
        }

        if (user.FirstName != currentUser.FirstName ||
            user.LastName != currentUser.LastName ||
            user.Email != currentUser.Email)
        {
            user.UpdateProfile(currentUser.FirstName, currentUser.LastName, currentUser.Email);
            await users.UpdateAsync(user, cancellationToken);
        }

        return user;
    }
}
