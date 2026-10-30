# Orbita Backend Porting Roadmap

## Current implementation status — 2026-10-30

This roadmap remains the functional target. The status below distinguishes implemented foundations from work that is not yet complete.

| Phase | Status | Evidence / remaining work |
| --- | --- | --- |
| 0 — Architecture baseline | Complete | Source/target mapping, functional inventory, dependency rules, and acceptance criteria are documented. |
| 1 — Domain and contracts | Foundation implemented | Orbita domain entities, value objects, calculation rules, TSP models, commands, queries, DTOs, and repository contracts are present. Project contracts now follow one-type-per-file organization. Continue validating completeness against the functional inventory. |
| 2 — PostgreSQL persistence | Baseline complete; capability validation ongoing | A single clean InitialOrbitaSchema migration is generated from the current model. PostgreSQL 18 integration tests apply it to an empty database, validate the mapped schema, and exercise repository, uniqueness, rollback, and concurrency behavior. Historical data import remains separate. |
| 3 — Application workflows | Partial | Project lifecycle, assignment, current-user, project draft, co-author, TSP ingestion, and deterministic calculation workflows are present. Remaining planning, time-entry, financial, export, governance, and reporting capabilities must be assessed against the source inventory. |
| 4 — API and security | Partial | Project and draft HTTP endpoints and current-user resolution are present. Full Entra token validation and end-to-end authorization coverage for the complete security matrix remain outstanding. |
| 5 — Automated verification | Foundation complete; coverage expansion ongoing | Unit, PostgreSQL integration, architecture, and architectural tribunal projects run in CI. API-level end-to-end security scenarios and remaining business workflows still require coverage. |
| 6 — Operations and documentation | Partial | CI, environment-driven configuration, migration guidance, and this roadmap are present. Production deployment, readiness, secret-store integration, and operational validation remain outstanding. |

The next roadmap phase must be selected from the remaining functional inventory, not inferred from the existence of the current foundation.


## 1. Objective

Port the complete backend business capability of AstrolabaMagellan into Orbita while retaining Orbita's established solution structure, Franz.Common 2.3.3 integration, .NET 10 baseline, and PostgreSQL persistence model.

This is a backend migration, not a repository copy. The existing Orbita sample bookstore implementation is replaceable template content. The port must preserve Magellan's business rules, API behaviour, authorization boundaries, persistence semantics, and regression coverage while translating names and conventions into English.

## 2. Source and target baselines

| Concern | Baseline |
|---|---|
| Source repository | `bestacio89/AstrolabaMagellan` |
| Source branch | `main` (the repository currently exposes no `develop` branch) |
| Source snapshot | `b88ab8177bbab47da46daf78c08cf7db253d296d` |
| Target repository | `bestacio89/Orbita` |
| Target branch | `develop` |
| Target snapshot | `470e27668f2fd99795701d4d0d57540562f1e7bb` |
| Runtime | .NET 10 |
| Persistence | PostgreSQL with EF Core code-first migrations |
| Framework | Franz.Common 2.3.3 |
| Frontend | Out of scope; preserve backend contracts without porting Angular |

All implementation branches and pull requests are based on `Orbita/develop`. Source branch names are not inferred: Magellan currently has only `main`.

## 3. Target architecture and placement

Keep the existing Orbita solution layout. Do not create a parallel `src` tree or import the Magellan project names.

| Orbita project | Responsibility in the migrated backend |
|---|---|
| `sources/back/Orbita.Domain` | Business entities, value objects, enums, domain rules, project calculations, TSP domain models |
| `sources/back/Orbita.Contracts` | Application-facing commands, queries, DTOs, repository/service contracts, stable authorization identifiers and typed client contracts |
| `sources/back/Orbita.Application` | Use cases, orchestration, project access rules, import/draft workflows, calculations, tracking, exports, application validation |
| `sources/back/Orbita.Persistence` | PostgreSQL DbContext, EF configurations, repositories, migrations, persistence adapters, audit integration |
| `sources/back/Orbita.ClientHttp` | Typed outbound HTTP clients only where the backend integration requires them; not a home for inbound API controllers |
| `sources/back/Orbita.API` | REST controllers, Entra JWT authentication, development-only test identity, authorization policies, error mapping, API composition |
| `sources/back/Orbita.Consumer` | Only asynchronous/background processing that is actually required by the migrated backend; do not introduce Kafka merely because the template supports it |
| `tests/Orbita.Testing` | Domain, application, persistence, and API test suites, grouped by test concern |
| `tests/Orbita.Testing.ArchitecturalReports` | Architecture compliance and dependency-direction audits |

