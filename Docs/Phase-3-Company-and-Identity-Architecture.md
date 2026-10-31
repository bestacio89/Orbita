# Phase 3 — Company and Identity Architecture

## Status

Proposed backend architecture baseline for Phase 3. This document records the multi-company requirements agreed for Orbita. It does not claim that the model, workflows, persistence mappings, or authentication integrations are implemented.

## Objective

Orbita must support multiple independent companies on one application installation without assuming that every company uses Microsoft Entra ID or shares the same identity infrastructure.

The backend owns company membership, project ownership, business permissions, and the mapping between authenticated external identities and Orbita users. An identity provider authenticates a person; it does not define Orbita's company or project permissions.

## Core concepts and boundaries

### Company

A Company is an organisation using Orbita. It is the primary business isolation boundary for company-owned data.

### User

A User is an Orbita person record. It is not itself an Entra account and must not be identified by mutable attributes such as email address or display name.

A user may have more than one external identity and may be a member of more than one company.

### External identity

An ExternalIdentity links an Orbita user to an identity authenticated by a configured provider. Its stable key is the validated issuer and subject pair (issuer + subject), scoped according to the provider's protocol contract. Email, username, and display name are profile attributes only.

Provider-specific identifiers must not leak into domain authorisation rules. Entra tenant/object identifiers may be retained as provider-specific identity metadata where needed for compatibility, but must not remain the universal identity model.

### Company membership

CompanyMembership represents a user's relationship with a company, including membership state and company-scoped role. Membership is independent of project assignment.

A person may be authenticated and have an active Orbita user record without having membership in a particular company. Membership in one company grants no access to another company.

### Project ownership and assignment

Every project belongs to exactly one owning company. Project assignments remain a separate project-scoped relationship and do not follow automatically from company membership.

A company member does not automatically receive access to every project. A project manager is restricted to projects explicitly assigned to them unless a separately specified company-level capability explicitly grants broader portfolio access.

Cross-company collaboration must be an explicit, auditable grant. It must not be inferred from a shared user identity or email address.

### Company identity configuration

Identity-provider configuration is company-scoped. A company may configure one or more supported providers and an explicit account policy.

The domain stores validated configuration metadata and references to secrets; it never stores client secrets, private keys, passwords, or other credentials. Secret material belongs in the deployment's secret manager.

Authentication protocols and token validation remain API/infrastructure responsibilities. The domain and application layers consume a validated, provider-neutral actor identity.

## Authentication and provisioning requirements

The backend architecture must be able to accommodate:

- OpenID Connect providers, including Entra ID, Okta, Auth0, Keycloak, and Google identity services.
- SAML 2.0 enterprise federation.
- Local Orbita accounts where explicitly enabled by platform/company policy.
- SCIM or equivalent provisioning lifecycle integration as a separate provisioning concern, not as an authentication protocol.
- Company-specific provider selection and account-linking policy.
- Provider-neutral account activation, deactivation, membership lifecycle, and audit history.

This is a compatibility target. It does not require a vendor-specific connector for every provider in the first implementation. Prefer standards-based OIDC and SAML integration points over hard-coded vendor branches.

## First-launch and environment discovery boundary

The initial deployment experience will eventually let an administrator select and validate the platform bootstrap/authentication configuration. Environment discovery may identify candidate configuration, but it is advisory and read-only.

Discovery must never silently trust an endpoint, create an identity-provider trust relationship, provision an administrator, or enable an authentication method. The administrator must explicitly confirm and validate configuration.

The backend must expose the configuration, validation, bootstrap-state, and provider capability contracts before frontend work begins. The first-launch user interface itself is deferred until the frontend phase.

Platform bootstrap configuration is distinct from company-level identity configuration. Completing platform bootstrap must not force all companies to use the same identity provider.

## Authorisation invariants

1. Authentication proves an external identity; it does not grant company membership.
2. Company membership does not grant project assignment.
3. Project assignment is scoped to one project and one user.
4. Company roles and global platform roles are separate role namespaces.
5. A user's access to a company or project is evaluated on every operation.
6. Email, display name, and caller-supplied capability claims are never authorisation keys.
7. Cross-company access requires an explicit grant and is auditable.
8. Financial visibility is enforced at query, DTO, and export boundaries; it is not implied by an identity-provider role.
9. Deactivating an external identity must not silently delete the Orbita user or their audit history.
10. Revoking a company membership must prevent further company/project operations within that company's scope.

## Initial domain implementation

The first Phase 3 domain slice introduces `Company`, `CompanyMembership`, and `ExternalIdentity`, together with company-scoped role, membership lifecycle, and provider-kind enums. `User` now supports multiple provider-neutral external identity links, rejects duplicate issuer/subject links, and prevents unlinking its last active identity. Domain invariants are covered by unit tests. The current Entra provisioning factory and scalar columns remain as a compatibility bridge; the external identity collection is deliberately excluded from EF mapping until the dedicated schema migration and repository slice. This is not yet a complete multi-company implementation: provider-neutral actor resolution, project ownership, persistence mappings, repository isolation, workflows, and migrations remain subsequent deliverables.

`ExternalIdentity` uses an immutable issuer/subject pair and supports OIDC, SAML 2.0, and local identity kinds. Provider-specific authentication remains outside the domain. The existing Entra-specific `User` and API identity contract are retained temporarily until the provider-neutral application and API transition is implemented as a coherent slice.

## Persistence and migration implications

- Add company, external identity, company membership, company identity configuration, and any explicit cross-company grant model required by approved workflows.
- Add CompanyId to company-owned aggregates and enforce ownership in repository queries as well as relational constraints.
- Scope project commercial identity uniqueness to its owning company unless the business rules explicitly require a platform-wide uniqueness constraint.
- Add indexes and unique constraints for provider identity keys, company membership, and project ownership.
- Keep the initial clean migration history authoritative; schema changes are added as normal EF Core migrations.
- Existing single-company seed/bootstrap data must be assigned to an explicit default company through a controlled bootstrap or migration path.

## Delivery sequence

1. Agree the company, user, external identity, membership, and project ownership domain model.
2. Add domain invariants and unit tests.
3. Define one-type-per-file contracts and application ports.
4. Implement PostgreSQL mappings, constraints, and repository queries.
5. Implement company and membership provisioning/workflows.
6. Implement company-scoped project creation, listing, access checks, and financial visibility.
7. Define provider-neutral identity configuration and validation contracts.
8. Implement API authentication adapters and provider integrations in the API/security phase.
9. Add API end-to-end security tests for cross-company, cross-project, inactive identity, and provider-identity scenarios.

## Explicitly unresolved business decisions

The following must be settled from the business requirements before implementation hardens them into public contracts:

- Whether one person may be a member of multiple companies in the same Orbita installation (the current design permits it).
- Which company roles exist and their exact capabilities.
- Whether company administrators may see every project in their company or still require explicit project assignments.
- Whether a project can have more than one owning company, or only one owner with explicit external collaborators (the current design assumes one owner).
- Whether cross-company collaboration grants read-only access, selected project roles, or both.
- Whether a platform operator can access company data, and under what audited break-glass policy.
- Whether local accounts are allowed by default, opt-in, or prohibited by platform policy.

These decisions are not inferred from the single-company Magellan source and must not be silently guessed. The initial implementation proceeds with one owning company per project and explicit collaboration grants as the baseline.
