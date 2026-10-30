# PostgreSQL Migration Reconciliation

## Purpose

Define the database compatibility strategy between the historical AstrolabaMagellan PostgreSQL schema and the Orbita persistence model before generating EF Core migrations.

This document is a migration design record. It does not claim that the schema conversion or migrations have been implemented.

## Source of truth inspected

The historical source is `bestacio89/AstrolabaMagellan`, branch `main`, pinned at `b88ab8177bbab47da46daf78c08cf7db253d296d` (the branch currently resolves to the same commit).

The source EF Core migration chain contains:

| Migration ID | Purpose |
|---|---|
| `20261019121926_Postgresqlmigration` | Initial PostgreSQL schema |
| `20261024160000_EvolutionsRevueBimensuelle` | Adds lots and generalized comments, extends planning axes, and migrates task updates into comments |

The source also contains `PilotageDbContextModelSnapshot.cs`. The migration source uses Npgsql PostgreSQL types, including `uuid`, `date`, `timestamp with time zone`, and `numeric(18,6)`.

Orbita currently has 50 explicitly named relational tables in its EF configurations. The two historical migration operations, their EF designer metadata, and the historical model snapshot have now been ported into `Orbita.Persistence.Migrations`, preserving their original migration IDs and PostgreSQL operations. The source CLR type names inside the historical snapshot are intentionally retained as migration metadata; the EF context association and migration namespace are Orbita-specific. The port compiled in the Release build on commit `f184a08bf29c34a78191dd6a304efb155dca067f`.

## Current implementation status

- [x] Historical initial migration ported with ID `20261019121926_Postgresqlmigration`.
- [x] Historical follow-up migration ported with ID `20261024160000_EvolutionsRevueBimensuelle`.
- [x] Historical Magellan model snapshot retained as migration-lineage metadata. It is **not** the current Orbita model snapshot.
- [x] PostgreSQL Testcontainers integration project added and explicitly wired into CI.
- [x] Historical migration chain applied to an empty PostgreSQL 18 database.
- [x] Historical migration constraints verified against PostgreSQL: external-identity uniqueness, lot-code uniqueness, and restrictive project-to-lot deletion.
- [x] Historical follow-up migration data-copy behavior covered by a populated PostgreSQL test: task updates are copied into comments before the legacy table is removed.
- [ ] Column-level source-to-target mapping and forward schema/data migration.
- [ ] Current Orbita model snapshot committed alongside the reviewed forward migration.
- [ ] Fresh PostgreSQL database reaches the current Orbita schema through the complete migration chain.
- [ ] Existing Magellan schema upgrades to Orbita without data loss.
- [ ] Current Orbita constraints, indexes, foreign keys, and delete behaviors verified against PostgreSQL.
- [ ] Optimistic concurrency verified through competing PostgreSQL-backed updates.
- [ ] Repository persistence/retrieval round trips verified against PostgreSQL.

### Important distinction

The historical migrations currently prove that the original Magellan schema can be created and evolved. They do **not** prove that the current Orbita model can be deployed to that database. The generated EF preview `20261030172607_OrbitaSchemaTranslation` was inspected and rejected because its default `Up` method drops and recreates 49 tables. It is not a safe migration and must not be committed as-is.

The current snapshot must be committed only with the reviewed forward migration that it describes. Replacing the historical snapshot early would make EF report an apparently current model while the database still contains the legacy schema.

## Migration strategy

We must support two distinct database lifecycles:

1. **Fresh installation:** the complete committed migration chain creates the current Orbita schema from an empty PostgreSQL database.
2. **Existing Magellan installation:** the historical schema and its data are upgraded to the Orbita schema without silently dropping or reinterpreting business data.

The migration chain must preserve the historical migration IDs and schema semantics as a compatibility baseline. The source migration operations and snapshot must be ported into the Orbita migration assembly with their provenance retained. The subsequent Orbita migration must explicitly transform the historical schema into the current English schema.

Do not replace this with a newly generated initial migration that assumes an empty database. That would make fresh installations work while leaving existing Magellan databases without a safe upgrade path.

Do not edit an already released migration to make it match the current model. Add a forward migration for every schema transformation.

## Candidate table mapping

The following is a first-pass semantic mapping based on the historical migration operations and current Orbita EF configurations. It is not yet a column-level approval. Each mapping must be verified against the source snapshot, target CLR properties, constraints, indexes, and data conversion rules before writing destructive or irreversible operations.