### Dependency rules

- Domain must not depend on Application, Persistence, API, EF Core, or ASP.NET Core.
- Contracts may reference Domain types where they form explicit public contracts, but must not reference Persistence.
- Application depends on Domain and Contracts; it must not reference the concrete Persistence project.
- Persistence implements contracts and depends on Domain; it must not become an application service layer.
- API is the composition root and may reference Application, Contracts, and Persistence.
- Franz.Common packages remain external framework dependencies under their published `Franz.Common.*` identities at version `2.3.3`. Never rename these identifiers to `Orbita.Common.*`.
- Use Franz.Common mediator, mapping, business, hosting, HTTP, EF, auditing, and messaging facilities only where their actual 2.3.3 APIs fit the use case. Do not duplicate framework abstractions without a demonstrated need.

## 4. Functional inventory to preserve

### Project and identity management
- Project creation, editing, archive/status rules, uniqueness identity, and project lifecycle.
- User provisioning from validated Entra tenant/object identity, activation/deactivation, and last-login data.
- Global application roles `Manager` and `Admin`, with distinct capability sets.
- Project-level assignments for Project Manager, Contributor, and Reader.
- Profile-to-user ownership links used to scope Contributor work.
- Project journal/audit entries for relevant business mutations.

### Project delivery and tracking
- Project profile, contract dates, forecast dates, sold planning, agency grids, phases, lots, axes, task types, tasks, workload, time consumption, and forecast replanning.
- Task status/progress/remaining-work/date rules and optimistic concurrency/version checks.
- Planning dates, milestones, hidden/highlighted planning elements, frozen reference plans, and variance explanations.
- Workshops, deliverables, recipients, weekly status points, actions, decisions, risks, comments, and notifications.
- Project parameters, alerts, AI-related reference lists, and reporting/grouping options.

### Financial and commercial capability
- Sold revenue, discounts, management/provision percentages, budgets, purchases, expenses, actual costs, invoicing milestones, billing links, and financial indicators.
- Financial snapshots for committee reporting.
- Financial visibility filtering at the server boundary, including DTOs and exports.
- Manager financial export capability must not be implicitly granted to Admin.

### TSP import and drafting
- Read and validate TSP Excel workbooks.
- Convert imported workbooks into the domain model without losing recognized and custom fields.
- Create blank, imported, or project-derived drafts.
- Draft author/co-author ownership and optimistic concurrency.
- Draft analysis, validation feedback, Excel export, and controlled project launch.
- Preserve workbook fixtures and expected-result JSON scenarios as regression assets.

### API and authorization
- Entra ID JWT validation against configured tenant and API audience.
- Development identity handler restricted to Development and Tests.
- Derive capabilities from validated `roles` claims; never trust incoming capability claims.
- Use stable tenant ID + object ID identity; do not authorize by email or display name.
- Require authentication by default and make only explicitly public endpoints anonymous.
- Enforce both global capability and project assignment for project operations.
- Return 404 for inaccessible project resources where existence must not be disclosed.
- Contributor follow-up is limited to tasks charged to the linked profile.
- Contributor time entry requires a positively charged task belonging to the linked profile.
- Comment update/delete is restricted to the original author, including against same-project managers/admins.
- API current-user capability contract is versioned and descriptive only; every operation rechecks authorization server-side.

## 5. Porting phases

Each phase is delivered as one or more coherent commits and a pull request targeting `develop`. Keep each commit to no more than 20 changed files. Split commits by change nature (domain model, contracts, persistence mappings, application services, API/authentication, tests, documentation), not arbitrary file-count slices. A phase PR may contain multiple commits.

### Phase 0 — Migration plan and architecture baseline
**Purpose:** Make the migration scope, target placement, dependency rules, and acceptance criteria explicit before moving implementation code.

Deliverables:
- This roadmap, source/target baseline, functional inventory, phase sequence, and validation strategy.
- A source-to-target project mapping.
- A documented decision to replace Orbita's sample Book/Member feature rather than retain unrelated demo behaviour.

