#nullable enable

using Orbita.Domain.Enums;

namespace Orbita.Contracts.Authorization;

/// <summary>Validated external principal metadata. Profile claims are never authentication keys.</summary>
public interface ICurrentUserContext
{
    bool IsAuthenticated { get; }
    string? Issuer { get; }
    string? Subject { get; }
    IdentityProviderKind? ProviderKind { get; }
    Guid? UserId { get; }
    string? FirstName { get; }
    string? LastName { get; }
    string? DisplayName { get; }
    string? Email { get; }
    IReadOnlyCollection<string> GlobalRoles { get; }
}
