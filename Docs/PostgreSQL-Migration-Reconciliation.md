# PostgreSQL Persistence Baseline

## Decision

Orbita owns a new PostgreSQL schema. Its EF Core migration history is independent of AstrolabaMagellan.

The current ApplicationDbContext model and its explicit entity configurations are the sole source of truth for the Orbita schema. Historical Magellan migration IDs, designer metadata, model snapshots, French table names, and upgrade compatibility are not part of Orbita's deployment contract.

If data must be imported from AstrolabaMagellan, that work belongs in a separate, explicit import workflow. It must not shape Orbita's EF migration lineage.

## Required migration state

The persistence project must contain exactly one clean initial migration for the current Orbita model, plus the matching ApplicationDbContextModelSnapshot.

The initial migration must:
- create the current mapped tables, columns, primary keys, foreign keys, indexes, check constraints, and owned-value storage;
- use PostgreSQL/Npgsql-native types and the configured schema;
- contain no DropTable, DropColumn, RenameTable, or RenameColumn operations against historical Magellan structures;
- contain no French legacy table names or Magellan migration IDs;
- produce a database for which Database.HasPendingModelChanges() is false immediately after migration.

Do not hand-author or mechanically trim EF migration operations. Generate the migration from the current model with EF Core 10 and review the complete generated Up, Down, designer, and snapshot files together.

## Current status

Validated on 2026-10-30.

The historical Magellan migration classes, designer files, and snapshot have been removed from the active Orbita migration assembly. The current branch contains exactly these three migration source files:

- `20261030204648_InitialOrbitaSchema.cs`
- `20261030204648_InitialOrbitaSchema.Designer.cs`
- `ApplicationDbContextModelSnapshot.cs`

The migration was generated with EF Core 10.0.14 from the checked-out `ApplicationDbContext` model after clearing build outputs and removing the previous migration sources. The migration's `Up()` method contains no `DropTable`, `DropColumn`, `RenameTable`, or `RenameColumn` operations. The `Down()` method contains the expected reverse operations for rolling back the initial schema.

Validation completed successfully:

- EF tooling reported no migrations before the new baseline was generated.
- `dotnet ef migrations has-pending-model-changes` passed after generation.
- Release build passed.
- Unit tests passed.
- PostgreSQL 18 Testcontainers integration tests passed, including migration application to an empty database and schema verification.
- Architecture tests passed.
- Architectural tribunal tests passed.

The generated baseline is now the authoritative Orbita schema lineage. This does not perform or imply a data-preserving import from Magellan. Any such import remains a separate workflow with its own source contract, mapping, validation, and idempotency guarantees.

## Generation procedure

From the repository root, after removing the historical migration classes and stale snapshot from the working tree:

```powershell
dotnet ef migrations add InitialOrbitaSchema `
  --project sources/back/Orbita.Persistence `
  --startup-project sources/back/Orbita.Api `
  --context ApplicationDbContext `
  --output-dir Migrations
```

If the API startup project cannot construct the design-time context without external services, use or add an IDesignTimeDbContextFactory<ApplicationDbContext> in the persistence project. The factory must use the same Npgsql provider and model configuration as runtime; it must not introduce a second, divergent model.

Then validate:

```powershell
dotnet ef migrations has-pending-model-changes `
  --project sources/back/Orbita.Persistence `
  --startup-project sources/back/Orbita.Api `
  --context ApplicationDbContext

dotnet test Orbita.slnx --configuration Release
```

The PostgreSQL integration suite must run against PostgreSQL 18 Testcontainers and apply migrations to an empty database using Database.MigrateAsync(). It must verify the resulting tables, columns, indexes, foreign keys, check constraints, repository round trips, uniqueness, transaction rollback, and optimistic concurrency.

## Import boundary

Any future Magellan data import must be isolated from the Orbita migration assembly. It must have its own explicit source schema contract, deterministic mapping, validation report, and idempotent execution strategy. It must never require Orbita's initial migration to know about historical Magellan tables.

## Safety rules

- Do not use EnsureCreated as a substitute for migrations.
- Do not use SQLite or EF InMemory to claim PostgreSQL correctness.
- Do not preserve legacy schema compatibility in Orbita's migration history.
- Do not silently coerce unknown source values during a future import.
- Keep the clean baseline isolated from historical Magellan migration lineage; validate future schema changes through generated migrations and PostgreSQL integration tests.
