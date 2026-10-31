#nullable enable

using Orbita.Contracts.Authorization;
using Orbita.Contracts.Persistence;
using Orbita.Domain.Entities;

namespace Orbita.Application.Identity;

/// <summary>Resolves a validated issuer/subject pair to its persisted Orbita user.</summary>
public sealed class CurrentActorResolver(
    ICurrentUserContext currentUser,
    IUserRepository users,
    TimeProvider timeProvider)
{
    public async Task<User?> GetOrProvisionAsync(CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated ||
            string.IsNullOrWhiteSpace(currentUser.Issuer) ||
            string.IsNullOrWhiteSpace(currentUser.Subject) ||
            currentUser.ProviderKind is not { } providerKind)
        {
            return null;
        }

        var now = timeProvider.GetUtcNow();
        var user = await users.GetByExternalIdentityAsync(
            currentUser.Issuer,
            currentUser.Subject,
            cancellationToken);

        if (user is null)
        {
            user = User.Create(
                currentUser.FirstName,
                currentUser.LastName,
                currentUser.Email,
                now);

            user.LinkExternalIdentity(ExternalIdentity.Link(
                user.Id,
                providerKind,
                currentUser.Issuer,
                currentUser.Subject,
                now);
            identity.RecordAuthentication(now);
            user.LinkExternalIdentity(identity);

            user.RecordLogin(now);
            await users.AddAsync(user, cancellationToken);
            return user;
        }

        if (!user.IsActive)
        {
            return null;
        }

        var identity = user.ExternalIdentities.Single(item =>
            string.Equals(item.Issuer, currentUser.Issuer, StringComparison.Ordinal) &&
            string.Equals(item.Subject, currentUser.Subject, StringComparison.Ordinal));

        if (!identity.IsActive)
        {
            return null;
        }

        identity.RecordAuthentication(now);
        user.RecordLogin(now);

        if (user.FirstName != currentUser.FirstName ||
            user.LastName != currentUser.LastName ||
            user.Email != currentUser.Email)
        {
            user.UpdateProfile(currentUser.FirstName, currentUser.LastName, currentUser.Email);
        }

        await users.UpdateAsync(user, cancellationToken);
        return user;
    }
}