| Historical table | Orbita table | Migration treatment |
|---|---|---|
| `Utilisateurs` | `Users` | Rename and map external identity to tenant/object identity only where source data can be unambiguously resolved |
| `Projets` | `Projects` | Rename; review owned settings, unique key, enum values and creator relationship |
| `Habilitations` | `ProjectAssignments` | Transform role/assignment semantics; verify global roles are not incorrectly migrated as project assignments |
| `BrouillonsProjet` | `ProjectDrafts` | Rename and map author/co-author/modifier fields |
| `Journal` | `ProjectJournalEntries` | Rename and map audit payload |
| `Axes` | `ProjectAxes` | Rename |
| `Lots` | `ProjectLots` | Rename |
| `Phases` | `ProjectPhases` | Rename |
| `Etapes` | `ProjectSteps` | Rename |
| `TypesTache` | `ProjectTaskTypes` | Rename |
| `Taches` | `WorkTasks` | Rename; verify enum and nullable-field conversions |
| `ChargesTache` | `TaskEfforts` | Rename |
| `Consommes` | `TimeEntries` | Rename |
| `EvolutionsPerimetre` | `ScopeChanges` | Rename |
| `ProfilsProjet` | `ProjectProfiles` | Rename |
| `GrillesAgence` | `AgencyRates` | Rename |
| `PlanningsVendusProfil` | `SoldProfilePlannings` | Rename |
| `PlanningsVendusPhase` | `SoldPhasePlannings` | Rename |
| `PlanningsVendusPhaseMois` | `SoldPhaseMonths` | Rename |
| `PrevisionnelsProfil` | `ForecastProfiles` | Rename |
| `BudgetsAchat` | `PurchaseBudgetLines` | Rename |
| `BudgetsFrais` | `ExpenseBudgetLines` | Rename |
| `Prestataires` | `ProjectSuppliers` | Rename |
| `Metriques` | `ProjectMetrics` | Rename |
| `Echeances` | `InvoiceMilestones` | Rename; verify phase-link representation |
| `LiensEcheancePhase` | `InvoiceMilestonePhases` | Rename |
| `AchatsReels` | `ActualPurchases` | Rename |
| `FraisReels` | `ActualExpenses` | Rename |
| `ImportsFrais` | `ExpenseImports` | Rename |
| `PhotosIndicateurs` | `IndicatorSnapshots` | Rename |
| `ChampsFiche` | `ProjectFields` | Rename |
| `ImportsTsp` | `TspImportRecords` | Rename |
| `ValeursListeIa` | `AiReferenceValues` | Rename |
| `DatesPlanning` | `PlanningDates` | Rename |
| `Jalons` | `ProjectMilestones` | Rename |
| `ElementsPlanning` | `PlanningElements` | Rename |
| `PlansReference` | `ReferencePlans` | Rename |
| `LignesPlanReference` | `ReferencePlanTaskLines` | Rename |
| `DatesPlanReference` | `ReferencePlanDates` | Rename |
| `Ateliers` | `Workshops` | Rename |
| `Destinataires` | `WorkshopRecipients` | Verify recipient variants and workshop relationship |
| `Livrables` | `Deliverables` | Rename |
| `Commentaires` | `ProjectComments` | Rename; preserve polymorphic target identity |
| `MisesAJourTache` | `ProjectComments` | Already merged into `Commentaires` by the second historical migration; do not migrate twice |
| `PointsHebdo` | `WeeklyStatusPoints` | Rename |
| `Actions` | `ProjectActions` | Rename |
| `Decisions` | `ProjectDecisions` | Rename |
| `Risques` | `ProjectRisks` | Rename; review risk enum conversions |
| `Notifications` | `ProjectNotifications` | Rename |
| `Absences` | `Absences` | Retain name; review user identity and creator references |
| `PersonnesAjoutees` | `ProjectAddedPeople` | Rename |

### Additional target tables requiring explicit disposition

Orbita currently maps `ReferencePlanTaskLines`, `ReferencePlanDates`, and other target tables listed above, but the migration must still verify every target table against the historical snapshot. Tables with no historical equivalent must be classified as one of:

- genuinely new Orbita capability, created empty by the forward migration;
- a renamed or split source structure, with an explicit data transformation;
- a retired source structure, with a documented retention/archive decision.

No table or column may be dropped merely because it is absent from the new CLR model.

## Mandatory column-level reconciliation

Before generating the forward migration, produce a reviewed mapping for every historical column:

- source table and column;
- target table and property/column;
- PostgreSQL source and target types;
- nullability and default behavior;
- primary/foreign keys and delete behavior;
- indexes and uniqueness;
- enum/string conversion, including legacy values;
- data transformation or backfill;
- rollback feasibility and data-loss assessment.

