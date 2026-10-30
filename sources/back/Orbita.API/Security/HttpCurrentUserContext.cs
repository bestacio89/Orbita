#nullable enable

using System.Security.Claims;
using Orbita.Contracts.Authorization;

namespace Orbita.API.Security;

/// <summary>Projects the already-validated HTTP principal into the application identity contract.</summary>
public sealed class HttpCurrentUserContext(IHttpContextAccessor accessor) : ICurrentUserContext
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

    public Guid? UserId => ReadGuid("orbita_user_id");

    public Guid? TenantId => ReadGuid("tid");

    public Guid? ObjectId => ReadGuid("oid") ?? ReadGuid(ClaimTypes.NameIdentifier);

    public string? FirstName =>
        Principal?.FindFirst("given_name")?.Value ??
        Principal?.FindFirst(ClaimTypes.GivenName)?.Value;

    public string? LastName =>
        Principal?.FindFirst("family_name")?.Value ??
        Principal?.FindFirst(ClaimTypes.Surname)?.Value;

    public string? DisplayName =>
        Principal?.FindFirst("name")?.Value ??
        Principal?.FindFirst(ClaimTypes.Name)?.Value;

    public string? Email =>
        Principal?.FindFirst("preferred_username")?.Value ??
        Principal?.FindFirst(ClaimTypes.Email)?.Value ??
        Principal?.FindFirst("email")?.Value;

    public IReadOnlyCollection<string> GlobalRoles =>
        Principal?.FindAll("roles")
            .Concat(Principal.FindAll(ClaimTypes.Role))
            .Select(claim => claim.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray()
        ?? Array.Empty<string>();

    private Guid? ReadGuid(string claimType) =>
        Guid.TryParse(Principal?.FindFirst(claimType)?.Value, out var value) && value != Guid.Empty
            ? value
            : null;
}
