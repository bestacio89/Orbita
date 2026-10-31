using Orbita.Domain.Common;
using Orbita.Domain.Enums;

namespace Orbita.Domain.Entities;

/// <summary>
/// Provider-neutral Orbita person record. External identities are authentication keys;
/// profile attributes are mutable presentation data and never authorize access.
/// </summary>
public sealed class User : DomainEntity
{
    private readonly List<ExternalIdentity> _externalIdentities = [];

    private User()
    {
    }

    private User(string? firstName, string? lastName, string? email, DateTimeOffset createdAt)
    {
        UpdateProfile(firstName, lastName, email);
        CreatedAt = createdAt;
        IsActive = true;
    }

    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? Email { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? LastLoginAt { get; private set; }
    public IReadOnlyCollection<ExternalIdentity> ExternalIdentities => _externalIdentities.AsReadOnly();

    public string DisplayName
    {
        get
        {
            var name = string.Join(' ', new[] { FirstName, LastName }
                .Where(value => !string.IsNullOrWhiteSpace(value)));
            return string.IsNullOrWhiteSpace(name) ? Email ?? Id.ToString("D") : name;
        }
    }

    public static User Create(
        string? firstName,
        string? lastName,
        string? email,
        DateTimeOffset createdAt) =>
        new(firstName, lastName, email, createdAt);

    public void LinkExternalIdentity(ExternalIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(identity);
        if (_externalIdentities.Any(existing =>
                string.Equals(existing.Issuer, identity.Issuer, StringComparison.Ordinal) &&
                string.Equals(existing.Subject, identity.Subject, StringComparison.Ordinal)))
        {
            throw new BusinessRuleException("This external identity is already linked to the user.");
        }

        _externalIdentities.Add(identity);
    }

    public bool UnlinkExternalIdentity(Guid identityId)
    {
        var identity = _externalIdentities.SingleOrDefault(item => item.Id == identityId);
        if (identity is null)
        {
            return false;
        }

        if (identity.IsActive && _externalIdentities.Count(item => item.IsActive) <= 1)
        {
            throw new BusinessRuleException("A user must retain at least one active external identity.");
        }

        identity.Deactivate();
        return true;
    }

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