Particular attention is required for:

- `Utilisateurs.IdentifiantExterne`: Orbita identity is tenant ID plus Entra object ID; an external identifier cannot be split or guessed.
- `Habilitations`: legacy role values must be distinguished from global application roles and project-scoped assignments.
- polymorphic comments and the historical data copy from `MisesAJourTache`;
- risk probability/impact conversions and any changed enum cardinality;
- date/time values and PostgreSQL `timestamp with time zone` semantics;
- owned project settings and serialized JSON/text payloads;
- decimal precision, currently intended to remain `numeric(18,6)`;
- optimistic concurrency version columns.

### Generated migration review

EF Core 10 successfully generated a draft migration from the historical snapshot to the current Orbita model in CI run `41098623438`. The generated draft contains 49 `DropTable` operations and 49 `CreateTable` operations in its `Up` method. It does not infer table renames because the source and target CLR entity identities differ.

**That generated migration is destructive and must not be committed as-is.** It would discard the historical data in the old tables. The generated preview was retained as a short-lived CI artifact for inspection only.

The forward migration must replace those drop/create pairs with reviewed data-preserving operations. For tables with equivalent semantics, use explicit PostgreSQL-safe table/column renames and targeted alterations. For changed shapes, add explicit backfills and validate row counts and relationships before removing legacy columns. Any source table without an unambiguous target must remain available until its disposition is approved.

### Confirmed identity and role semantics

The source API constructs `IdentifiantExterne` from the validated Entra claims as `{tid}:{oid}`. This permits a deterministic split into Orbita's `TenantId` and `ObjectId`, but the migration must validate both components as GUIDs and fail before changing user data if any non-empty source identity is malformed. It must not infer either value from email or display name.

The source project-assignment role values map to Orbita's project roles as follows:

| Historical role | Orbita role |
|---|---|
| `Consultant` | `Contributor` |
| `ChefDeProjet` | `ProjectManager` |
| `Pilotage` | `Steering` |
| `Lecteur` | `Reader` |

The historical `Utilisateur.Administrateur` flag is not a project assignment. Orbita derives global capabilities from validated Entra `roles` claims. The migration must preserve this legacy flag for review or provide an explicit reconciliation report; it must not silently promote it into a project role.

Risk probability/impact, project status/origin, task status, action/decision status, notification state, absence period and other string-backed historical enums require explicit, reviewed value maps before conversion to Orbita's integer-backed enums. Unknown values must abort the migration rather than fall back to a default.

Orbita's Franz-backed entities also introduce common audit and soft-delete columns. Their backfill values must be deterministic and must not fabricate historical actors or timestamps. Existing source creation/modification fields should be mapped where semantically equivalent; otherwise use an explicitly documented migration actor and source timestamp policy.

## Required implementation sequence

1. Historical migration operations, designer metadata and snapshot are ported with their original IDs and PostgreSQL annotations.
2. Generate the next migration against the actual Orbita model and review every operation; treat EF's output as a draft, not as a safe data migration.
3. Complete and review the column-level mapping for every source table, including common audit fields, enums, owned settings and relationships.
4. Replace generated drop/create operations with explicit rename/alter/data-copy operations where data continuity is required.
5. Add validated backfills for Entra identity and enum transformations; fail safely when a value cannot be migrated unambiguously.
6. Add a final current-model snapshot and verify that EF reports no pending model changes.
7. Test both paths against real PostgreSQL:
   - empty database through all migrations;
   - historical schema populated with representative legacy data through the full upgrade.
8. Verify row counts, key relationships, representative business values and constraints before and after upgrade.

## Data safety rules

- Never use `EnsureCreated` for this migration path.
- Never use SQLite or EF InMemory to claim PostgreSQL migration correctness.
- Never discard migration history or truncate production data to make a migration pass.
- Do not infer Entra tenant/object identifiers from email addresses or display names.
- Do not silently coerce unknown enum values to defaults.
- Keep destructive cleanup separate from schema conversion and only after explicit data-retention approval.
- All migration tests must use disposable PostgreSQL databases/containers and deterministic fixtures.

## Acceptance criteria

- The historical migration IDs remain recognized by EF Core.
- A clean PostgreSQL database reaches the current Orbita schema using only committed migrations.
- A database at the historical Magellan migration level upgrades without data loss.
- The current EF model has no pending model changes after the final migration.
- PostgreSQL integration tests cover migration, data preservation, constraints and concurrency.
- The migration SQL and mapping decisions are reviewed before the PR is considered complete.
