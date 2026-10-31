using Orbita.Domain.Common;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;

namespace Orbita.Testing.Domain;

public sealed class CompanyIdentityDomainTests
{
    private static readonly DateTimeOffset Now =
        new(2026, 10, 31, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Company_creation_normalizes_key_and_name()
    {
        var company = Company.Create("  Acme Corporation  ", " acme ", Now);

        Assert.Equal("Acme Corporation", company.Name);
        Assert.Equal("ACME", company.Key);
        Assert.True(company.IsActive);
    }

    [Theory]
    [InlineData("", "ACME")]
    [InlineData("Acme", " ")]
    public void Company_creation_requires_name_and_key(string name, string key)
    {
        Assert.Throws<BusinessRuleException>(() => Company.Create(name, key, Now));
    }

    [Fact]
    public void Membership_lifecycle_is_company_scoped_and_terminal_after_revocation()
    {
        var membership = CompanyMembership.Create(
            Guid.NewGuid(), Guid.NewGuid(), CompanyRole.Member, Now, Guid.NewGuid());

        membership.Suspend();
        Assert.Equal(CompanyMembershipStatus.Suspended, membership.Status);

        membership.Reactivate();
        Assert.Equal(CompanyMembershipStatus.Active, membership.Status);

        membership.Revoke(Now.AddDays(1));
        Assert.Equal(CompanyMembershipStatus.Revoked, membership.Status);
        Assert.Equal(Now.AddDays(1), membership.RevokedAt);

        Assert.Throws<BusinessRuleException>(() => membership.Reactivate());
        Assert.Throws<BusinessRuleException>(() => membership.ChangeRole(CompanyRole.Administrator));
    }

    [Fact]
    public void Membership_rejects_invalid_identifiers_and_roles()
    {
        Assert.Throws<ArgumentException>(() =>
            CompanyMembership.Create(Guid.Empty, Guid.NewGuid(), CompanyRole.Member, Now, null));
        Assert.Throws<ArgumentException>(() =>
            CompanyMembership.Create(Guid.NewGuid(), Guid.Empty, CompanyRole.Member, Now, null));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CompanyMembership.Create(Guid.NewGuid(), Guid.NewGuid(), (CompanyRole)999, Now, null));
    }

    [Fact]
    public void External_identity_uses_immutable_issuer_and_case_sensitive_subject()
    {
        var userId = Guid.NewGuid();
        var identity = ExternalIdentity.Link(
            userId,
            IdentityProviderKind.OpenIdConnect,
            " https://identity.example.test/tenant ",
            "  User-Subject  ",
            Now);

        Assert.Equal(userId, identity.UserId);
        Assert.Equal("https://identity.example.test/tenant", identity.Issuer);
        Assert.Equal("User-Subject", identity.Subject);
        Assert.True(identity.IsActive);
    }

    [Theory]
    [InlineData("", "subject")]
    [InlineData("not-a-uri", "subject")]
    [InlineData("https://identity.example.test", " ")]
    public void External_identity_requires_valid_issuer_and_subject(string issuer, string subject)
    {
        Assert.Throws<BusinessRuleException>(() =>
            ExternalIdentity.Link(Guid.NewGuid(), IdentityProviderKind.OpenIdConnect, issuer, subject, Now));
    }

    [Fact]
    public void External_identity_authentication_is_monotonic_and_requires_active_link()
    {
        var identity = ExternalIdentity.Link(
            Guid.NewGuid(),
            IdentityProviderKind.Saml2,
            "https://idp.example.test/metadata",
            "persistent-subject",
            Now);

        identity.RecordAuthentication(Now.AddMinutes(5));
        Assert.Equal(Now.AddMinutes(5), identity.LastAuthenticatedAt);

        Assert.Throws<BusinessRuleException>(() => identity.RecordAuthentication(Now));
        identity.Deactivate();
        Assert.Throws<BusinessRuleException>(() => identity.RecordAuthentication(Now.AddMinutes(6)));
    }

    [Fact]
    public void External_identity_can_be_reactivated_without_changing_its_key()
    {
        var identity = ExternalIdentity.Link(
            Guid.NewGuid(),
            IdentityProviderKind.Local,
            "https://orbita.example.test/local",
            "account-123",
            Now);

        identity.Deactivate();
        identity.Reactivate();

        Assert.True(identity.IsActive);
        Assert.Equal("https://orbita.example.test/local", identity.Issuer);
        Assert.Equal("account-123", identity.Subject);
    }
}