Acceptance:
- Every source backend capability has a target project and a planned phase.
- No frontend migration is mixed into backend PRs.
- Every phase PR uses the agreed PR template.

### Phase 1 — Domain model and public contracts
**Purpose:** Establish the English-language domain and stable application contracts without infrastructure coupling.

Deliverables:
- Replace sample Book/Member domain types with the Magellan project, access, planning, finance, governance, workload, draft, and TSP domain concepts.
- Translate French type/member/file names into consistent English names; preserve business meaning and serialized API contract decisions explicitly.
- Port domain invariants, value objects, enums, calculation engine, calendar/time-zone rules, and TSP semantic models.
- Define application contracts, DTOs, commands, queries, repository abstractions, current-user identity, global role names, and capability identifiers.
- Use Franz.Common base entities/value objects and mediator contracts where supported by the actual package API.

Acceptance:
- Domain and contracts compile independently of API and Persistence.
- Domain unit tests cover invariants and calculations.
- No French identifiers remain in newly ported public code, except literal business data that must remain French for domain compatibility.

### Phase 2 — PostgreSQL persistence
**Purpose:** Port the complete data model and make PostgreSQL migrations the authoritative schema evolution mechanism.

Deliverables:
- Replace the sample `ApplicationDbContext` model with the full project model.
- Port all EF entity configurations, indexes, unique constraints, relationships, delete behaviours, concurrency tokens, value conversions, and PostgreSQL-specific mappings.
- Establish a fresh Orbita migration lineage containing exactly one EF Core-generated initial migration and its matching model snapshot. Historical Magellan migration IDs, schema names, and upgrade operations do not belong in Orbita's migration assembly; historical data conversion is a separate import workflow.
- Implement persistence adapters/repositories behind Contracts.
- Configure design-time DbContext creation and environment-driven connection settings.
- Preserve tenant/object identity uniqueness and project/profile ownership constraints.

Acceptance:
- A clean PostgreSQL database can be created entirely from committed migrations.
- Existing PostgreSQL data-model semantics are represented, including unique constraints and concurrency behaviour.
- No SQLite substitute is used to claim PostgreSQL integration correctness.
- Application has no reference to the Persistence project.

### Phase 3 — Application workflows and business services
**Purpose:** Port the complete application behaviour over the new Domain/Contracts/Persistence boundaries.

Deliverables:
- Project lifecycle, identity provisioning, project access, role assignment, and account administration.
- Project calculations, portfolio/project views, financial filtering, workload, tracking, planning, task entry, registers, and governance services.
- TSP import, workbook parsing/writing, draft authoring/co-authoring, draft analysis, and project launch.
- Exports and committee snapshots.
- Audit journal and deterministic clock abstractions.
- Application dependency registration and Franz.Common mediator/mapping integration.

Acceptance:
- Business rules match the source behaviour, with English identifiers and no accidental loss of calculations or workflows.
- Application unit tests cover rules, service boundaries, access denial, ownership, and concurrency.
- Application has no direct dependency on concrete EF DbContext or Persistence implementation.

### Phase 4 — API composition, Entra authentication, and authorization
**Purpose:** Expose the migrated workflows through Orbita's API host and enforce security at every resource boundary.

Deliverables:
- English controller/route/action names and English request/response contracts.
- Entra JWT validation and fail-fast production configuration.
- Development-only authentication for local and integration tests.
- Claims transformation deriving capabilities from exact `Manager` and `Admin` role values.
- Default authenticated policy, capability policies, and scope validation.
- Current-user and capability endpoints.
- Project-scoped access checks, 404 concealment, financial visibility, Contributor profile/task isolation, and author-only comment mutations.
- Problem-details exception handling and safe API error mapping.
- Development migration policy and production migration readiness checks consistent with the deployment model.

Acceptance:
- Anonymous, invalid token, wrong tenant/audience/scope, unknown role, missing assignment, wrong project role, cross-profile task, and non-author comment operations are rejected.
- Manager and Admin capabilities remain distinct.
- The frontend is not required for API authorization tests.

### Phase 5 — Automated verification and architecture enforcement
**Purpose:** Make the migration safe to evolve and prevent regressions in business and security behaviour.

