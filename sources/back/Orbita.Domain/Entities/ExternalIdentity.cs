using Orbita.Domain.Common;
using Orbita.Domain.Enums;

namespace Orbita.Domain.Entities;

/// <summary>
/// Provider-neutral link between an Orbita user and an authenticated external subject.
/// The issuer/subject pair is immutable; profile attributes are never identity keys.
/// </summary>
public sealed class ExternalIdentity : DomainEntity
{
    private ExternalIdentity()
    {
    }

    private ExternalIdentity(
        Guid userId,
        IdentityProviderKind providerKind,
        string issuer,
        string subject,
        DateTimeOffset linkedAt)
    {
        if (userId == Guid.Empty) throw new ArgumentException("A user identifier is required.", nameof(userId));
        if (!Enum.IsDefined(providerKind)) throw new ArgumentOutOfRangeException(nameof(providerKind));
        if (string.IsNullOrWhiteSpace(issuer)) throw new BusinessRuleException("An identity issuer is required.");
        if (string.IsNullOrWhiteSpace(subject)) throw new BusinessRuleException("An identity subject is required.");

        var normalizedIssuer = issuer.Trim();
        if (!Uri.TryCreate(normalizedIssuer, UriKind.Absolute, out _))
            throw new BusinessRuleException("An identity issuer must be an absolute URI.");

        UserId = userId;
        ProviderKind = providerKind;
        Issuer = normalizedIssuer;
        Subject = subject.Trim();
        LinkedAt = linkedAt;
        IsActive = true;
    }

    public Guid UserId { get; private set; }
    public IdentityProviderKind ProviderKind { get; private set; }
    public string Issuer { get; private set; } = string.Empty;
    public string Subject { get; private set; } = string.Empty;
    public DateTimeOffset LinkedAt { get; private set; }
    public DateTimeOffset? LastAuthenticatedAt { get; private set; }
    public bool IsActive { get; private set; }

    public static ExternalIdentity Link(
        Guid userId,
        IdentityProviderKind providerKind,
        string issuer,
        string subject,
        DateTimeOffset linkedAt) =>
        new(userId, providerKind, issuer, subject, linkedAt);

    public void RecordAuthentication(DateTimeOffset authenticatedAt)
    {
        if (!IsActive) throw new BusinessRuleException("An inactive external identity cannot authenticate.");
        if (LastAuthenticatedAt is not null && authenticatedAt < LastAuthenticatedAt)
            throw new BusinessRuleException("An authentication timestamp cannot move backwards.");

        LastAuthenticatedAt = authenticatedAt;
    }

    public void Deactivate() => IsActive = false;

    public void Reactivate() => IsActive = true;
}
