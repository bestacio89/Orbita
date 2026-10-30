using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbita.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialOrbitaSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ObjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    LastName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Absences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonKey = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PersonName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Period = table.Column<int>(type: "integer", nullable: false),
                    RecordedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RecordedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Absences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Absences_Users_RecordedByUserId",
                        column: x => x.RecordedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Absences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectDrafts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Client = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ProjectName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Reference = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Origin = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: false),
                    AuthorUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CoAuthorUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectDrafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectDrafts_Users_AuthorUserId",
                        column: x => x.AuthorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectDrafts_Users_CoAuthorUserId",
                        column: x => x.CoAuthorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectDrafts_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Client = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Reference = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    UniqueKey = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Agency = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ServiceLine = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Director = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ServiceLineManager = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    SalesRepresentative = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ClientContact = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ContractType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ContractualEndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ForecastEndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Discount = table.Column<decimal>(type: "numeric", nullable: false),
                    DiscountWasSpecified = table.Column<bool>(type: "boolean", nullable: false),
                    InitialRevenue = table.Column<decimal>(type: "numeric", nullable: true),
                    ManagementPercentage = table.Column<decimal>(type: "numeric", nullable: true),
                    ProvisionPercentage = table.Column<decimal>(type: "numeric", nullable: true),
                    Origin = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Settings_Grouping = table.Column<int>(type: "integer", nullable: false),
                    Settings_FirstPlanningMonth = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    Settings_MonthCount = table.Column<int>(type: "integer", nullable: false),
                    Settings_ActionAlertDays = table.Column<int>(type: "integer", nullable: true),
                    Settings_MilestoneAlertDays = table.Column<int>(type: "integer", nullable: true),
                    Settings_WorkshopAlertDays = table.Column<int>(type: "integer", nullable: true),
                    Settings_DeliverableAlertDays = table.Column<int>(type: "integer", nullable: true),
                    Settings_LastProjectCommitteeDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Settings_GovernanceInstances = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AgencyRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Profile = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ParisDailyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    ParisDailyCostRate = table.Column<decimal>(type: "numeric", nullable: false),
                    RegionalDailyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    RegionalDailyCostRate = table.Column<decimal>(type: "numeric", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgencyRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgencyRates_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AiReferenceValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ListKind = table.Column<int>(type: "integer", nullable: false),
                    Label = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiReferenceValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AiReferenceValues_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseBudgetLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    UnitCost = table.Column<decimal>(type: "numeric", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    RebillAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Comment = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseBudgetLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseBudgetLines_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseImports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Source = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FileName = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: true),
                    ImportedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ImportedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    RowCount = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseImports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseImports_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpenseImports_Users_ImportedByUserId",
                        column: x => x.ImportedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IndicatorSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    PlannedEffort = table.Column<decimal>(type: "numeric", nullable: false),
                    ActualEffort = table.Column<decimal>(type: "numeric", nullable: false),
                    RemainingEffort = table.Column<decimal>(type: "numeric", nullable: false),
                    ForecastEffort = table.Column<decimal>(type: "numeric", nullable: false),
                    FinalCost = table.Column<decimal>(type: "numeric", nullable: false),
                    Margin = table.Column<decimal>(type: "numeric", nullable: false),
                    NetMargin = table.Column<decimal>(type: "numeric", nullable: false),
                    NetMarginRate = table.Column<decimal>(type: "numeric", nullable: false),
                    ForecastEndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndicatorSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndicatorSnapshots_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceMilestones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Percentage = table.Column<decimal>(type: "numeric", nullable: false),
                    TspMonth = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    PlannedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    InvoicedOn = table.Column<DateOnly>(type: "date", nullable: true),
                    PaidOn = table.Column<DateOnly>(type: "date", nullable: true),
                    InvoicedAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    Comment = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    TriggerAxisCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceMilestones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceMilestones_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanningDates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanningDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanningDates_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanningElements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    Key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanningElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanningElements_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectAddedPeople",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAddedPeople", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectAddedPeople_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    AssignedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    GrantedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectAssignments_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectAssignments_Users_GrantedByUserId",
                        column: x => x.GrantedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectAssignments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectAxes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectAxes_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AuthorUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectComments_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectComments_Users_AuthorUserId",
                        column: x => x.AuthorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectDecisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Reference = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DecisionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    GovernanceInstance = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    DecisionMaker = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Impact = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectDecisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectDecisions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectDecisions_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Label = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    Value = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectFields_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectJournalEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    EntityType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    MemberName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    PreviousValue = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    NewValue = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectJournalEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectJournalEntries_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectJournalEntries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectLots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectLots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectLots_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMetrics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Step = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Percentage = table.Column<decimal>(type: "numeric", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectMetrics_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMilestones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    RequiresConfirmation = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMilestones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectMilestones_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipientUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SentAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeliveryStatus = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectNotifications_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectNotifications_Users_RecipientUserId",
                        column: x => x.RecipientUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectPhases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    IsReferenceValue = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectPhases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectPhases_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    LegalEntity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DailyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    DailyCostRate = table.Column<decimal>(type: "numeric", nullable: false),
                    CostRatePercentage = table.Column<decimal>(type: "numeric", nullable: true),
                    Expenses = table.Column<decimal>(type: "numeric", nullable: false),
                    Collaborator = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Origin = table.Column<int>(type: "integer", nullable: false),
                    AddedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AddedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    AdditionReason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsAiAgent = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectProfiles_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectProfiles_Users_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectSteps_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectSuppliers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Company = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DailyCost = table.Column<decimal>(type: "numeric", nullable: false),
                    PersonDays = table.Column<decimal>(type: "numeric", nullable: false),
                    DailyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    Comment = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSuppliers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectSuppliers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTaskTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsCoreType = table.Column<bool>(type: "boolean", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTaskTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectTaskTypes_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseBudgetLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UnitCost = table.Column<decimal>(type: "numeric", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    ResaleAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Comment = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseBudgetLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseBudgetLines_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReferencePlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SnapshotDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FrozenByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferencePlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReferencePlans_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReferencePlans_Users_FrozenByUserId",
                        column: x => x.FrozenByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SoldPhasePlannings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Phase = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    EnteredDailyCostRate = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoldPhasePlannings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SoldPhasePlannings_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TspImportRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: true),
                    ImportedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ImportedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ObservedPoints = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    ProfilePlanningSource = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    PhasePlanningSource = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    HiddenByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TspImportRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TspImportRecords_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TspImportRecords_Users_HiddenByUserId",
                        column: x => x.HiddenByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TspImportRecords_Users_ImportedByUserId",
                        column: x => x.ImportedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyStatusPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyStatusPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyStatusPoints_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeeklyStatusPoints_Users_ModifiedByUserId",
                        column: x => x.ModifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ActualExpenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpenseImportId = table.Column<Guid>(type: "uuid", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Collaborator = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    IsRebillable = table.Column<bool>(type: "boolean", nullable: false),
                    Comment = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActualExpenses", x => x.Id);
                    table.CheckConstraint("CK_ActualExpenses_Amount_Positive", "\"Amount\" > 0");
                    table.ForeignKey(
                        name: "FK_ActualExpenses_ExpenseImports_ExpenseImportId",
                        column: x => x.ExpenseImportId,
                        principalTable: "ExpenseImports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActualExpenses_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActualExpenses_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceMilestonePhases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceMilestoneId = table.Column<Guid>(type: "uuid", nullable: false),
                    PhaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceMilestonePhases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceMilestonePhases_InvoiceMilestones_InvoiceMilestoneId",
                        column: x => x.InvoiceMilestoneId,
                        principalTable: "InvoiceMilestones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceMilestonePhases_ProjectPhases_PhaseId",
                        column: x => x.PhaseId,
                        principalTable: "ProjectPhases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceMilestonePhases_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ForecastProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Month = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    PersonDays = table.Column<decimal>(type: "numeric", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForecastProfiles_ProjectProfiles_ProjectProfileId",
                        column: x => x.ProjectProfileId,
                        principalTable: "ProjectProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForecastProfiles_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SoldProfilePlannings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Month = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    PersonDays = table.Column<decimal>(type: "numeric", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoldProfilePlannings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SoldProfilePlannings_ProjectProfiles_ProjectProfileId",
                        column: x => x.ProjectProfileId,
                        principalTable: "ProjectProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SoldProfilePlannings_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Reference = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    Origin = table.Column<int>(type: "integer", nullable: false),
                    TspRow = table.Column<int>(type: "integer", nullable: true),
                    TspAxisCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TspLot = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TspPhase = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TspStep = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TspTitle = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Assumption = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    DeliverableMarker = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsSection = table.Column<bool>(type: "boolean", nullable: false),
                    HasDeliverableColumn = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AxisId = table.Column<Guid>(type: "uuid", nullable: true),
                    PhaseId = table.Column<Guid>(type: "uuid", nullable: true),
                    Lot = table.Column<string>(type: "text", nullable: true),
                    TaskTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    TaskTypeWasExplicitlySet = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true),
                    Progress = table.Column<decimal>(type: "numeric", nullable: true),
                    RemainingEffort = table.Column<decimal>(type: "numeric", nullable: true),
                    PlannedStartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PlannedEndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Comment = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    ValidatedOn = table.Column<DateOnly>(type: "date", nullable: true),
                    AiOptimizationType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AiComment = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    AiRevisedEffort = table.Column<decimal>(type: "numeric", nullable: true),
                    InvoiceMilestoneId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTasks", x => x.Id);
                    table.CheckConstraint("CK_WorkTasks_Progress", "\"Progress\" IS NULL OR (\"Progress\" >= 0 AND \"Progress\" <= 1)");
                    table.ForeignKey(
                        name: "FK_WorkTasks_InvoiceMilestones_InvoiceMilestoneId",
                        column: x => x.InvoiceMilestoneId,
                        principalTable: "InvoiceMilestones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkTasks_ProjectAxes_AxisId",
                        column: x => x.AxisId,
                        principalTable: "ProjectAxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkTasks_ProjectPhases_PhaseId",
                        column: x => x.PhaseId,
                        principalTable: "ProjectPhases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkTasks_ProjectTaskTypes_TaskTypeId",
                        column: x => x.TaskTypeId,
                        principalTable: "ProjectTaskTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkTasks_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActualPurchases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchaseBudgetLineId = table.Column<Guid>(type: "uuid", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Supplier = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitCost = table.Column<decimal>(type: "numeric", nullable: false),
                    RebillAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActualPurchases", x => x.Id);
                    table.CheckConstraint("CK_ActualPurchases_Amounts", "\"Quantity\" > 0 AND \"UnitCost\" >= 0 AND \"RebillAmount\" >= 0");
                    table.ForeignKey(
                        name: "FK_ActualPurchases_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActualPurchases_PurchaseBudgetLines_PurchaseBudgetLineId",
                        column: x => x.PurchaseBudgetLineId,
                        principalTable: "PurchaseBudgetLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActualPurchases_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReferencePlanDates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReferencePlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferencePlanDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReferencePlanDates_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReferencePlanDates_ReferencePlans_ReferencePlanId",
                        column: x => x.ReferencePlanId,
                        principalTable: "ReferencePlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SoldPhaseMonths",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SoldPhasePlanningId = table.Column<Guid>(type: "uuid", nullable: false),
                    Month = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    PersonDays = table.Column<decimal>(type: "numeric", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoldPhaseMonths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SoldPhaseMonths_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SoldPhaseMonths_SoldPhasePlannings_SoldPhasePlanningId",
                        column: x => x.SoldPhasePlanningId,
                        principalTable: "SoldPhasePlannings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Deliverables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientValidatedOn = table.Column<DateOnly>(type: "date", nullable: true),
                    StorageUri = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliverables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deliverables_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Deliverables_WorkTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "WorkTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Reference = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    OwnerName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    GovernanceInstance = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedOn = table.Column<DateOnly>(type: "date", nullable: false),
                    TargetDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Progress = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CompletedOn = table.Column<DateOnly>(type: "date", nullable: true),
                    Comment = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectActions", x => x.Id);
                    table.CheckConstraint("CK_ProjectActions_Progress", "\"Progress\" >= 0 AND \"Progress\" <= 1");
                    table.ForeignKey(
                        name: "FK_ProjectActions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectActions_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectActions_Users_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectActions_WorkTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "WorkTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReferencePlanTaskLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReferencePlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlannedStartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PlannedEndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PlannedEffort = table.Column<decimal>(type: "numeric", nullable: false),
                    VarianceCause = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VarianceExplanation = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferencePlanTaskLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReferencePlanTaskLines_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReferencePlanTaskLines_ReferencePlans_ReferencePlanId",
                        column: x => x.ReferencePlanId,
                        principalTable: "ReferencePlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReferencePlanTaskLines_WorkTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "WorkTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScopeChanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    SourceProjectProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ActorUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsContractAmendment = table.Column<bool>(type: "boolean", nullable: false),
                    PersonDays = table.Column<decimal>(type: "numeric", nullable: false),
                    DecisionId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScopeChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScopeChanges_ProjectDecisions_DecisionId",
                        column: x => x.DecisionId,
                        principalTable: "ProjectDecisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScopeChanges_ProjectProfiles_ProjectProfileId",
                        column: x => x.ProjectProfileId,
                        principalTable: "ProjectProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScopeChanges_ProjectProfiles_SourceProjectProfileId",
                        column: x => x.SourceProjectProfileId,
                        principalTable: "ProjectProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScopeChanges_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScopeChanges_Users_ActorUserId",
                        column: x => x.ActorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScopeChanges_WorkTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "WorkTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskEfforts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    SoldEffort = table.Column<decimal>(type: "numeric", nullable: false),
                    AdditionalEffort = table.Column<decimal>(type: "numeric", nullable: false),
                    AdditionReason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    AddedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AddedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskEfforts", x => x.Id);
                    table.CheckConstraint("CK_TaskEfforts_NonNegative", "\"SoldEffort\" >= 0 AND \"AdditionalEffort\" >= 0");
                    table.ForeignKey(
                        name: "FK_TaskEfforts_ProjectProfiles_ProjectProfileId",
                        column: x => x.ProjectProfileId,
                        principalTable: "ProjectProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskEfforts_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskEfforts_Users_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskEfforts_WorkTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "WorkTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PersonDays = table.Column<decimal>(type: "numeric", nullable: false),
                    RecordedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RecordedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeEntries", x => x.Id);
                    table.CheckConstraint("CK_TimeEntries_PersonDays_Positive", "\"PersonDays\" > 0");
                    table.ForeignKey(
                        name: "FK_TimeEntries_ProjectProfiles_ProjectProfileId",
                        column: x => x.ProjectProfileId,
                        principalTable: "ProjectProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimeEntries_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TimeEntries_Users_RecordedByUserId",
                        column: x => x.RecordedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimeEntries_WorkTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "WorkTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Workshops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledDate = table.Column<DateOnly>(type: "date", nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    InvitationSent = table.Column<bool>(type: "boolean", nullable: false),
                    InvitationSentOn = table.Column<DateOnly>(type: "date", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workshops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workshops_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Workshops_WorkTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "WorkTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectRisks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Reference = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Probability = table.Column<int>(type: "integer", nullable: false),
                    Impact = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: true),
                    Cause = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Mitigation = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ActionPlan = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Owner = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActionId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRisks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectRisks_ProjectActions_ActionId",
                        column: x => x.ActionId,
                        principalTable: "ProjectActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectRisks_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectRisks_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkshopRecipients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkshopId = table.Column<Guid>(type: "uuid", nullable: false),
                    NameOrEmail = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkshopRecipients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkshopRecipients_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkshopRecipients_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkshopRecipients_Workshops_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "Workshops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Absences_PersonKey_Date",
                table: "Absences",
                columns: new[] { "PersonKey", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Absences_RecordedByUserId",
                table: "Absences",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Absences_UserId",
                table: "Absences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActualExpenses_CreatedByUserId",
                table: "ActualExpenses",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActualExpenses_ExpenseImportId",
                table: "ActualExpenses",
                column: "ExpenseImportId");

            migrationBuilder.CreateIndex(
                name: "IX_ActualExpenses_ProjectId",
                table: "ActualExpenses",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ActualExpenses_ProjectId_Date",
                table: "ActualExpenses",
                columns: new[] { "ProjectId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_ActualPurchases_CreatedByUserId",
                table: "ActualPurchases",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActualPurchases_ProjectId",
                table: "ActualPurchases",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ActualPurchases_ProjectId_Date",
                table: "ActualPurchases",
                columns: new[] { "ProjectId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_ActualPurchases_PurchaseBudgetLineId",
                table: "ActualPurchases",
                column: "PurchaseBudgetLineId");

            migrationBuilder.CreateIndex(
                name: "IX_AgencyRates_ProjectId",
                table: "AgencyRates",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AgencyRates_ProjectId_Order",
                table: "AgencyRates",
                columns: new[] { "ProjectId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_AiReferenceValues_ProjectId",
                table: "AiReferenceValues",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AiReferenceValues_ProjectId_ListKind_Label",
                table: "AiReferenceValues",
                columns: new[] { "ProjectId", "ListKind", "Label" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deliverables_ProjectId",
                table: "Deliverables",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliverables_TaskId",
                table: "Deliverables",
                column: "TaskId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseBudgetLines_ProjectId",
                table: "ExpenseBudgetLines",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseBudgetLines_ProjectId_Order",
                table: "ExpenseBudgetLines",
                columns: new[] { "ProjectId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseImports_ImportedByUserId",
                table: "ExpenseImports",
                column: "ImportedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseImports_ProjectId",
                table: "ExpenseImports",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseImports_ProjectId_ImportedAt",
                table: "ExpenseImports",
                columns: new[] { "ProjectId", "ImportedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ForecastProfiles_ProjectId",
                table: "ForecastProfiles",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ForecastProfiles_ProjectProfileId_Month",
                table: "ForecastProfiles",
                columns: new[] { "ProjectProfileId", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndicatorSnapshots_ProjectId",
                table: "IndicatorSnapshots",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_IndicatorSnapshots_ProjectId_Date_Kind",
                table: "IndicatorSnapshots",
                columns: new[] { "ProjectId", "Date", "Kind" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceMilestonePhases_InvoiceMilestoneId_PhaseId",
                table: "InvoiceMilestonePhases",
                columns: new[] { "InvoiceMilestoneId", "PhaseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceMilestonePhases_PhaseId",
                table: "InvoiceMilestonePhases",
                column: "PhaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceMilestonePhases_ProjectId",
                table: "InvoiceMilestonePhases",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceMilestones_ProjectId",
                table: "InvoiceMilestones",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceMilestones_ProjectId_Number",
                table: "InvoiceMilestones",
                columns: new[] { "ProjectId", "Number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanningDates_ProjectId",
                table: "PlanningDates",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanningDates_ProjectId_Key",
                table: "PlanningDates",
                columns: new[] { "ProjectId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanningElements_ProjectId",
                table: "PlanningElements",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanningElements_ProjectId_Kind_Key",
                table: "PlanningElements",
                columns: new[] { "ProjectId", "Kind", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectActions_CreatedByUserId",
                table: "ProjectActions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectActions_OwnerUserId",
                table: "ProjectActions",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectActions_ProjectId",
                table: "ProjectActions",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectActions_ProjectId_Reference",
                table: "ProjectActions",
                columns: new[] { "ProjectId", "Reference" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectActions_TaskId",
                table: "ProjectActions",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAddedPeople_ProjectId",
                table: "ProjectAddedPeople",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAddedPeople_ProjectId_Key",
                table: "ProjectAddedPeople",
                columns: new[] { "ProjectId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAssignments_GrantedByUserId",
                table: "ProjectAssignments",
                column: "GrantedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAssignments_ProjectId",
                table: "ProjectAssignments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAssignments_ProjectId_UserId",
                table: "ProjectAssignments",
                columns: new[] { "ProjectId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAssignments_UserId",
                table: "ProjectAssignments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAxes_ProjectId",
                table: "ProjectAxes",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAxes_ProjectId_Code",
                table: "ProjectAxes",
                columns: new[] { "ProjectId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectComments_AuthorUserId",
                table: "ProjectComments",
                column: "AuthorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectComments_ProjectId",
                table: "ProjectComments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectComments_ProjectId_TargetType_TargetId_CreatedAt",
                table: "ProjectComments",
                columns: new[] { "ProjectId", "TargetType", "TargetId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDecisions_CreatedByUserId",
                table: "ProjectDecisions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDecisions_ProjectId",
                table: "ProjectDecisions",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDecisions_ProjectId_Reference",
                table: "ProjectDecisions",
                columns: new[] { "ProjectId", "Reference" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDrafts_AuthorUserId_ModifiedAt",
                table: "ProjectDrafts",
                columns: new[] { "AuthorUserId", "ModifiedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDrafts_CoAuthorUserId_ModifiedAt",
                table: "ProjectDrafts",
                columns: new[] { "CoAuthorUserId", "ModifiedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDrafts_ModifiedByUserId",
                table: "ProjectDrafts",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFields_ProjectId",
                table: "ProjectFields",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFields_ProjectId_Order",
                table: "ProjectFields",
                columns: new[] { "ProjectId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectJournalEntries_ProjectId",
                table: "ProjectJournalEntries",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectJournalEntries_ProjectId_EntityType_EntityId_Occurre~",
                table: "ProjectJournalEntries",
                columns: new[] { "ProjectId", "EntityType", "EntityId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectJournalEntries_ProjectId_OccurredAt",
                table: "ProjectJournalEntries",
                columns: new[] { "ProjectId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectJournalEntries_UserId",
                table: "ProjectJournalEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLots_ProjectId",
                table: "ProjectLots",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLots_ProjectId_Code",
                table: "ProjectLots",
                columns: new[] { "ProjectId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMetrics_ProjectId",
                table: "ProjectMetrics",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMetrics_ProjectId_Order",
                table: "ProjectMetrics",
                columns: new[] { "ProjectId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMilestones_ProjectId",
                table: "ProjectMilestones",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectNotifications_ProjectId",
                table: "ProjectNotifications",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectNotifications_ProjectId_SubjectId_Kind",
                table: "ProjectNotifications",
                columns: new[] { "ProjectId", "SubjectId", "Kind" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectNotifications_RecipientUserId_DeliveryStatus_TargetD~",
                table: "ProjectNotifications",
                columns: new[] { "RecipientUserId", "DeliveryStatus", "TargetDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhases_ProjectId",
                table: "ProjectPhases",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhases_ProjectId_Code",
                table: "ProjectPhases",
                columns: new[] { "ProjectId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProfiles_AddedByUserId",
                table: "ProjectProfiles",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProfiles_ProjectId",
                table: "ProjectProfiles",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProfiles_ProjectId_Rank",
                table: "ProjectProfiles",
                columns: new[] { "ProjectId", "Rank" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProfiles_UserId",
                table: "ProjectProfiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRisks_ActionId",
                table: "ProjectRisks",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRisks_CreatedByUserId",
                table: "ProjectRisks",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRisks_ProjectId",
                table: "ProjectRisks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRisks_ProjectId_Reference",
                table: "ProjectRisks",
                columns: new[] { "ProjectId", "Reference" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatedByUserId",
                table: "Projects",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UniqueKey",
                table: "Projects",
                column: "UniqueKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSteps_ProjectId",
                table: "ProjectSteps",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSteps_ProjectId_Order",
                table: "ProjectSteps",
                columns: new[] { "ProjectId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSuppliers_ProjectId",
                table: "ProjectSuppliers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSuppliers_ProjectId_Order",
                table: "ProjectSuppliers",
                columns: new[] { "ProjectId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskTypes_ProjectId",
                table: "ProjectTaskTypes",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskTypes_ProjectId_Code",
                table: "ProjectTaskTypes",
                columns: new[] { "ProjectId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseBudgetLines_ProjectId",
                table: "PurchaseBudgetLines",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseBudgetLines_ProjectId_Order",
                table: "PurchaseBudgetLines",
                columns: new[] { "ProjectId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_ReferencePlanDates_ProjectId",
                table: "ReferencePlanDates",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferencePlanDates_ReferencePlanId_Key",
                table: "ReferencePlanDates",
                columns: new[] { "ReferencePlanId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReferencePlans_FrozenByUserId",
                table: "ReferencePlans",
                column: "FrozenByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferencePlans_ProjectId",
                table: "ReferencePlans",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferencePlanTaskLines_ProjectId",
                table: "ReferencePlanTaskLines",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferencePlanTaskLines_ReferencePlanId_TaskId",
                table: "ReferencePlanTaskLines",
                columns: new[] { "ReferencePlanId", "TaskId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReferencePlanTaskLines_TaskId",
                table: "ReferencePlanTaskLines",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ScopeChanges_ActorUserId",
                table: "ScopeChanges",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ScopeChanges_DecisionId",
                table: "ScopeChanges",
                column: "DecisionId");

            migrationBuilder.CreateIndex(
                name: "IX_ScopeChanges_ProjectId",
                table: "ScopeChanges",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ScopeChanges_ProjectProfileId",
                table: "ScopeChanges",
                column: "ProjectProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ScopeChanges_SourceProjectProfileId",
                table: "ScopeChanges",
                column: "SourceProjectProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ScopeChanges_TaskId_OccurredAt",
                table: "ScopeChanges",
                columns: new[] { "TaskId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SoldPhaseMonths_ProjectId",
                table: "SoldPhaseMonths",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SoldPhaseMonths_SoldPhasePlanningId_Month",
                table: "SoldPhaseMonths",
                columns: new[] { "SoldPhasePlanningId", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SoldPhasePlannings_ProjectId",
                table: "SoldPhasePlannings",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SoldProfilePlannings_ProjectId",
                table: "SoldProfilePlannings",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SoldProfilePlannings_ProjectProfileId_Month",
                table: "SoldProfilePlannings",
                columns: new[] { "ProjectProfileId", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskEfforts_AddedByUserId",
                table: "TaskEfforts",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskEfforts_ProjectId",
                table: "TaskEfforts",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskEfforts_ProjectProfileId",
                table: "TaskEfforts",
                column: "ProjectProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskEfforts_TaskId_ProjectProfileId",
                table: "TaskEfforts",
                columns: new[] { "TaskId", "ProjectProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_ProjectId",
                table: "TimeEntries",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_ProjectProfileId_WorkDate",
                table: "TimeEntries",
                columns: new[] { "ProjectProfileId", "WorkDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_RecordedByUserId",
                table: "TimeEntries",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_TaskId_ProjectProfileId_WorkDate",
                table: "TimeEntries",
                columns: new[] { "TaskId", "ProjectProfileId", "WorkDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TspImportRecords_HiddenByUserId",
                table: "TspImportRecords",
                column: "HiddenByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TspImportRecords_ImportedByUserId",
                table: "TspImportRecords",
                column: "ImportedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TspImportRecords_ProjectId",
                table: "TspImportRecords",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TspImportRecords_ProjectId_ImportedAt",
                table: "TspImportRecords",
                columns: new[] { "ProjectId", "ImportedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_ObjectId",
                table: "Users",
                columns: new[] { "TenantId", "ObjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyStatusPoints_ModifiedByUserId",
                table: "WeeklyStatusPoints",
                column: "ModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyStatusPoints_ProjectId",
                table: "WeeklyStatusPoints",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyStatusPoints_ProjectId_Key",
                table: "WeeklyStatusPoints",
                columns: new[] { "ProjectId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopRecipients_ProjectId",
                table: "WorkshopRecipients",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopRecipients_UserId",
                table: "WorkshopRecipients",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopRecipients_WorkshopId_Order",
                table: "WorkshopRecipients",
                columns: new[] { "WorkshopId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_Workshops_ProjectId",
                table: "Workshops",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Workshops_TaskId",
                table: "Workshops",
                column: "TaskId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkTasks_AxisId",
                table: "WorkTasks",
                column: "AxisId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTasks_InvoiceMilestoneId",
                table: "WorkTasks",
                column: "InvoiceMilestoneId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTasks_PhaseId",
                table: "WorkTasks",
                column: "PhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTasks_ProjectId",
                table: "WorkTasks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTasks_ProjectId_Position",
                table: "WorkTasks",
                columns: new[] { "ProjectId", "Position" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkTasks_ProjectId_Reference",
                table: "WorkTasks",
                columns: new[] { "ProjectId", "Reference" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkTasks_TaskTypeId",
                table: "WorkTasks",
                column: "TaskTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Absences");

            migrationBuilder.DropTable(
                name: "ActualExpenses");

            migrationBuilder.DropTable(
                name: "ActualPurchases");

            migrationBuilder.DropTable(
                name: "AgencyRates");

            migrationBuilder.DropTable(
                name: "AiReferenceValues");

            migrationBuilder.DropTable(
                name: "Deliverables");

            migrationBuilder.DropTable(
                name: "ExpenseBudgetLines");

            migrationBuilder.DropTable(
                name: "ForecastProfiles");

            migrationBuilder.DropTable(
                name: "IndicatorSnapshots");

            migrationBuilder.DropTable(
                name: "InvoiceMilestonePhases");

            migrationBuilder.DropTable(
                name: "PlanningDates");

            migrationBuilder.DropTable(
                name: "PlanningElements");

            migrationBuilder.DropTable(
                name: "ProjectAddedPeople");

            migrationBuilder.DropTable(
                name: "ProjectAssignments");

            migrationBuilder.DropTable(
                name: "ProjectComments");

            migrationBuilder.DropTable(
                name: "ProjectDrafts");

            migrationBuilder.DropTable(
                name: "ProjectFields");

            migrationBuilder.DropTable(
                name: "ProjectJournalEntries");

            migrationBuilder.DropTable(
                name: "ProjectLots");

            migrationBuilder.DropTable(
                name: "ProjectMetrics");

            migrationBuilder.DropTable(
                name: "ProjectMilestones");

            migrationBuilder.DropTable(
                name: "ProjectNotifications");

            migrationBuilder.DropTable(
                name: "ProjectRisks");

            migrationBuilder.DropTable(
                name: "ProjectSteps");

            migrationBuilder.DropTable(
                name: "ProjectSuppliers");

            migrationBuilder.DropTable(
                name: "ReferencePlanDates");

            migrationBuilder.DropTable(
                name: "ReferencePlanTaskLines");

            migrationBuilder.DropTable(
                name: "ScopeChanges");

            migrationBuilder.DropTable(
                name: "SoldPhaseMonths");

            migrationBuilder.DropTable(
                name: "SoldProfilePlannings");

            migrationBuilder.DropTable(
                name: "TaskEfforts");

            migrationBuilder.DropTable(
                name: "TimeEntries");

            migrationBuilder.DropTable(
                name: "TspImportRecords");

            migrationBuilder.DropTable(
                name: "WeeklyStatusPoints");

            migrationBuilder.DropTable(
                name: "WorkshopRecipients");

            migrationBuilder.DropTable(
                name: "ExpenseImports");

            migrationBuilder.DropTable(
                name: "PurchaseBudgetLines");

            migrationBuilder.DropTable(
                name: "ProjectActions");

            migrationBuilder.DropTable(
                name: "ReferencePlans");

            migrationBuilder.DropTable(
                name: "ProjectDecisions");

            migrationBuilder.DropTable(
                name: "SoldPhasePlannings");

            migrationBuilder.DropTable(
                name: "ProjectProfiles");

            migrationBuilder.DropTable(
                name: "Workshops");

            migrationBuilder.DropTable(
                name: "WorkTasks");

            migrationBuilder.DropTable(
                name: "InvoiceMilestones");

            migrationBuilder.DropTable(
                name: "ProjectAxes");

            migrationBuilder.DropTable(
                name: "ProjectPhases");

            migrationBuilder.DropTable(
                name: "ProjectTaskTypes");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
