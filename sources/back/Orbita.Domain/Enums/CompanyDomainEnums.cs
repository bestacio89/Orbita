namespace Orbita.Domain.Enums;

/// <summary>Company-scoped role namespace. These roles are independent of platform/global roles.</summary>
public enum CompanyRole
{
    Owner = 1,
    Administrator = 2,
    Member = 3,
    Guest = 4
}

/// <summary>Lifecycle state of a user's membership in one company.</summary>
public enum CompanyMembershipStatus
{
    Active = 1,
    Suspended = 2,
    Revoked = 3
}

/// <summary>Protocol family used to authenticate an external identity.</summary>
public enum IdentityProviderKind
{
    OpenIdConnect = 1,
    Saml2 = 2,
    Local = 3
}
