using Orbita.Domain.Common;

namespace Orbita.Domain.Entities;

/// <summary>
/// An application user provisioned from a validated Microsoft Entra identity.
/// Tenant ID and object ID form the immutable external identity; email and display names are not authorization keys.
/// </summary>
public sealed class User : DomainEntity
{
    private User()
    {
    }

    private User(Guid tenantId, Guid objectId, string firstName, string lastName, string email, DateTimeOffset createdAt)
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
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? LastLoginAt { get; private set; }

    public string DisplayName => string.Join(' ', new[] { FirstName, LastName }
        .Where(value => !string.IsNullOrWhiteSpace(value)));

    public static User Provision(
        Guid tenantId,
        Guid objectId,
        string firstName,
        string lastName,
        string email,
        DateTimeOffset createdAt) =>
        new(tenantId, objectId, firstName, lastName, email, createdAt);

    public void UpdateProfile(string firstName, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new BusinessRuleException("A user's first name is required.");
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new BusinessRuleException("A user's last name is required.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new BusinessRuleException("A user's email address is required.");
        }

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim();
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
}
