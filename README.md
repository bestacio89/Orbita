# Orbita

Orbita is an independent project and work-orchestration application developed by Enthelekeia Systems.

It provides the backend foundation for managing projects, project assignments, planning, workload, TSP calculations, governance, commercial tracking, imports and project activity.

## Technology

- .NET 10 and ASP.NET Core
- PostgreSQL with Entity Framework Core
- Microsoft Entra ID for identity and authentication
- Franz.Common 2.3.3 for shared application, mediator, persistence, HTTP, messaging and observability infrastructure
- Kafka-based messaging and outbox infrastructure
- Docker and infrastructure-as-code deployment foundations
- Aegis architectural tribunals for architectural compliance reporting

## Solution structure

| Project | Responsibility |
| --- | --- |
| `Orbita.Domain` | Project domain model, value objects, policies and calculation engines |
| `Orbita.Contracts` | Application commands, queries, DTOs, authorization and persistence abstractions |
| `Orbita.Application` | Use-case handlers, identity resolution and application orchestration |
| `Orbita.Persistence` | EF Core context, repository implementations and database integration |
| `Orbita.API` | HTTP API, authentication integration and host composition |
| `Orbita.Consumer` | Background messaging host and Kafka consumers |
| `Orbita.ClientHttp` | Typed HTTP client integration boundary |
| `Tests/Orbita.Testing` | Behavioural and application test suite |
| `Tests/Orbita.Testing.ArchitecturalReports` | Consolidated architectural tribunal reports |

## Architecture

Orbita consumes Franz.Common as an external dependency. The application retains its own `Orbita.*` assemblies and namespaces; Franz.Common is infrastructure, not a replacement for Orbita's domain or application contracts.

The API enforces authorization independently of client applications. A validated Entra identity is resolved to an Orbita user, while project access is checked against explicit project assignments.

## Local development

The API and consumer configuration is under their respective `appsettings.json` files. Local defaults target PostgreSQL and Redis; Kafka settings are provided for the consumer host. Override connection details through environment-specific configuration or environment variables. Do not commit credentials.

Database migrations are applied automatically only in Development. Production schema changes belong to the deployment pipeline.

## Build and validation

Restore and build the solution:

```bash
dotnet restore Orbita.slnx
dotnet build Orbita.slnx --configuration Release --no-restore
```

Pull-request CI restores and builds the solution, then runs the consolidated architectural tribunals. Behavioural tests remain available in `Tests/Orbita.Testing` for targeted and broader validation.

## Infrastructure

Deployment foundations are maintained separately under `Infrastructure/`, `.github/workflows/`, `.gitlab/ci/` and `pipelines/`. These contain the provider-specific infrastructure and CI/CD definitions; they are not application domain examples.

## Documentation

See [Backend Migration Roadmap](Docs/Backend-Migration-Roadmap.md) for the ongoing backend migration and implementation plan.
