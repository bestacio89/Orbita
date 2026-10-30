# Orbita

Orbita is an independent project and work-orchestration application developed by Enthelekeia Systems.

The backend manages projects, assignments, planning, workload, TSP calculations, governance, commercial tracking, imports, drafts, and project activity. It is being ported from AstrolabaMagellan into Orbita's own architecture and contracts; the frontend is intentionally out of scope for this backend phase.

## Technology

- .NET 10 and ASP.NET Core
- PostgreSQL with Entity Framework Core 10 and Npgsql
- Franz.Common 2.3.3 for shared application, mediator, persistence, HTTP, messaging, and observability infrastructure
- Microsoft Entra ID integration at the API boundary; production identity configuration and end-to-end security verification remain part of the API/security phase
- A separate Kafka consumer host for asynchronous workflows where required
- Aegis architectural tribunals for architecture compliance reporting

## Solution structure

| Project | Responsibility |
| --- | --- |
| `Orbita.Domain` | Business entities, value objects, policies, and calculation engines |
| `Orbita.Contracts` | Commands, queries, DTOs, authorization identifiers, and persistence abstractions |
| `Orbita.Application` | Use-case handlers and application orchestration |
| `Orbita.Persistence` | EF Core context, configurations, PostgreSQL migrations, and repositories |
| `Orbita.API` | HTTP API, authentication integration, authorization, and host composition |
| `Orbita.ClientHttp` | Typed outbound HTTP integration boundary |
| `Orbita.Consumer` | Background messaging host |
| `Tests/Orbita.Testing.Unit` | Domain and application unit tests |
| `Tests/Orbita.Testing.Integration` | PostgreSQL 18 Testcontainers integration tests |
| `Tests/Orbita.Testing` | Architecture and infrastructure contract tests |
| `Tests/Orbita.Testing.ArchitecturalReports` | Consolidated architectural tribunal reports |

## Architecture

Orbita consumes Franz.Common as an external dependency. Its own domain, contracts, and application layers remain under the `Orbita.*` namespaces. Franz.Common is infrastructure, not a replacement for Orbita's business model.

The API is the composition root. Authorization is enforced server-side against validated identity and explicit project assignments; client-side capability data is descriptive and never replaces per-operation authorization.

## Local development

The API and consumer configuration is under their respective `appsettings.json` files. Database connection values can be overridden through environment-specific configuration or environment variables. Do not commit credentials, tenant secrets, or production connection strings.

Database migrations run automatically only in Development. Production schema changes must be applied by the deployment pipeline.

## Build and verification

```bash
dotnet restore Orbita.slnx
dotnet build Orbita.slnx --configuration Release --no-restore

dotnet test Tests/Orbita.Testing.Unit/Orbita.Testing.Unit.csproj --configuration Release
dotnet test Tests/Orbita.Testing.Integration/Orbita.Testing.Integration.csproj --configuration Release
dotnet test Tests/Orbita.Testing/Orbita.Testing.csproj --configuration Release
dotnet test Tests/Orbita.Testing.ArchitecturalReports/Orbita.Testing.ArchitecturalReports.csproj --configuration Release
```

Pull-request CI runs all four test projects. PostgreSQL integration tests use PostgreSQL 18 through Testcontainers; SQLite and EF InMemory are not substitutes for persistence verification.

## PostgreSQL migrations

Orbita owns a fresh schema and an independent migration lineage. The current baseline is `20261030204648_InitialOrbitaSchema`, generated from the current `ApplicationDbContext` model. It is the only migration in the assembly and is paired with `ApplicationDbContextModelSnapshot`.

Verify model consistency with:

```bash
dotnet ef migrations has-pending-model-changes \
  --project sources/back/Orbita.Persistence \
  --startup-project sources/back/Orbita.API \
  --context ApplicationDbContext
```

Historical Magellan data conversion is a separate import workflow. It must not be added to Orbita's EF migration lineage.

## Infrastructure

Deployment foundations are maintained separately under `Infrastructure/`, `.github/workflows/`, `.gitlab/ci/`, and `pipelines/`. Provider-specific infrastructure is not duplicated in the business backend.

## Documentation

- [Backend Migration Roadmap](Docs/Backend-Migration-Roadmap.md)
- [PostgreSQL Migration Reconciliation](Docs/PostgreSQL-Migration-Reconciliation.md)
