using Orbita.Domain.Common;

namespace Orbita.Domain.Entities;

/// <summary>
/// Application user provisioned from a validated Microsoft Entra identity.
/// Tenant ID and object ID form the immutable external identity; email and display names are presentation data only.
/// </summary>
public sealed class User : DomainEntity
{
    private User()
    {
    }

    private User(Guid tenantId, Guid objectId, string? firstName, string? lastName, string? email, DateTimeOffset createdAt)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("The Entra tenant identifier is required.", nameof(tenantId));
        }

        if (objectId == Guid.Empty)
        {
            throw new ArgumentException("The Entra object identifier is required.", nameof(objectId));
        }

        TenantId = tenantId;
        ObjectId = objectId;
        UpdateProfile(firstName, lastName, email);
        CreatedAt = createdAt;
        IsActive = true;
    }

    public Guid TenantId { get; private set; }
    public Guid ObjectId { get; private set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? Email { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? LastLoginAt { get; private set; }

    public string DisplayName
    {
        get
        {
            var name = string.Join(' ', new[] { FirstName, LastName }
                .Where(value => !string.IsNullOrWhiteSpace(value)));
            return string.IsNullOrWhiteSpace(name) ? Email ?? ObjectId.ToString("D") : name;
        }
    }

    public static User Provision(
        Guid tenantId,
        Guid objectId,
        string? firstName,
        string? lastName,
        string? email,
        DateTimeOffset createdAt) =>
        new(tenantId, objectId, firstName, lastName, email, createdAt);

    public void UpdateProfile(string? firstName, string? lastName, string? email)
    {
        FirstName = NormalizeOptional(firstName);
        LastName = NormalizeOptional(lastName);
        Email = NormalizeOptional(email);
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    public void RecordLogin(DateTimeOffset occurredAt)
    {
        if (LastLoginAt is not null && occurredAt < LastLoginAt)
        {
            throw new BusinessRuleException("A login timestamp cannot move backwards.");
        }

        LastLoginAt = occurredAt;
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
