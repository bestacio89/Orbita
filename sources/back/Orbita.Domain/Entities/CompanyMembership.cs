using Orbita.Domain.Common;
using Orbita.Domain.Enums;

namespace Orbita.Domain.Entities;

/// <summary>
/// Company-scoped membership. It does not imply assignment to any project owned by the company.
/// </summary>
public sealed class CompanyMembership : DomainEntity
{
    private CompanyMembership()
    {
    }

    private CompanyMembership(
        Guid companyId,
        Guid userId,
        CompanyRole role,
        DateTimeOffset joinedAt,
        Guid? grantedByUserId)
    {
        if (companyId == Guid.Empty) throw new ArgumentException("A company identifier is required.", nameof(companyId));
        if (userId == Guid.Empty) throw new ArgumentException("A user identifier is required.", nameof(userId));
        if (!Enum.IsDefined(role)) throw new ArgumentOutOfRangeException(nameof(role));

        CompanyId = companyId;
        UserId = userId;
        Role = role;
        Status = CompanyMembershipStatus.Active;
        JoinedAt = joinedAt;
        GrantedByUserId = grantedByUserId;
    }

    public Guid CompanyId { get; private set; }
    public Guid UserId { get; private set; }
    public CompanyRole Role { get; private set; }
    public CompanyMembershipStatus Status { get; private set; }
    public DateTimeOffset JoinedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public Guid? GrantedByUserId { get; private set; }

    public static CompanyMembership Create(
        Guid companyId,
        Guid userId,
        CompanyRole role,
        DateTimeOffset joinedAt,
        Guid? grantedByUserId) =>
        new(companyId, userId, role, joinedAt, grantedByUserId);

    public void ChangeRole(CompanyRole role)
    {
        if (!Enum.IsDefined(role)) throw new ArgumentOutOfRangeException(nameof(role));
        if (Status == CompanyMembershipStatus.Revoked)
            throw new BusinessRuleException("A revoked company membership cannot change role.");

        Role = role;
    }

    public void Suspend()
    {
        if (Status == CompanyMembershipStatus.Revoked)
            throw new BusinessRuleException("A revoked company membership cannot be suspended.");

        Status = CompanyMembershipStatus.Suspended;
    }

    public void Reactivate()
    {
        if (Status == CompanyMembershipStatus.Revoked)
            throw new BusinessRuleException("A revoked company membership cannot be reactivated.");

        Status = CompanyMembershipStatus.Active;
    }

    public void Revoke(DateTimeOffset revokedAt)
    {
        if (revokedAt < JoinedAt)
            throw new BusinessRuleException("Membership revocation cannot precede its creation.");

        if (Status == CompanyMembershipStatus.Revoked) return;
        Status = CompanyMembershipStatus.Revoked;
        RevokedAt = revokedAt;
    }
}
