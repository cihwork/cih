using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleva.Services.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AiInteractions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: true),
                    ContextType = table.Column<int>(type: "INTEGER", nullable: false),
                    ContextId = table.Column<int>(type: "INTEGER", nullable: true),
                    Prompt = table.Column<string>(type: "TEXT", nullable: false),
                    PromptHash = table.Column<string>(type: "TEXT", nullable: true),
                    ResponseSummary = table.Column<string>(type: "TEXT", nullable: true),
                    ModelUsed = table.Column<string>(type: "TEXT", nullable: true),
                    TokensIn = table.Column<int>(type: "INTEGER", nullable: true),
                    TokensOut = table.Column<int>(type: "INTEGER", nullable: true),
                    ConfidenceScore = table.Column<decimal>(type: "TEXT", nullable: true),
                    CreatedById = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiInteractions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AiRecommendations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: true),
                    ContextType = table.Column<int>(type: "INTEGER", nullable: false),
                    ContextId = table.Column<int>(type: "INTEGER", nullable: true),
                    RecommendationType = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Rationale = table.Column<string>(type: "TEXT", nullable: true),
                    SuggestedActionsJson = table.Column<string>(type: "TEXT", nullable: true),
                    ModelUsed = table.Column<string>(type: "TEXT", nullable: true),
                    ConfidenceScore = table.Column<decimal>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewedById = table.Column<int>(type: "INTEGER", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiRecommendations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    QuestionId = table.Column<int>(type: "INTEGER", nullable: false),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: true),
                    TextValue = table.Column<string>(type: "TEXT", nullable: true),
                    NumericValue = table.Column<decimal>(type: "TEXT", nullable: true),
                    BooleanValue = table.Column<bool>(type: "INTEGER", nullable: true),
                    JsonValue = table.Column<string>(type: "TEXT", nullable: true),
                    Score = table.Column<decimal>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    EntityType = table.Column<string>(type: "TEXT", nullable: false),
                    EntityId = table.Column<string>(type: "TEXT", nullable: false),
                    ChangesJson = table.Column<string>(type: "TEXT", nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    MetadataJson = table.Column<string>(type: "TEXT", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BscGoals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PerformanceCycleId = table.Column<int>(type: "INTEGER", nullable: false),
                    PerspectiveId = table.Column<int>(type: "INTEGER", nullable: false),
                    DepartmentId = table.Column<int>(type: "INTEGER", nullable: true),
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: true),
                    CascadedFromGoalId = table.Column<int>(type: "INTEGER", nullable: true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Weight = table.Column<decimal>(type: "TEXT", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Progress = table.Column<decimal>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BscGoals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BscIndicators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BscGoalId = table.Column<int>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Formula = table.Column<string>(type: "TEXT", nullable: true),
                    UnitOfMeasure = table.Column<string>(type: "TEXT", nullable: true),
                    BaselineValue = table.Column<decimal>(type: "TEXT", nullable: true),
                    TargetValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    CurrentValue = table.Column<decimal>(type: "TEXT", nullable: true),
                    MeasurementFrequency = table.Column<string>(type: "TEXT", nullable: true),
                    DataSource = table.Column<string>(type: "TEXT", nullable: true),
                    IsHigherBetter = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    LastMeasuredDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BscIndicators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BscPerspectives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BscPerspectives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChangeLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntityType = table.Column<string>(type: "TEXT", nullable: false),
                    EntityId = table.Column<string>(type: "TEXT", nullable: false),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    BeforeJson = table.Column<string>(type: "TEXT", nullable: true),
                    AfterJson = table.Column<string>(type: "TEXT", nullable: true),
                    ChangedById = table.Column<int>(type: "INTEGER", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Source = table.Column<int>(type: "INTEGER", nullable: false),
                    CorrelationId = table.Column<string>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CheckIns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ManagerId = table.Column<int>(type: "INTEGER", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Mood = table.Column<int>(type: "INTEGER", nullable: false),
                    EnergyLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductivityScore = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    Blockers = table.Column<string>(type: "TEXT", nullable: true),
                    SupportNeeded = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckIns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Competencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Category = table.Column<int>(type: "INTEGER", nullable: false),
                    LevelType = table.Column<int>(type: "INTEGER", nullable: false),
                    LevelDefinitionsJson = table.Column<string>(type: "TEXT", nullable: true),
                    IsCore = table.Column<bool>(type: "INTEGER", nullable: false),
                    ParentCompetencyId = table.Column<int>(type: "INTEGER", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompetencyLibraries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<string>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetencyLibraries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Configurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    IsSecret = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsEncrypted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentDepartmentId = table.Column<int>(type: "INTEGER", nullable: true),
                    ManagerId = table.Column<int>(type: "INTEGER", nullable: true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssessedById = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PrimaryStyle = table.Column<int>(type: "INTEGER", nullable: true),
                    SecondaryStyle = table.Column<int>(type: "INTEGER", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AssessmentLink = table.Column<string>(type: "TEXT", nullable: true),
                    ReportUrl = table.Column<string>(type: "TEXT", nullable: true),
                    RawDataJson = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscAssessments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiscAssessmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionCode = table.Column<string>(type: "TEXT", nullable: false),
                    AnswerValue = table.Column<string>(type: "TEXT", nullable: true),
                    Axis = table.Column<string>(type: "TEXT", nullable: true),
                    AnsweredAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscResponses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiscAssessmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    DScore = table.Column<decimal>(type: "TEXT", nullable: false),
                    IScore = table.Column<decimal>(type: "TEXT", nullable: false),
                    SScore = table.Column<decimal>(type: "TEXT", nullable: false),
                    CScore = table.Column<decimal>(type: "TEXT", nullable: false),
                    PrimaryStyle = table.Column<int>(type: "INTEGER", nullable: false),
                    SecondaryStyle = table.Column<int>(type: "INTEGER", nullable: true),
                    ProfileCombination = table.Column<string>(type: "TEXT", nullable: false),
                    Interpretation = table.Column<string>(type: "TEXT", nullable: true),
                    StrengthsJson = table.Column<string>(type: "TEXT", nullable: true),
                    DevelopmentAreasJson = table.Column<string>(type: "TEXT", nullable: true),
                    CommunicationTipsJson = table.Column<string>(type: "TEXT", nullable: true),
                    StressIndicatorsJson = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EcAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationalUnitId = table.Column<int>(type: "INTEGER", nullable: true),
                    AssessmentType = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ConsciousnessLevel = table.Column<int>(type: "INTEGER", nullable: true),
                    ParticipationRate = table.Column<decimal>(type: "TEXT", nullable: true),
                    OverallScore = table.Column<decimal>(type: "TEXT", nullable: true),
                    DimensionsJson = table.Column<string>(type: "TEXT", nullable: true),
                    Findings = table.Column<string>(type: "TEXT", nullable: true),
                    Recommendations = table.Column<string>(type: "TEXT", nullable: true),
                    ConductedById = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcAssessments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EcResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EcAssessmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: true),
                    QuestionId = table.Column<int>(type: "INTEGER", nullable: true),
                    ResponseDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AnswersJson = table.Column<string>(type: "TEXT", nullable: false),
                    Sentiment = table.Column<int>(type: "INTEGER", nullable: true),
                    EnergyLevel = table.Column<int>(type: "INTEGER", nullable: true),
                    MetadataJson = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcResponses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EcResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EcAssessmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    MaturityLevel = table.Column<int>(type: "INTEGER", nullable: true),
                    ParticipationRate = table.Column<decimal>(type: "TEXT", nullable: true),
                    OverallScore = table.Column<decimal>(type: "TEXT", nullable: true),
                    StrengthsJson = table.Column<string>(type: "TEXT", nullable: true),
                    RisksJson = table.Column<string>(type: "TEXT", nullable: true),
                    RecommendationsJson = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    DocumentNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    AvatarUrl = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    DepartmentId = table.Column<int>(type: "INTEGER", nullable: true),
                    PositionId = table.Column<int>(type: "INTEGER", nullable: true),
                    ManagerId = table.Column<int>(type: "INTEGER", nullable: true),
                    HireDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PositionTitle = table.Column<string>(type: "TEXT", nullable: true),
                    Timezone = table.Column<string>(type: "TEXT", nullable: true),
                    Language = table.Column<string>(type: "TEXT", nullable: true),
                    SettingsJson = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FromEmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ToEmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    OneOnOneId = table.Column<int>(type: "INTEGER", nullable: true),
                    Context = table.Column<string>(type: "TEXT", nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    FeedbackType = table.Column<int>(type: "INTEGER", nullable: false),
                    Sentiment = table.Column<int>(type: "INTEGER", nullable: true),
                    CompetencyId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsPrivate = table.Column<bool>(type: "INTEGER", nullable: false),
                    Visibility = table.Column<int>(type: "INTEGER", nullable: false),
                    DeliveredDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Instances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Slug = table.Column<string>(type: "TEXT", nullable: false),
                    McpApiKey = table.Column<string>(type: "TEXT", nullable: false),
                    Plan = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    MaxUsers = table.Column<int>(type: "INTEGER", nullable: false),
                    SettingsJson = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MoodEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    EntryAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Mood = table.Column<int>(type: "INTEGER", nullable: false),
                    EnergyLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    Source = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoodEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PdiActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PdiGoalId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ActionType = table.Column<int>(type: "INTEGER", nullable: false),
                    EstimatedHours = table.Column<decimal>(type: "TEXT", nullable: true),
                    ActualHours = table.Column<decimal>(type: "TEXT", nullable: true),
                    DueDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    CompletedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ResourcesNeeded = table.Column<string>(type: "TEXT", nullable: true),
                    EvidenceUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PdiActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PdiCheckpoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PdiPlanId = table.Column<int>(type: "INTEGER", nullable: false),
                    PdiGoalId = table.Column<int>(type: "INTEGER", nullable: true),
                    CheckpointDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    OverallSentiment = table.Column<int>(type: "INTEGER", nullable: true),
                    ProgressSummary = table.Column<string>(type: "TEXT", nullable: true),
                    Wins = table.Column<string>(type: "TEXT", nullable: true),
                    Challenges = table.Column<string>(type: "TEXT", nullable: true),
                    NextSteps = table.Column<string>(type: "TEXT", nullable: true),
                    CoachNotes = table.Column<string>(type: "TEXT", nullable: true),
                    CompletedById = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PdiCheckpoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PdiCycles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    CoachId = table.Column<int>(type: "INTEGER", nullable: true),
                    ManagerId = table.Column<int>(type: "INTEGER", nullable: true),
                    CycleName = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    OverallProgress = table.Column<decimal>(type: "TEXT", nullable: true),
                    FocusAreasJson = table.Column<string>(type: "TEXT", nullable: true),
                    Context = table.Column<string>(type: "TEXT", nullable: true),
                    KickoffDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    CompletionDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PdiCycles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PdiEvidences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PdiActionId = table.Column<int>(type: "INTEGER", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    FileUrl = table.Column<string>(type: "TEXT", nullable: false),
                    FileType = table.Column<string>(type: "TEXT", nullable: true),
                    ContentHash = table.Column<string>(type: "TEXT", nullable: true),
                    UploadedById = table.Column<int>(type: "INTEGER", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PdiEvidences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PdiGoals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PdiPlanId = table.Column<int>(type: "INTEGER", nullable: false),
                    CompetencyId = table.Column<int>(type: "INTEGER", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    GoalType = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentLevel = table.Column<int>(type: "INTEGER", nullable: true),
                    TargetLevel = table.Column<int>(type: "INTEGER", nullable: true),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Progress = table.Column<decimal>(type: "TEXT", nullable: true),
                    Deadline = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    CompletedDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    SuccessCriteria = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PdiGoals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PdiPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PdiCycleId = table.Column<int>(type: "INTEGER", nullable: false),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    CoachId = table.Column<int>(type: "INTEGER", nullable: true),
                    ManagerId = table.Column<int>(type: "INTEGER", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Progress = table.Column<decimal>(type: "TEXT", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    Context = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PdiPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PerformanceCycles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformanceCycles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PerformanceReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PerformanceCycleId = table.Column<int>(type: "INTEGER", nullable: false),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CalibrationOwnerId = table.Column<int>(type: "INTEGER", nullable: true),
                    ReviewType = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    OverallScore = table.Column<decimal>(type: "TEXT", nullable: true),
                    CalibrationScore = table.Column<decimal>(type: "TEXT", nullable: true),
                    Comments = table.Column<string>(type: "TEXT", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformanceReviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DepartmentId = table.Column<int>(type: "INTEGER", nullable: true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Level = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PulseQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PulseSurveyId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionText = table.Column<string>(type: "TEXT", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    Required = table.Column<bool>(type: "INTEGER", nullable: false),
                    QuestionType = table.Column<int>(type: "INTEGER", nullable: false),
                    OptionsJson = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PulseQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PulseResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PulseSurveyId = table.Column<int>(type: "INTEGER", nullable: false),
                    PulseQuestionId = table.Column<int>(type: "INTEGER", nullable: true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: true),
                    ResponseDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AnswersJson = table.Column<string>(type: "TEXT", nullable: false),
                    OverallSentiment = table.Column<int>(type: "INTEGER", nullable: true),
                    EnergyLevel = table.Column<int>(type: "INTEGER", nullable: true),
                    MetadataJson = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PulseResponses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PulseSurveys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationalUnitId = table.Column<int>(type: "INTEGER", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Frequency = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionsJson = table.Column<string>(type: "TEXT", nullable: false),
                    IsAnonymous = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    CreatedById = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PulseSurveys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBanks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    QuestionBankId = table.Column<int>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    HelpText = table.Column<string>(type: "TEXT", nullable: true),
                    QuestionType = table.Column<int>(type: "INTEGER", nullable: false),
                    AnswerFormat = table.Column<int>(type: "INTEGER", nullable: false),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    MetadataJson = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReviewResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PerformanceReviewId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionId = table.Column<int>(type: "INTEGER", nullable: true),
                    AnswerText = table.Column<string>(type: "TEXT", nullable: true),
                    Score = table.Column<decimal>(type: "TEXT", nullable: true),
                    EvidenceJson = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewResponses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeamMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoleInTeam = table.Column<string>(type: "TEXT", nullable: true),
                    JoinedAt = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    LeftAt = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    IsPrimary = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DepartmentId = table.Column<int>(type: "INTEGER", nullable: true),
                    LeadId = table.Column<int>(type: "INTEGER", nullable: true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AiInteractions");

            migrationBuilder.DropTable(
                name: "AiRecommendations");

            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "BscGoals");

            migrationBuilder.DropTable(
                name: "BscIndicators");

            migrationBuilder.DropTable(
                name: "BscPerspectives");

            migrationBuilder.DropTable(
                name: "ChangeLogs");

            migrationBuilder.DropTable(
                name: "CheckIns");

            migrationBuilder.DropTable(
                name: "Competencies");

            migrationBuilder.DropTable(
                name: "CompetencyLibraries");

            migrationBuilder.DropTable(
                name: "Configurations");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "DiscAssessments");

            migrationBuilder.DropTable(
                name: "DiscResponses");

            migrationBuilder.DropTable(
                name: "DiscResults");

            migrationBuilder.DropTable(
                name: "EcAssessments");

            migrationBuilder.DropTable(
                name: "EcResponses");

            migrationBuilder.DropTable(
                name: "EcResults");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "Instances");

            migrationBuilder.DropTable(
                name: "MoodEntries");

            migrationBuilder.DropTable(
                name: "PdiActions");

            migrationBuilder.DropTable(
                name: "PdiCheckpoints");

            migrationBuilder.DropTable(
                name: "PdiCycles");

            migrationBuilder.DropTable(
                name: "PdiEvidences");

            migrationBuilder.DropTable(
                name: "PdiGoals");

            migrationBuilder.DropTable(
                name: "PdiPlans");

            migrationBuilder.DropTable(
                name: "PerformanceCycles");

            migrationBuilder.DropTable(
                name: "PerformanceReviews");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "PulseQuestions");

            migrationBuilder.DropTable(
                name: "PulseResponses");

            migrationBuilder.DropTable(
                name: "PulseSurveys");

            migrationBuilder.DropTable(
                name: "QuestionBanks");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "ReviewResponses");

            migrationBuilder.DropTable(
                name: "TeamMembers");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