Deliverables:
- Domain unit tests for calculations, dates, planning, TSP interpretation, and entity invariants.
- Application tests for workflows, project access, financial visibility, Contributor restrictions, author-only comments, and concurrency.
- PostgreSQL Testcontainers integration tests for migrations, relational constraints, repository operations, and authorization.
- API integration tests for authentication configuration, claims transformation, status codes, current-user contract, and end-to-end authorization.
- Architecture tests for Orbita's dependency direction and layer boundaries.
- Reusable test fixtures/builders with deterministic identity and clock behaviour.

Acceptance:
- Full solution restore/build/test passes.
- PostgreSQL tests run against PostgreSQL, not SQLite or EF InMemory.
- Security matrix covers positive and negative cases.
- Architecture tests enforce Domain <- Contracts/Application <- Persistence/API direction as defined above (with API as composition root).

### Phase 6 — Operational configuration and documentation
**Purpose:** Make the backend deployable without importing the source repository's unrelated frontend or duplicating Orbita's multi-cloud template infrastructure.

Deliverables:
- English API appsettings templates containing no real tenant IDs, secrets, or production connection strings.
- PostgreSQL, Entra, CORS, telemetry, and migration configuration guidance.
- Docker/API runtime configuration and health/readiness semantics.
- CI workflow coverage for restore, build, unit tests, PostgreSQL integration tests, and migration validation.
- English README/setup/security/operations documentation.
- Deployment hand-off notes for existing Orbita Azure/AWS/GCP infrastructure; do not redesign those stacks as part of the business backend port.

Acceptance:
- A fresh environment can be configured using documented variables and secret-store inputs.
- Production startup fails safely when required identity/database configuration is missing or migrations are pending.
- CI validates the backend without requiring Angular.

## 6. Commit and pull request rules

### Commit convention

Use the repository's established format:

`[Action][Scope]: lowercase summary`

Allowed actions: `Feat`, `Fix`, `Patch`, `Test`, `Refactor`, `Docs`, `Chore`.

Use the narrowest meaningful scope, for example `Domain`, `Contracts`, `Application`, `Persistence`, `Api`, `Tests`, `Security`, or `Docs`.

- Header maximum: 50 characters.
- Body bullets describe technical what and architectural why.
- Maximum 20 changed files per commit.
- A commit should contain one coherent nature of change.
- Never mix unrelated formatting, renaming, infrastructure, and business changes in one commit.

### Pull request template

Every phase PR must use this exact structure:

```markdown
## 📌 Summary
Short summary of the changes.

## 🔍 Details
- [ ] Title follows commit convention (`[Patch]`, `[Feat]`, `[Fix]`, `[Doc]`, etc.)
- [ ] Explains what and why, not just how

## ✅ Changes
- ...

## 🧪 Tests
- [ ] Unit tests updated/added
- [ ] Integration tests verified
- [ ] CI passes locally (if applicable)

## 📚 Docs
- [ ] README/CHANGELOG updated if public API/UX changed
- [ ] No breaking changes undocumented

## 🚀 Release Notes
One-line changelog entry
```

## 7. Cross-phase quality gates

- Preserve the source business semantics; do not perform opportunistic redesign during translation.
- Translate identifiers and technical documentation into English. Preserve user-facing French business vocabulary only where it is actual business data or an external compatibility requirement.
- Do not carry over source namespace/project names such as `PilotageForfaits.*`; use `Orbita.*`.
- Keep `Franz.Common.*` package IDs and namespaces unchanged at version `2.3.3`.
- No secrets, real tenant identifiers, or production customer data in Git.
- Do not introduce Kafka, Redis, Azure-specific services, or other infrastructure merely because the Orbita template supports them. Add only what the migrated backend actually uses.
- Do not claim a test/build/CI result unless it has actually run and the result is available.
- Each phase must be reviewable and buildable before the next phase is stacked on it.

## 8. Initial implementation order

1. Phase 0 architecture baseline.
2. Phase 1 domain and contracts, including removal of unrelated template examples.
3. Phase 2 clean PostgreSQL baseline and persistence verification.
4. Complete the remaining Phase 3 business capabilities in coherent capability-based PRs.
5. Complete Phase 4 Entra authentication and API-level authorization verification.
6. Expand Phase 5 end-to-end and regression coverage as each capability is delivered.
7. Complete Phase 6 deployment, secret management, readiness, and operational documentation.

The migration is complete only when the Orbita backend exposes the required Magellan capabilities with equivalent business and security behaviour, passes the PostgreSQL-backed test suite, and conforms to Orbita's architecture.
