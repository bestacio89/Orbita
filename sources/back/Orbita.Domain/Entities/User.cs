using Orbita.Domain.Common;

namespace Orbita.Domain.Entities;

/// <summary>
/// Orbita user profile. External authentication identities are linked separately from the user record;
/// email and display names are mutable presentation attributes, never identity keys.
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

    /// <summary>
    /// Compatibility factory for the existing Entra provisioning flow.
    /// New identity integrations must create a provider-neutral user and link an ExternalIdentity.
    /// </summary>
    public static User Provision(
        Guid tenantId,
        Guid objectId,
        string? firstName,
        string? lastName,
        string? email,
        DateTimeOffset createdAt)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("The Entra tenant identifier is required.", nameof(tenantId));
        if (objectId == Guid.Empty)
            throw new ArgumentException("The Entra object identifier is required.", nameof(objectId));

        var user = new User(firstName, lastName, email, createdAt);
        user.LinkExternalIdentity(
            Domain.Enums.IdentityProviderKind.OpenIdConnect,
            $"https://login.microsoftonline.com/{tenantId:D}/v2.0",
            objectId.ToString("D"),
            createdAt);

        return user;
    }

    public ExternalIdentity LinkExternalIdentity(
        Domain.Enums.IdentityProviderKind providerKind,
        string issuer,
        string subject,
        DateTimeOffset linkedAt)
    {
        if (_externalIdentities.Any(identity =>
                string.Equals(identity.Issuer, issuer.Trim(), StringComparison.Ordinal) &&
                string.Equals(identity.Subject, subject.Trim(), StringComparison.Ordinal)))
        {
            throw new BusinessRuleException("This external identity is already linked to the user.");
        }

        var identity = ExternalIdentity.Link(Id, providerKind, issuer, subject, linkedAt);
        _externalIdentities.Add(identity);
        return identity;
    }

    public bool UnlinkExternalIdentity(Guid externalIdentityId)
    {
        var identity = _externalIdentities.SingleOrDefault(candidate => candidate.Id == externalIdentityId);
        if (identity is null)
            return false;

        if (_externalIdentities.Count(candidate => candidate.IsActive) <= 1 && identity.IsActive)
            throw new BusinessRuleException("A user cannot unlink their last active external identity.");

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
            throw new BusinessRuleException("A login timestamp cannot move backwards.");

        LastLoginAt = occurredAt;
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
