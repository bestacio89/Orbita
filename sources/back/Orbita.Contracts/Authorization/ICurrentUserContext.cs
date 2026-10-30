#nullable enable

namespace Orbita.Contracts.Authorization;

/// <summary>
/// Current authenticated principal after token validation and claims transformation.
/// Tenant/object identifiers are the external identity key; email and display name are presentation data only.
/// </summary>
public interface ICurrentUserContext
{
    bool IsAuthenticated { get; }
    Guid? UserId { get; }
    Guid? TenantId { get; }
    Guid? ObjectId { get; }
    string? FirstName { get; }
    string? LastName { get; }
    string? DisplayName { get; }
    string? Email { get; }
    IReadOnlyCollection<string> GlobalRoles { get; }
}
