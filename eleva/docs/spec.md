# Eleva

Data: 2026-04-22

Este documento especifica o **Eleva**, uma plataforma nova de RH construída sobre os padrões do **Escalada Framework**. O objetivo é definir a base técnica do produto para que a implementação possa começar do zero com contratos claros, estrutura previsível e infraestrutura transversal já alinhada ao framework de referência.

O Eleva usa os padrões do Escalada Framework para:

- multi-tenancy com `InstanceId` como fronteira lógica por cliente
- `InstancePO` como entidade raiz de instância
- bootstrap HTTP, middleware, controllers e registro de ferramentas MCP
- status de execução para processos, jobs e chamadas assíncronas
- modelo de services, interfaces e persistência com separação entre `Server`, `Services` e `Shared`

## 1. Visão Geral

Eleva é uma plataforma de RH orientada a operação e inteligência de pessoas. O domínio cobre:

- cadastro e estrutura organizacional
- avaliação comportamental e diagnósticos
- PDI e ciclos de desenvolvimento
- performance, BSC e avaliações
- engagement, feedback, check-ins e pulso
- banco de perguntas e suporte a IA
- auditoria e histórico operacional

O projeto deve ser construído como uma solution nova, com namespaces `Eleva.*`, sem depender de código ou narrativa de projetos anteriores. O que orienta a implementação é o contrato do Escalada Framework.

## 2. Referência Arquitetural

O Escalada Framework define os padrões que o Eleva segue na implementação base:

1. multi-tenancy por `InstanceId`
2. contexto por request via `InstanceContext`
3. acesso ao tenant atual via `ICurrentInstanceAccessor`
4. descoberta e execução de tools MCP via `McpServiceRegistry`
5. contrato de tool via `McpFunction`, `McpArgs`, `McpParameter` e `McpSchemaValidator`
6. middleware para resolução do contexto de instância
7. controle de `ExecutionStatus` para operações rastreáveis
8. services transversais reutilizáveis para configuração, memória e auditoria

O Eleva não precisa conhecer o histórico de nenhum produto anterior. Ele apenas implementa esses padrões para o domínio de RH.

## 3. Estrutura da Solution

### 3.1 Layout esperado

```text
Eleva.sln
src/
  Eleva.Server/          (ASP.NET Core Web API, MCP, controllers, middleware, jobs)
  Eleva.Services/        (orquestração de domínio, DbContext, persistência, serviços)
  Eleva.Shared/          (interfaces base, enums, persistence objects)
tests/
  Eleva.UnitTests/
  Eleva.IntegrationTests/
skills/
bridge/
config/
database/
scripts/
docs/
```

### 3.2 Camadas

- `Eleva.Shared` concentra contratos compartilhados, enums e POs.
- `Eleva.Services` concentra regras de negócio, consultas, escrita e `AppDbContext`.
- `Eleva.Server` expõe HTTP, MCP, middleware, bootstrap e jobs.

### 3.3 Convenções

- todos os namespaces usam `Eleva.*`
- toda entidade mutável herda `BaseEntity`
- toda entidade multi-tenant carrega `InstanceId`
- o modelo de execução e observabilidade segue o padrão do Escalada Framework

## 4. Infraestrutura Base

### 4.1 Multi-tenancy

O tenant lógico do Eleva é uma `Instance`. Toda operação deve executar dentro do contexto de uma instância válida.

- `InstanceId` é a chave de isolamento
- `InstancePO` representa a instância do cliente
- o contexto atual é resolvido por `InstanceContext`
- o acesso a `InstanceId` e `UserId` é exposto por `ICurrentInstanceAccessor`

### 4.2 MCP

O Eleva expõe suas capacidades administrativas e operacionais por MCP, seguindo o contrato do Escalada Framework.

- tools são registradas no `McpServiceRegistry`
- cada tool é descrita por `McpFunction`
- parâmetros usam `McpParameter`
- validação de payload é feita por `McpSchemaValidator`
- `ToolAnnotation` classifica a tool por risco e efeito
- `McpController` concentra listagem, chamada, validação e auditoria

### 4.3 Middleware

`InstanceContextMiddleware` resolve os headers do request e popula o contexto de execução.

- `X-Instance-Id`
- `X-User-Id`

### 4.4 Execution Status

O Eleva mantém a infraestrutura de status de execução para registrar operações relevantes.

- `IExecutionStatusService`
- `ExecutionStatusService`
- `IExecutionStatusStore`
- `RedisExecutionStatusStore`

Esse mecanismo cobre jobs, chamadas MCP e operações que precisem de rastreio operacional.

### 4.5 Services transversais

O projeto preserva os serviços transversais esperados pelo framework:

- `ConfigService`
- `MemoryService`
- `AuditLogService`
- `TaskService`

## 5. Domínio de RH

### 5.1 Enums

Os enums do Eleva devem refletir o domínio funcional e permanecer estáveis para persistência e APIs. Os contratos esperados são:

- identidade e acesso: `UserRole`, `EmploymentStatus`, `OrganizationalUnitType`, `SubscriptionPlan`
- diagnósticos e avaliação: `DiscProfile`, `AssessmentStatus`, `EcLevel`, `AssessmentType`, `SentimentLevel`
- PDI: `PdiStatus`, `PdiActionType`, `PdiGoalType`, `PdiPriority`, `PdiGoalStatus`, `PdiActionStatus`, `PdiCheckpointType`, `PdiCheckpointStatus`, `CycleStatus`
- performance: `ReviewStatus`, `BscStatus`, `IndicatorStatus`, `LevelType`
- engagement: `PulseType`, `PulseFrequency`, `SurveyStatus`, `FeedbackVisibility`, `FeedbackRecordStatus`, `OneOnOneStatus`
- perguntas e IA: `QuestionType`, `AnswerFormat`, `AiContextType`, `AiRecommendationType`, `AiRecommendationStatus`
- auditoria e rastreio: `AuditStatus`, `ChangeSource`

### 5.2 Persistence Objects

Os POs do Eleva ficam em `Eleva.Shared/PersistenceObjects/` e todos herdam `BaseEntity`.

- `Core/`: `InstancePO`, `UserPO`, `ConfigurationPO`
- `People/`: `EmployeePO`, `DepartmentPO`, `PositionPO`, `TeamPO`, `TeamMemberPO`
- `Assessments/`: `CompetencyPO`, `CompetencyLibraryPO`, `DiscAssessmentPO`, `DiscResponsePO`, `DiscResultPO`, `EcAssessmentPO`, `EcResponsePO`, `EcResultPO`
- `Pdi/`: `PdiCyclePO`, `PdiPlanPO`, `PdiGoalPO`, `PdiActionPO`, `PdiCheckpointPO`, `PdiEvidencePO`
- `Performance/`: `PerformanceCyclePO`, `BscPerspectivePO`, `BscGoalPO`, `BscIndicatorPO`, `PerformanceReviewPO`, `ReviewResponsePO`
- `Engagement/`: `PulseSurveyPO`, `PulseQuestionPO`, `PulseResponsePO`, `FeedbackPO`, `CheckInPO`, `MoodEntryPO`
- `Questions/`: `QuestionBankPO`, `QuestionPO`, `AnswerPO`
- `AICopilot/`: `AiRecommendationPO`, `AiInteractionPO`
- `History/`: `AuditLogPO`, `ChangeLogPO`

### 5.3 DbSets

O `AppDbContext` deve expor os seguintes conjuntos:

- `Instances`, `Users`, `Configurations`
- `Employees`, `Departments`, `Positions`, `Teams`, `TeamMembers`
- `Competencies`, `CompetencyLibraries`, `DiscAssessments`, `DiscResponses`, `DiscResults`, `EcAssessments`, `EcResponses`, `EcResults`
- `PdiCycles`, `PdiPlans`, `PdiGoals`, `PdiActions`, `PdiCheckpoints`, `PdiEvidences`
- `PerformanceCycles`, `BscPerspectives`, `BscGoals`, `BscIndicators`, `PerformanceReviews`, `ReviewResponses`
- `PulseSurveys`, `PulseQuestions`, `PulseResponses`, `Feedbacks`, `CheckIns`, `MoodEntries`
- `QuestionBanks`, `Questions`, `Answers`
- `AiRecommendations`, `AiInteractions`
- `AuditLogs`, `ChangeLogs`

### 5.4 Services

As interfaces e implementações esperadas em `Eleva.Services/Services/` são:

- `Services/People/`: `IEmployeeService`, `IDepartmentService`
- `Services/Assessments/`: `IDiscService`, `IEcService`, `ICompetencyService`
- `Services/Pdi/`: `IPdiService`, `IPdiCycleService`
- `Services/Performance/`: `IBscService`, `IReviewService`
- `Services/Engagement/`: `IPulseService`, `IFeedbackService`, `ICheckInService`
- `Services/Questions/`: `IQuestionBankService`
- `Services/AICopilot/`: `IAiRecommendationService`
- `Services/Core/`: `IConfigService`
- `Services/History/`: `IAuditLogService`
- `Services/ExecutionStatus/`: `IExecutionStatusService`, `IExecutionStatusStore`

### 5.5 MCP Services

Os MCP services do Eleva cobrem os fluxos principais do domínio:

- `PeopleMcpService`
- `DiscMcpService`
- `EcMcpService`
- `PdiMcpService`
- `PerformanceMcpService`
- `EngagementMcpService`
- `AiCopilotMcpService`
- `QuestionsMcpService`
- `AuthMcpService`
- `ConfigMcpService`
- `SystemMcpService`
- `AuditMcpService`

### 5.6 Jobs

Os jobs Quartz do Eleva tratam rotinas de apoio ao domínio:

- `PdiNotificationJob`
- `AssessmentCycleJob`
- `PulseSurveyJob`
- `PerformanceCycleJob`
- `AiRecommendationJob`
- `AuditConsolidationJob`

### 5.7 Skills

As skills do Eleva ficam em `skills/`:

- `eleva-implementation-rules`
- `eleva-mcp-create-service`
- `eleva-database-migrations`
- `eleva-run-tests`

## 6. Program.cs e Bootstrap

O bootstrap deve registrar a infraestrutura base do Eleva de forma explícita e previsível.

### 6.1 Responsabilidades

- configurar `AppDbContext`
- registrar `InstanceContext` e `ScopedInstanceAccessor`
- registrar os services de domínio
- registrar `McpServiceRegistry`
- registrar os MCP services
- registrar `ExecutionStatus`
- configurar controllers, JSON e middlewares
- configurar Quartz para jobs de RH

### 6.2 Ambiente e banco

- SQLite pode ser usado no desenvolvimento e testes
- MySQL/MariaDB pode ser usado em produção
- o bootstrap deve aplicar migrations no startup quando o ambiente exigir

### 6.3 Ordem de bootstrap

1. criar a solution e os projetos
2. registrar os project references
3. configurar o banco de dados
4. registrar o contexto de instância
5. registrar os services de domínio
6. registrar a infraestrutura MCP
7. registrar Quartz e jobs
8. expor controllers e health check

## 7. Regras de Modelagem e Persistência

- toda entidade mutável recebe `InstanceId`
- `InstanceId` deve ser preenchido automaticamente na persistência
- escritas fora do contexto atual devem falhar
- consultas devem respeitar o boundary de instância
- enums devem persistir como string
- entidades com histórico devem suportar `DeletedAt` quando aplicável
- logs e auditoria devem ser registrados para operações sensíveis
- o `AppDbContext` deve preservar filtros globais e consistência de tenant

## 8. Checklist de Validação

- `dotnet build` sem erros
- `dotnet test` passando
- `GET /health` retornando OK
- `GET /mcp/tools` listando apenas tools do domínio de RH
- `POST /mcp/tools/call` executando `employee_list`
- migrations aplicadas no startup
- `InstanceId` aplicado e validado em escrita e leitura
- services de RH resolvidos por DI
- jobs Quartz registrados

Os apêndices abaixo mantêm os contratos técnicos completos do Eleva. O código C# permanece como referência de implementação e deve ser preservado integralmente.

---

## 8. Apendice A - contratos completos

As secoes abaixo preservam os contratos de `Eleva.Shared`, `Eleva.Services`, `Eleva.Server` e dos MCP services. Elas sao a referencia de codigo C# a ser implementada na construcao do projeto.

---

### 8.1 Eleva.Shared

### 2.1 Interfaces base

```csharp
namespace Eleva.Shared.Interfaces;

public interface IInstanceBaseEntity
{
    int Id { get; set; }
    int InstanceId { get; set; }
    string? ReferenceCode { get; set; }
}
```

```csharp
namespace Eleva.Shared.Interfaces;

public interface ICurrentInstanceAccessor
{
    int? GetInstanceId();
    int? GetUserId();
}
```

```csharp
namespace Eleva.Shared.Interfaces;

public abstract class BaseEntity : IInstanceBaseEntity
{
    public int Id { get; set; }
    public int InstanceId { get; set; }
    public string? ReferenceCode { get; set; }
}
```

### 2.2 Enums

```csharp
namespace Eleva.Shared.Enums;

public enum UserRole
{
    Admin,
    BP,
    Coach,
    Leader,
    Employee
}

public enum DiscProfile
{
    D,
    I,
    S,
    C
}

public enum EcLevel
{
    Level1,
    Level2,
    Level3,
    Level4,
    Level5,
    Level6,
    Level7
}

public enum PdiStatus
{
    Draft,
    Active,
    InProgress,
    Completed,
    Cancelled
}

public enum PdiActionType
{
    Learning70,
    Social20,
    Formal10
}

public enum AssessmentStatus
{
    Pending,
    InProgress,
    Completed,
    Expired
}

public enum ReviewStatus
{
    Draft,
    InProgress,
    Submitted,
    Calibrated,
    Closed
}

public enum FeedbackType
{
    Positive,
    Constructive,
    Recognition
}

public enum SurveyStatus
{
    Draft,
    Active,
    Closed
}

public enum CompetencyCategory
{
    Technical,
    Behavioral,
    Leadership
}

public enum CycleStatus
{
    Planning,
    Active,
    Closing,
    Closed
}

public enum InstanceStatus
{
    Active,
    Suspended,
    Inactive
}

public enum SubscriptionPlan
{
    Free,
    Basic,
    Professional,
    Enterprise
}

public enum EmploymentStatus
{
    Active,
    OnLeave,
    Terminated
}

public enum OrganizationalUnitType
{
    Department,
    Team,
    Area,
    Division
}

public enum AssessmentType
{
    CollectiveListening,
    ClimateSurvey,
    Maturity
}

public enum PulseType
{
    Mood,
    Energy,
    Engagement,
    Risk
}

public enum PulseFrequency
{
    Daily,
    Weekly,
    Biweekly,
    Monthly
}

public enum SentimentLevel
{
    VeryNegative,
    Negative,
    Neutral,
    Positive,
    VeryPositive
}

public enum LevelType
{
    Numeric,
    Descriptive,
    Boolean
}

public enum BscStatus
{
    Planning,
    Active,
    OnHold,
    Completed,
    Cancelled
}

public enum IndicatorStatus
{
    OnTrack,
    AtRisk,
    OffTrack,
    Achieved
}

public enum PdiGoalType
{
    Competency,
    Behavior,
    Project,
    Learning
}

public enum PdiPriority
{
    High,
    Medium,
    Low
}

public enum PdiGoalStatus
{
    NotStarted,
    InProgress,
    Blocked,
    Completed,
    Cancelled
}

public enum PdiActionStatus
{
    Pending,
    InProgress,
    Completed,
    Cancelled
}

public enum PdiCheckpointType
{
    Weekly,
    Biweekly,
    Monthly,
    Milestone
}

public enum PdiCheckpointStatus
{
    Scheduled,
    Completed,
    Cancelled
}

public enum OneOnOneStatus
{
    Scheduled,
    Completed,
    Cancelled,
    Rescheduled
}

public enum FeedbackVisibility
{
    Private,
    ManagerOnly,
    PersonAndManager,
    Public
}

public enum FeedbackRecordStatus
{
    Draft,
    Delivered,
    Acknowledged,
    Archived
}

public enum AiContextType
{
    Pdi,
    OneOnOne,
    Feedback,
    Pulse,
    GapAnalysis
}

public enum AiRecommendationType
{
    ConversationTopic,
    ActionItem,
    Alert,
    LearningPath
}

public enum AiRecommendationStatus
{
    Pending,
    Accepted,
    Rejected,
    Implemented,
    Dismissed
}

public enum AuditStatus
{
    Success,
    Failure,
    Partial
}

public enum QuestionType
{
    OpenText,
    SingleChoice,
    MultiChoice,
    Scale,
    Boolean
}

public enum AnswerFormat
{
    Text,
    Number,
    Boolean,
    Json
}

public enum ChangeSource
{
    Api,
    Mpc,
    Job,
    System,
    Import
}
```

### 2.3 PersistenceObjects

Todos os POs herdam `BaseEntity`. Campos de auditoria comuns aparecem nos objetos que persistem eventos de negocio.

#### Core

```csharp
namespace Eleva.Shared.PersistenceObjects.Core;

using Eleva.Shared.Interfaces;
using Eleva.Shared.Enums;

public class InstancePO : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string McpApiKey { get; set; } = null!;
    public SubscriptionPlan Plan { get; set; }
    public InstanceStatus Status { get; set; }
    public bool IsActive { get; set; }
    public int MaxUsers { get; set; }
    public string? SettingsJson { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class UserPO : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public class ConfigurationPO : BaseEntity
{
    public string Key { get; set; } = null!;
    public string? Value { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
    public bool IsSecret { get; set; }
    public bool IsEncrypted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

#### People

```csharp
namespace Eleva.Shared.PersistenceObjects.People;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class EmployeePO : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? DocumentNumber { get; set; }
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public int? UserId { get; set; }
    public int? DepartmentId { get; set; }
    public int? PositionId { get; set; }
    public int? ManagerId { get; set; }
    public DateOnly? HireDate { get; set; }
    public DateOnly? BirthDate { get; set; }
    public EmploymentStatus Status { get; set; }
    public string? PositionTitle { get; set; }
    public string? Timezone { get; set; }
    public string? Language { get; set; }
    public string? SettingsJson { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class DepartmentPO : BaseEntity
{
    public int? ParentDepartmentId { get; set; }
    public int? ManagerId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public OrganizationalUnitType Type { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class PositionPO : BaseEntity
{
    public int? DepartmentId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Level { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class TeamPO : BaseEntity
{
    public int? DepartmentId { get; set; }
    public int? LeadId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class TeamMemberPO : BaseEntity
{
    public int TeamId { get; set; }
    public int EmployeeId { get; set; }
    public string? RoleInTeam { get; set; }
    public DateOnly JoinedAt { get; set; }
    public DateOnly? LeftAt { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

#### Assessments

```csharp
namespace Eleva.Shared.PersistenceObjects.Assessments;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class CompetencyPO : BaseEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public CompetencyCategory Category { get; set; }
    public LevelType LevelType { get; set; }
    public string? LevelDefinitionsJson { get; set; }
    public bool IsCore { get; set; }
    public int? ParentCompetencyId { get; set; }
    public int? SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class CompetencyLibraryPO : BaseEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Version { get; set; } = "1.0";
    public string? Source { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class DiscAssessmentPO : BaseEntity
{
    public int EmployeeId { get; set; }
    public int? AssessedById { get; set; }
    public AssessmentStatus Status { get; set; }
    public DiscProfile? PrimaryStyle { get; set; }
    public DiscProfile? SecondaryStyle { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? AssessmentLink { get; set; }
    public string? ReportUrl { get; set; }
    public string? RawDataJson { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class DiscResponsePO : BaseEntity
{
    public int DiscAssessmentId { get; set; }
    public string QuestionCode { get; set; } = null!;
    public string? AnswerValue { get; set; }
    public string? Axis { get; set; }
    public DateTime AnsweredAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class DiscResultPO : BaseEntity
{
    public int DiscAssessmentId { get; set; }
    public decimal DScore { get; set; }
    public decimal IScore { get; set; }
    public decimal SScore { get; set; }
    public decimal CScore { get; set; }
    public DiscProfile PrimaryStyle { get; set; }
    public DiscProfile? SecondaryStyle { get; set; }
    public string ProfileCombination { get; set; } = null!;
    public string? Interpretation { get; set; }
    public string? StrengthsJson { get; set; }
    public string? DevelopmentAreasJson { get; set; }
    public string? CommunicationTipsJson { get; set; }
    public string? StressIndicatorsJson { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class EcAssessmentPO : BaseEntity
{
    public int? OrganizationalUnitId { get; set; }
    public AssessmentType AssessmentType { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public AssessmentStatus Status { get; set; }
    public EcLevel? ConsciousnessLevel { get; set; }
    public decimal? ParticipationRate { get; set; }
    public decimal? OverallScore { get; set; }
    public string? DimensionsJson { get; set; }
    public string? Findings { get; set; }
    public string? Recommendations { get; set; }
    public int? ConductedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class EcResponsePO : BaseEntity
{
    public int EcAssessmentId { get; set; }
    public int? EmployeeId { get; set; }
    public int? QuestionId { get; set; }
    public DateTime ResponseDate { get; set; }
    public string AnswersJson { get; set; } = null!;
    public SentimentLevel? Sentiment { get; set; }
    public int? EnergyLevel { get; set; }
    public string? MetadataJson { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class EcResultPO : BaseEntity
{
    public int EcAssessmentId { get; set; }
    public EcLevel? MaturityLevel { get; set; }
    public decimal? ParticipationRate { get; set; }
    public decimal? OverallScore { get; set; }
    public string? StrengthsJson { get; set; }
    public string? RisksJson { get; set; }
    public string? RecommendationsJson { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

#### PDI

```csharp
namespace Eleva.Shared.PersistenceObjects.Pdi;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class PdiCyclePO : BaseEntity
{
    public int EmployeeId { get; set; }
    public int? CoachId { get; set; }
    public int? ManagerId { get; set; }
    public string CycleName { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public CycleStatus Status { get; set; }
    public decimal? OverallProgress { get; set; }
    public string? FocusAreasJson { get; set; }
    public string? Context { get; set; }
    public DateOnly? KickoffDate { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class PdiPlanPO : BaseEntity
{
    public int PdiCycleId { get; set; }
    public int EmployeeId { get; set; }
    public int? CoachId { get; set; }
    public int? ManagerId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public PdiStatus Status { get; set; }
    public decimal? Progress { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Context { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class PdiGoalPO : BaseEntity
{
    public int PdiPlanId { get; set; }
    public int? CompetencyId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public PdiGoalType GoalType { get; set; }
    public int? CurrentLevel { get; set; }
    public int? TargetLevel { get; set; }
    public PdiPriority Priority { get; set; }
    public PdiGoalStatus Status { get; set; }
    public decimal? Progress { get; set; }
    public DateOnly? Deadline { get; set; }
    public DateOnly? CompletedDate { get; set; }
    public string? SuccessCriteria { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class PdiActionPO : BaseEntity
{
    public int PdiGoalId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public PdiActionType ActionType { get; set; }
    public decimal? EstimatedHours { get; set; }
    public decimal? ActualHours { get; set; }
    public DateOnly? DueDate { get; set; }
    public DateOnly? CompletedDate { get; set; }
    public PdiActionStatus Status { get; set; }
    public string? ResourcesNeeded { get; set; }
    public string? EvidenceUrl { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class PdiCheckpointPO : BaseEntity
{
    public int PdiPlanId { get; set; }
    public int? PdiGoalId { get; set; }
    public DateOnly CheckpointDate { get; set; }
    public PdiCheckpointType Type { get; set; }
    public PdiCheckpointStatus Status { get; set; }
    public SentimentLevel? OverallSentiment { get; set; }
    public string? ProgressSummary { get; set; }
    public string? Wins { get; set; }
    public string? Challenges { get; set; }
    public string? NextSteps { get; set; }
    public string? CoachNotes { get; set; }
    public int? CompletedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class PdiEvidencePO : BaseEntity
{
    public int PdiActionId { get; set; }
    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public string? FileType { get; set; }
    public string? ContentHash { get; set; }
    public int? UploadedById { get; set; }
    public DateTime UploadedAt { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

#### Performance

```csharp
namespace Eleva.Shared.PersistenceObjects.Performance;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class PerformanceCyclePO : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public CycleStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class BscPerspectivePO : BaseEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class BscGoalPO : BaseEntity
{
    public int PerformanceCycleId { get; set; }
    public int PerspectiveId { get; set; }
    public int? DepartmentId { get; set; }
    public int? OwnerId { get; set; }
    public int? CascadedFromGoalId { get; set; }
    public string Code { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public BscStatus Status { get; set; }
    public decimal? Weight { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal? Progress { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class BscIndicatorPO : BaseEntity
{
    public int BscGoalId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Formula { get; set; }
    public string? UnitOfMeasure { get; set; }
    public decimal? BaselineValue { get; set; }
    public decimal TargetValue { get; set; }
    public decimal? CurrentValue { get; set; }
    public string? MeasurementFrequency { get; set; }
    public string? DataSource { get; set; }
    public bool IsHigherBetter { get; set; }
    public IndicatorStatus Status { get; set; }
    public DateOnly? LastMeasuredDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class PerformanceReviewPO : BaseEntity
{
    public int PerformanceCycleId { get; set; }
    public int EmployeeId { get; set; }
    public int ReviewerId { get; set; }
    public int? CalibrationOwnerId { get; set; }
    public string? ReviewType { get; set; }
    public ReviewStatus Status { get; set; }
    public decimal? OverallScore { get; set; }
    public decimal? CalibrationScore { get; set; }
    public string? Comments { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class ReviewResponsePO : BaseEntity
{
    public int PerformanceReviewId { get; set; }
    public int? QuestionId { get; set; }
    public string? AnswerText { get; set; }
    public decimal? Score { get; set; }
    public string? EvidenceJson { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

#### Engagement

```csharp
namespace Eleva.Shared.PersistenceObjects.Engagement;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class PulseSurveyPO : BaseEntity
{
    public int? OrganizationalUnitId { get; set; }
    public string Title { get; set; } = null!;
    public PulseType Type { get; set; }
    public PulseFrequency Frequency { get; set; }
    public string QuestionsJson { get; set; } = null!;
    public bool IsAnonymous { get; set; }
    public SurveyStatus Status { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class PulseQuestionPO : BaseEntity
{
    public int PulseSurveyId { get; set; }
    public string QuestionText { get; set; } = null!;
    public int SortOrder { get; set; }
    public bool Required { get; set; }
    public QuestionType QuestionType { get; set; }
    public string? OptionsJson { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class PulseResponsePO : BaseEntity
{
    public int PulseSurveyId { get; set; }
    public int? PulseQuestionId { get; set; }
    public int? EmployeeId { get; set; }
    public DateTime ResponseDate { get; set; }
    public string AnswersJson { get; set; } = null!;
    public SentimentLevel? OverallSentiment { get; set; }
    public int? EnergyLevel { get; set; }
    public string? MetadataJson { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class FeedbackPO : BaseEntity
{
    public int FromEmployeeId { get; set; }
    public int ToEmployeeId { get; set; }
    public int? OneOnOneId { get; set; }
    public string? Context { get; set; }
    public string Content { get; set; } = null!;
    public FeedbackType FeedbackType { get; set; }
    public SentimentLevel? Sentiment { get; set; }
    public int? CompetencyId { get; set; }
    public bool IsPrivate { get; set; }
    public FeedbackVisibility Visibility { get; set; }
    public DateTime? DeliveredDate { get; set; }
    public FeedbackRecordStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class CheckInPO : BaseEntity
{
    public int EmployeeId { get; set; }
    public int? ManagerId { get; set; }
    public DateTime Date { get; set; }
    public SentimentLevel Mood { get; set; }
    public int EnergyLevel { get; set; }
    public int ProductivityScore { get; set; }
    public string? Notes { get; set; }
    public string? Blockers { get; set; }
    public bool SupportNeeded { get; set; }
    public OneOnOneStatus? Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class MoodEntryPO : BaseEntity
{
    public int EmployeeId { get; set; }
    public DateTime EntryAt { get; set; }
    public SentimentLevel Mood { get; set; }
    public int EnergyLevel { get; set; }
    public string? Notes { get; set; }
    public string? Source { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

#### Questions

```csharp
namespace Eleva.Shared.PersistenceObjects.Questions;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class QuestionBankPO : BaseEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Version { get; set; } = "1.0";
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class QuestionPO : BaseEntity
{
    public int QuestionBankId { get; set; }
    public string Code { get; set; } = null!;
    public string Text { get; set; } = null!;
    public string? HelpText { get; set; }
    public QuestionType QuestionType { get; set; }
    public AnswerFormat AnswerFormat { get; set; }
    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }
    public string? MetadataJson { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class AnswerPO : BaseEntity
{
    public int QuestionId { get; set; }
    public int? EmployeeId { get; set; }
    public string? TextValue { get; set; }
    public decimal? NumericValue { get; set; }
    public bool? BooleanValue { get; set; }
    public string? JsonValue { get; set; }
    public decimal? Score { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

#### AICopilot

```csharp
namespace Eleva.Shared.PersistenceObjects.AICopilot;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class AiRecommendationPO : BaseEntity
{
    public int? EmployeeId { get; set; }
    public AiContextType ContextType { get; set; }
    public int? ContextId { get; set; }
    public AiRecommendationType RecommendationType { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? Rationale { get; set; }
    public string? SuggestedActionsJson { get; set; }
    public string? ModelUsed { get; set; }
    public decimal? ConfidenceScore { get; set; }
    public AiRecommendationStatus Status { get; set; }
    public int? ReviewedById { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class AiInteractionPO : BaseEntity
{
    public int? EmployeeId { get; set; }
    public AiContextType ContextType { get; set; }
    public int? ContextId { get; set; }
    public string Prompt { get; set; } = null!;
    public string? PromptHash { get; set; }
    public string? ResponseSummary { get; set; }
    public string? ModelUsed { get; set; }
    public int? TokensIn { get; set; }
    public int? TokensOut { get; set; }
    public decimal? ConfidenceScore { get; set; }
    public int? CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

#### History

```csharp
namespace Eleva.Shared.PersistenceObjects.History;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class AuditLogPO : BaseEntity
{
    public int? UserId { get; set; }
    public string Action { get; set; } = null!;
    public string EntityType { get; set; } = null!;
    public string EntityId { get; set; } = null!;
    public string? ChangesJson { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public AuditStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public string? MetadataJson { get; set; }
    public DateTime Timestamp { get; set; }
}

public class ChangeLogPO : BaseEntity
{
    public string EntityType { get; set; } = null!;
    public string EntityId { get; set; } = null!;
    public string Action { get; set; } = null!;
    public string? BeforeJson { get; set; }
    public string? AfterJson { get; set; }
    public int? ChangedById { get; set; }
    public DateTime ChangedAt { get; set; }
    public ChangeSource Source { get; set; }
    public string? CorrelationId { get; set; }
}
```

---

## 9. Apendice B - Eleva.Services

O `Eleva.Services` concentra orquestracao de negocio e persistencia com `AppDbContext`.

### 3.1 AppDbContext

```csharp
namespace Eleva.Services.Data;

using Eleva.Shared.PersistenceObjects.Core;
using Eleva.Shared.PersistenceObjects.People;
using Eleva.Shared.PersistenceObjects.Assessments;
using Eleva.Shared.PersistenceObjects.Pdi;
using Eleva.Shared.PersistenceObjects.Performance;
using Eleva.Shared.PersistenceObjects.Engagement;
using Eleva.Shared.PersistenceObjects.Questions;
using Eleva.Shared.PersistenceObjects.AICopilot;
using Eleva.Shared.PersistenceObjects.History;
using Eleva.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    private readonly ICurrentInstanceAccessor? _instanceAccessor;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentInstanceAccessor? instanceAccessor = null) : base(options)
    {
        _instanceAccessor = instanceAccessor;
    }

    public DbSet<InstancePO> Instances => Set<InstancePO>();
    public DbSet<UserPO> Users => Set<UserPO>();
    public DbSet<ConfigurationPO> Configurations => Set<ConfigurationPO>();

    public DbSet<EmployeePO> Employees => Set<EmployeePO>();
    public DbSet<DepartmentPO> Departments => Set<DepartmentPO>();
    public DbSet<PositionPO> Positions => Set<PositionPO>();
    public DbSet<TeamPO> Teams => Set<TeamPO>();
    public DbSet<TeamMemberPO> TeamMembers => Set<TeamMemberPO>();

    public DbSet<CompetencyPO> Competencies => Set<CompetencyPO>();
    public DbSet<CompetencyLibraryPO> CompetencyLibraries => Set<CompetencyLibraryPO>();
    public DbSet<DiscAssessmentPO> DiscAssessments => Set<DiscAssessmentPO>();
    public DbSet<DiscResponsePO> DiscResponses => Set<DiscResponsePO>();
    public DbSet<DiscResultPO> DiscResults => Set<DiscResultPO>();
    public DbSet<EcAssessmentPO> EcAssessments => Set<EcAssessmentPO>();
    public DbSet<EcResponsePO> EcResponses => Set<EcResponsePO>();
    public DbSet<EcResultPO> EcResults => Set<EcResultPO>();

    public DbSet<PdiCyclePO> PdiCycles => Set<PdiCyclePO>();
    public DbSet<PdiPlanPO> PdiPlans => Set<PdiPlanPO>();
    public DbSet<PdiGoalPO> PdiGoals => Set<PdiGoalPO>();
    public DbSet<PdiActionPO> PdiActions => Set<PdiActionPO>();
    public DbSet<PdiCheckpointPO> PdiCheckpoints => Set<PdiCheckpointPO>();
    public DbSet<PdiEvidencePO> PdiEvidences => Set<PdiEvidencePO>();

    public DbSet<PerformanceCyclePO> PerformanceCycles => Set<PerformanceCyclePO>();
    public DbSet<BscPerspectivePO> BscPerspectives => Set<BscPerspectivePO>();
    public DbSet<BscGoalPO> BscGoals => Set<BscGoalPO>();
    public DbSet<BscIndicatorPO> BscIndicators => Set<BscIndicatorPO>();
    public DbSet<PerformanceReviewPO> PerformanceReviews => Set<PerformanceReviewPO>();
    public DbSet<ReviewResponsePO> ReviewResponses => Set<ReviewResponsePO>();

    public DbSet<PulseSurveyPO> PulseSurveys => Set<PulseSurveyPO>();
    public DbSet<PulseQuestionPO> PulseQuestions => Set<PulseQuestionPO>();
    public DbSet<PulseResponsePO> PulseResponses => Set<PulseResponsePO>();
    public DbSet<FeedbackPO> Feedbacks => Set<FeedbackPO>();
    public DbSet<CheckInPO> CheckIns => Set<CheckInPO>();
    public DbSet<MoodEntryPO> MoodEntries => Set<MoodEntryPO>();

    public DbSet<QuestionBankPO> QuestionBanks => Set<QuestionBankPO>();
    public DbSet<QuestionPO> Questions => Set<QuestionPO>();
    public DbSet<AnswerPO> Answers => Set<AnswerPO>();

    public DbSet<AiRecommendationPO> AiRecommendations => Set<AiRecommendationPO>();
    public DbSet<AiInteractionPO> AiInteractions => Set<AiInteractionPO>();

    public DbSet<AuditLogPO> AuditLogs => Set<AuditLogPO>();
    public DbSet<ChangeLogPO> ChangeLogs => Set<ChangeLogPO>();
}
```

### 3.2 Interfaces de servico

As interfaces abaixo sao a superficie funcional esperada do dominio. Os nomes dos metodos refletem os fluxos expostos por REST e MCP.

```csharp
namespace Eleva.Services.Services.People;

using Eleva.Shared.PersistenceObjects.People;

public interface IEmployeeService
{
    Task<IReadOnlyList<EmployeePO>> ListAsync(int instanceId, string? search = null, int? departmentId = null, int? managerId = null, bool? isActive = null, int skip = 0, int take = 50);
    Task<EmployeePO?> GetAsync(int instanceId, int employeeId);
    Task<EmployeePO> CreateAsync(int instanceId, EmployeePO employee);
    Task<EmployeePO> UpdateAsync(int instanceId, EmployeePO employee);
    Task<IReadOnlyList<EmployeePO>> GetTeamAsync(int instanceId, int managerId);
    Task<IReadOnlyList<EmployeePO>> GetHierarchyAsync(int instanceId, int employeeId);
}

public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentPO>> ListAsync(int instanceId, string? search = null, bool? isActive = null);
    Task<DepartmentPO?> GetAsync(int instanceId, int departmentId);
    Task<DepartmentPO> CreateAsync(int instanceId, DepartmentPO department);
    Task<DepartmentPO> UpdateAsync(int instanceId, DepartmentPO department);
}

public interface IDiscService
{
    Task<DiscAssessmentPO> CreateAssessmentAsync(int instanceId, int employeeId, string assessmentType);
    Task<DiscAssessmentPO> SubmitAssessmentAsync(int instanceId, int assessmentId, DiscResponsePO[] responses);
    Task<DiscResultPO?> GetResultAsync(int instanceId, int assessmentId);
    Task<object> GetTeamMapAsync(int instanceId, int managerId);
    Task<object> MatchAsync(int instanceId, int employeeId, int targetEmployeeId);
}

public interface IEcService
{
    Task<EcAssessmentPO> CreateAssessmentAsync(int instanceId, EcAssessmentPO assessment);
    Task<EcAssessmentPO> SubmitAssessmentAsync(int instanceId, int assessmentId, EcResponsePO[] responses);
    Task<EcResultPO?> GetResultAsync(int instanceId, int assessmentId);
    Task<object> GetOrgMaturityAsync(int instanceId, int? departmentId = null);
}

public interface ICompetencyService
{
    Task<IReadOnlyList<CompetencyPO>> ListAsync(int instanceId, CompetencyCategory? category = null, string? search = null);
    Task<CompetencyPO?> GetAsync(int instanceId, int competencyId);
    Task<CompetencyPO> CreateAsync(int instanceId, CompetencyPO competency);
    Task<CompetencyPO> UpdateAsync(int instanceId, CompetencyPO competency);
    Task<IReadOnlyList<CompetencyLibraryPO>> ListLibrariesAsync(int instanceId);
}

public interface IPdiService
{
    Task<PdiPlanPO> CreatePlanAsync(int instanceId, PdiPlanPO plan, IEnumerable<PdiGoalPO>? goals = null);
    Task<PdiPlanPO?> GetAsync(int instanceId, int pdiPlanId);
    Task<IReadOnlyList<PdiPlanPO>> ListAsync(int instanceId, int? employeeId = null, PdiStatus? status = null, string? cycle = null);
    Task<PdiGoalPO> CreateGoalAsync(int instanceId, int pdiPlanId, PdiGoalPO goal);
    Task<PdiActionPO> CreateActionAsync(int instanceId, int pdiGoalId, PdiActionPO action);
    Task<PdiCheckpointPO> CreateCheckpointAsync(int instanceId, int pdiPlanId, int pdiGoalId, PdiCheckpointPO checkpoint);
    Task<PdiEvidencePO> UploadEvidenceAsync(int instanceId, int pdiActionId, PdiEvidencePO evidence);
    Task<object> GetProgressSummaryAsync(int instanceId, int pdiPlanId);
}

public interface IPdiCycleService
{
    Task<PdiCyclePO> CreateAsync(int instanceId, PdiCyclePO cycle);
    Task<IReadOnlyList<PdiCyclePO>> ListAsync(int instanceId, int? employeeId = null, CycleStatus? status = null);
    Task<PdiCyclePO?> GetAsync(int instanceId, int cycleId);
    Task<PdiCyclePO> UpdateStatusAsync(int instanceId, int cycleId, CycleStatus status);
}

public interface IBscService
{
    Task<IReadOnlyList<BscPerspectivePO>> ListPerspectivesAsync(int instanceId);
    Task<BscGoalPO> CreateGoalAsync(int instanceId, BscGoalPO goal);
    Task<IReadOnlyList<BscGoalPO>> ListGoalsAsync(int instanceId, int? ownerId = null, BscStatus? status = null, int? perspectiveId = null);
    Task<BscIndicatorPO> CreateIndicatorAsync(int instanceId, BscIndicatorPO indicator);
    Task<BscIndicatorPO> UpdateIndicatorAsync(int instanceId, BscIndicatorPO indicator);
}

public interface IReviewService
{
    Task<PerformanceReviewPO> CreateAsync(int instanceId, PerformanceReviewPO review, IEnumerable<ReviewResponsePO>? responses = null);
    Task<PerformanceReviewPO> SubmitAsync(int instanceId, int reviewId);
    Task<PerformanceReviewPO> Create360Async(int instanceId, int employeeId, int cycleId);
    Task<PerformanceCyclePO> CreateCycleAsync(int instanceId, PerformanceCyclePO cycle);
    Task<IReadOnlyList<PerformanceReviewPO>> ListAsync(int instanceId, int? employeeId = null, ReviewStatus? status = null, int? cycleId = null);
}

public interface IPulseService
{
    Task<PulseSurveyPO> CreateAsync(int instanceId, PulseSurveyPO survey, IEnumerable<PulseQuestionPO>? questions = null);
    Task<PulseResponsePO> RespondAsync(int instanceId, int surveyId, PulseResponsePO response);
    Task<object> GetResultsAsync(int instanceId, int surveyId, int? departmentId = null);
}

public interface IFeedbackService
{
    Task<FeedbackPO> CreateAsync(int instanceId, FeedbackPO feedback);
    Task<IReadOnlyList<FeedbackPO>> ListAsync(int instanceId, int? employeeId = null, FeedbackType? type = null, FeedbackVisibility? visibility = null);
}

public interface ICheckInService
{
    Task<CheckInPO> CreateAsync(int instanceId, CheckInPO checkIn);
    Task<IReadOnlyList<CheckInPO>> ListAsync(int instanceId, int? employeeId = null, DateTime? from = null, DateTime? to = null);
}

public interface IQuestionBankService
{
    Task<IReadOnlyList<QuestionBankPO>> ListBanksAsync(int instanceId);
    Task<IReadOnlyList<QuestionPO>> ListQuestionsAsync(int instanceId, int? bankId = null, string? search = null);
    Task<QuestionBankPO> CreateBankAsync(int instanceId, QuestionBankPO bank);
    Task<QuestionPO> CreateQuestionAsync(int instanceId, QuestionPO question);
    Task<IReadOnlyList<QuestionPO>> SelectSmartAsync(int instanceId, AiContextType contextType, int? contextId = null, int take = 10);
    Task<bool> ValidateCrossAsync(int instanceId, int questionId, int? targetBankId = null);
}

public interface IAiRecommendationService
{
    Task<IReadOnlyList<AiRecommendationPO>> RecommendPdiAsync(int instanceId, int employeeId, int? competencyId = null);
    Task<IReadOnlyList<AiRecommendationPO>> DiscInsightsAsync(int instanceId, int employeeId);
    Task<object> TeamAnalysisAsync(int instanceId, int managerId, string? period = null);
    Task<object> LeadershipTipsAsync(int instanceId, int leaderId, string? context = null);
    Task<IReadOnlyList<QuestionPO>> QuestionSuggestAsync(int instanceId, int? employeeId = null, int? contextId = null);
}

public interface IConfigService
{
    Task<ConfigurationPO?> GetAsync(int instanceId, string key);
    Task<IReadOnlyList<ConfigurationPO>> ListAsync(int instanceId, string? prefix = null);
    Task<ConfigurationPO> SetAsync(int instanceId, string key, string? value, string? type = null, string? description = null);
}

public interface IAuditLogService
{
    Task<IReadOnlyList<AuditLogPO>> ListAsync(int instanceId, int skip = 0, int take = 50, string? entityType = null, string? action = null, int? userId = null);
    Task<IReadOnlyList<AuditLogPO>> SearchAsync(int instanceId, string term, int take = 50);
}

public interface IExecutionStatusService
{
    Task RecordAsync(string serviceName, string item, string status, string message, string? details = null);
    Task RecordSuccess(string serviceName, string item, string message, string? details = null);
    Task RecordInformation(string serviceName, string item, string message, string? details = null);
    Task RecordWarning(string serviceName, string item, string message, string? details = null);
    Task RecordError(string serviceName, string item, string message, string? details = null);
    Task<object> GetLastAsync(string? serviceName, string? item, int count);
    Task<object> GetByExecutionKeyAsync(string executionKey);
}
```

### 3.3 Observacoes de implementacao

- os services devem ser finos e transacionais
- `InstanceId` sempre precisa ser aplicado nas queries e nas escritas
- filtros globais no `DbContext` devem evitar vazamento cross-tenant
- enums persistidos no banco devem ser convertidos para string

---

## 10. Apendice C - Eleva.Server - MCP Services

O MCP segue o padrao definido pelo Escalada Framework:

- `IMcpService` registra tools em `McpServiceRegistry`
- cada tool e um `McpFunction`
- `ToolAnnotation` indica se a tool e read-only, mutating, external action ou dangerous
- `McpArgs` faz parse resiliente dos argumentos
- `McpSchemaValidator` valida payload por schema

### 4.1 Tipos MCP base

```csharp
namespace Eleva.Server.Mcp;

public interface IMcpService
{
    void RegisterTools(McpServiceRegistry registry);
}

public class McpFunction
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ToolAnnotation Annotation { get; set; }
    public Func<Dictionary<string, object?>, IServiceProvider, Task<object?>> Handler { get; set; } = null!;
    public Dictionary<string, McpParameter> Parameters { get; set; } = new();
    public string? SchemaJson { get; set; }
}

public class McpParameter
{
    public string Type { get; set; } = "string";
    public string? Description { get; set; }
    public bool Required { get; set; }
    public bool IsMoney { get; set; }
    public string[]? Enum { get; set; }
}

public enum ToolAnnotation
{
    ReadOnly,
    Mutating,
    ExternalAction,
    Dangerous
}
```

### 4.2 Registry e contexto

```csharp
namespace Eleva.Server.Mcp;

public class McpServiceRegistry
{
    private readonly Dictionary<string, McpFunction> _tools = new();

    public void Register(McpFunction function) => _tools[function.Name] = function;
    public McpFunction? GetTool(string name) => _tools.GetValueOrDefault(name);
    public IReadOnlyDictionary<string, McpFunction> GetAllTools() => _tools;
    public IEnumerable<McpFunction> GetToolsByAnnotation(ToolAnnotation annotation) => _tools.Values.Where(t => t.Annotation == annotation);
}

public class InstanceContext
{
    public int InstanceId { get; set; }
    public int? UserId { get; set; }
}

public sealed class ScopedInstanceAccessor : Eleva.Shared.Interfaces.ICurrentInstanceAccessor
{
    private readonly InstanceContext _instanceContext;

    public ScopedInstanceAccessor(InstanceContext instanceContext)
    {
        _instanceContext = instanceContext;
    }

    public int? GetInstanceId() => _instanceContext.InstanceId > 0 ? _instanceContext.InstanceId : null;
    public int? GetUserId() => _instanceContext.UserId;
}
```

### 4.3 Catalogo de tools por service

O bloco abaixo lista todas as tools esperadas. A implementacao concreta deve repetir o padrao `registry.Register(new McpFunction { ... Handler = async (args, sp) => ... })` para cada item.

#### PeopleMcpService

| Tool | Description | Annotation | Parameters |
|---|---|---|---|
| `employee_list` | Lista colaboradores com filtros | ReadOnly | `search`, `departmentId`, `managerId`, `isActive`, `skip`, `take` |
| `employee_get` | Busca colaborador por id | ReadOnly | `employeeId` |
| `employee_create` | Cria colaborador | Mutating | `name`, `email`, `departmentId`, `positionId`, `managerId`, `hireDate`, `isActive` |
| `employee_update` | Atualiza colaborador | Mutating | `employeeId`, `name`, `email`, `departmentId`, `positionId`, `managerId`, `status` |
| `department_list` | Lista departamentos | ReadOnly | `search`, `isActive` |
| `department_create` | Cria departamento | Mutating | `name`, `code`, `parentDepartmentId`, `managerId`, `description`, `type` |
| `position_list` | Lista cargos | ReadOnly | `search`, `departmentId`, `isActive` |
| `team_list` | Lista times | ReadOnly | `search`, `departmentId`, `leadId`, `isActive` |
| `hierarchy_get` | Retorna hierarquia | ReadOnly | `employeeId` |

```csharp
namespace Eleva.Server.Mcp.Services;

public class PeopleMcpService : IMcpService
{
    public void RegisterTools(McpServiceRegistry registry)
    {
        registry.Register(new McpFunction
        {
            Name = "employee_list",
            Description = "Listar colaboradores com filtros",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "search", new McpParameter { Description = "Busca por nome ou e-mail", Required = false } },
                { "departmentId", new McpParameter { Type = "integer", Description = "Filtrar por departamento", Required = false } },
                { "managerId", new McpParameter { Type = "integer", Description = "Filtrar por gestor", Required = false } },
                { "isActive", new McpParameter { Type = "boolean", Description = "Filtrar ativos", Required = false } },
                { "skip", new McpParameter { Type = "integer", Description = "Itens a ignorar", Required = false } },
                { "take", new McpParameter { Type = "integer", Description = "Itens a retornar", Required = false } }
            },
            Handler = async (args, sp) => await sp.GetRequiredService<Eleva.Services.Services.People.IEmployeeService>()
                .ListAsync(sp.GetRequiredService<InstanceContext>().InstanceId, McpArgs.StrOrNull(args, "search"), McpArgs.IntOrNull(args, "departmentId"), McpArgs.IntOrNull(args, "managerId"), args.ContainsKey("isActive") ? McpArgs.Bool(args, "isActive") : (bool?)null, McpArgs.Int(args, "skip", 0), McpArgs.Int(args, "take", 50))
        });
    }
}
```

#### DiscMcpService

| Tool | Description | Annotation | Parameters |
|---|---|---|---|
| `disc_assessment_create` | Cria assessment DISC | Mutating | `employeeId`, `assessmentType` |
| `disc_assessment_submit` | Envia respostas do DISC | Mutating | `assessmentId`, `responses` |
| `disc_result_get` | Retorna resultado do DISC | ReadOnly | `assessmentId` |
| `disc_team_map` | Mapa DISC do time | ReadOnly | `managerId` |
| `disc_matcher` | Sugere match comportamental | ReadOnly | `employeeId`, `targetEmployeeId` |

#### EcMcpService

| Tool | Description | Annotation | Parameters |
|---|---|---|---|
| `ec_assessment_create` | Cria avaliacao EC | Mutating | `assessment` |
| `ec_assessment_submit` | Envia respostas EC | Mutating | `assessmentId`, `responses` |
| `ec_result_get` | Retorna resultado EC | ReadOnly | `assessmentId` |
| `ec_org_maturity` | Maturidade organizacional | ReadOnly | `departmentId` |

#### PdiMcpService

| Tool | Description | Annotation | Parameters |
|---|---|---|---|
| `pdi_plan_create` | Cria plano PDI | Mutating | `plan`, `goals` |
| `pdi_plan_get` | Busca plano PDI | ReadOnly | `pdiPlanId` |
| `pdi_plan_list` | Lista planos PDI | ReadOnly | `employeeId`, `status`, `cycle` |
| `pdi_goal_create` | Cria meta PDI | Mutating | `pdiPlanId`, `goal` |
| `pdi_action_create` | Cria acao PDI | Mutating | `pdiGoalId`, `action` |
| `pdi_checkpoint_create` | Cria checkpoint | Mutating | `pdiPlanId`, `pdiGoalId`, `checkpoint` |
| `pdi_evidence_upload` | Anexa evidencias | Mutating | `pdiActionId`, `evidence` |
| `pdi_cycle_create` | Cria ciclo PDI | Mutating | `cycle` |
| `pdi_cycle_list` | Lista ciclos PDI | ReadOnly | `employeeId`, `status` |
| `pdi_progress_summary` | Resumo de progresso | ReadOnly | `pdiPlanId` |

#### PerformanceMcpService

| Tool | Description | Annotation | Parameters |
|---|---|---|---|
| `bsc_perspective_list` | Lista perspectivas | ReadOnly | nenhum |
| `bsc_goal_create` | Cria meta BSC | Mutating | `goal` |
| `bsc_goal_list` | Lista metas BSC | ReadOnly | `ownerId`, `status`, `perspectiveId` |
| `bsc_indicator_create` | Cria indicador | Mutating | `indicator` |
| `bsc_indicator_update` | Atualiza indicador | Mutating | `indicator` |
| `review_create` | Cria avaliacao de performance | Mutating | `review`, `responses` |
| `review_submit` | Submete avaliacao | Mutating | `reviewId` |
| `review_360_create` | Cria ciclo 360 | Mutating | `employeeId`, `cycleId` |
| `review_cycle_create` | Cria ciclo de avaliacao | Mutating | `cycle` |

#### EngagementMcpService

| Tool | Description | Annotation | Parameters |
|---|---|---|---|
| `pulse_create` | Cria pesquisa pulse | Mutating | `survey`, `questions` |
| `pulse_respond` | Responde pesquisa | Mutating | `surveyId`, `response` |
| `pulse_results` | Resultados pulse | ReadOnly | `surveyId`, `departmentId` |
| `feedback_create` | Cria feedback | Mutating | `feedback` |
| `feedback_list` | Lista feedbacks | ReadOnly | `employeeId`, `type`, `visibility` |
| `checkin_create` | Cria check-in | Mutating | `checkIn` |
| `checkin_list` | Lista check-ins | ReadOnly | `employeeId`, `from`, `to` |
| `mood_register` | Registra humor | Mutating | `entry` |
| `mood_team_summary` | Resumo de humor do time | ReadOnly | `managerId`, `period` |

#### AiCopilotMcpService

| Tool | Description | Annotation | Parameters |
|---|---|---|---|
| `ai_pdi_recommend` | Recomenda acoes de PDI | ReadOnly | `employeeId`, `competencyId` |
| `ai_disc_insights` | Insights DISC | ReadOnly | `employeeId` |
| `ai_team_analysis` | Analise do time | ReadOnly | `managerId`, `period` |
| `ai_leadership_tips` | Dicas de lideranca | ReadOnly | `leaderId`, `context` |
| `ai_question_suggest` | Sugere perguntas | ReadOnly | `employeeId`, `contextId` |

#### QuestionsMcpService

| Tool | Description | Annotation | Parameters |
|---|---|---|---|
| `question_bank_list` | Lista bancos de perguntas | ReadOnly | nenhum |
| `question_list` | Lista perguntas | ReadOnly | `bankId`, `search` |
| `question_create` | Cria pergunta | Mutating | `question` |
| `question_select_smart` | Seleciona perguntas com IA | ReadOnly | `contextType`, `contextId`, `take` |
| `question_validate_cross` | Valida cruzamento | ReadOnly | `questionId`, `targetBankId` |

#### SystemMcpService

| Tool | Description | Annotation | Parameters |
|---|---|---|---|
| `system_version` | Versao do sistema | ReadOnly | nenhum |
| `system_health` | Health do sistema | ReadOnly | nenhum |

#### ConfigMcpService

| Tool | Description | Annotation | Parameters |
|---|---|---|---|
| `config_get` | Busca configuracao | ReadOnly | `key` |
| `config_set` | Define configuracao | Mutating | `key`, `value`, `type`, `description` |
| `config_list` | Lista configuracoes | ReadOnly | `prefix` |

#### AuthMcpService

| Tool | Description | Annotation | Parameters |
|---|---|---|---|
| `auth_login` | Login do usuario | ExternalAction | `email`, `password`, `instanceSlug` |
| `auth_refresh` | Renova token | ExternalAction | `refreshToken` |
| `instance_create` | Cria instancia | Mutating | `name`, `slug`, `plan` |
| `instance_list` | Lista instancias | ReadOnly | nenhum |

#### AuditMcpService

| Tool | Description | Annotation | Parameters |
|---|---|---|---|
| `audit_log_list` | Lista logs de auditoria | ReadOnly | `skip`, `take`, `entityType`, `action`, `userId` |
| `audit_log_search` | Busca logs de auditoria | ReadOnly | `term`, `take` |

### 4.4 Padrao de registro MCP

Cada service deve seguir a mesma forma:

```csharp
registry.Register(new McpFunction
{
    Name = "tool_name",
    Description = "Descricao objetiva da tool",
    Annotation = ToolAnnotation.ReadOnly,
    Parameters = new Dictionary<string, McpParameter>
    {
        { "param", new McpParameter { Type = "string", Description = "Descricao", Required = true } }
    },
    Handler = async (args, sp) =>
    {
        var service = sp.GetRequiredService<ISomeService>();
        var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
        return await service.SomeMethodAsync(instanceId);
    }
});
```

---

## 11. Apendice D - Program.cs

O bootstrap segue o padrao definido pelo Escalada Framework, com SQLite no desenvolvimento e MySQL/MariaDB em producao.

```csharp
using Eleva.Services.Data;
using Eleva.Server.Mcp;
using Eleva.Server.Mcp.Services;
using Eleva.Server.Middleware;
using Eleva.Services.Services.People;
using Eleva.Services.Services.Assessments;
using Eleva.Services.Services.Pdi;
using Eleva.Services.Services.Performance;
using Eleva.Services.Services.Engagement;
using Eleva.Services.Services.Questions;
using Eleva.Services.Services.AICopilot;
using Eleva.Services.Services.Core;
using Eleva.Services.Services.History;
using Eleva.Services.Services.ExecutionStatus;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
if (connectionString.EndsWith(".db", StringComparison.OrdinalIgnoreCase) || connectionString.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<AppDbContext>(options => options
        .UseSqlite(connectionString)
        .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));
}
else
{
    var version = builder.Configuration["MySql:ServerVersion"] ?? "11.0.0-mariadb";
    builder.Services.AddDbContext<AppDbContext>(options => options
        .UseMySql(connectionString, ServerVersion.Parse(version), mysql => mysql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null))
        .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));
}

builder.Services.AddScoped<InstanceContext>();
builder.Services.AddScoped<Eleva.Shared.Interfaces.ICurrentInstanceAccessor, ScopedInstanceAccessor>();

builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDiscService, DiscService>();
builder.Services.AddScoped<IEcService, EcService>();
builder.Services.AddScoped<ICompetencyService, CompetencyService>();
builder.Services.AddScoped<IPdiService, PdiService>();
builder.Services.AddScoped<IPdiCycleService, PdiCycleService>();
builder.Services.AddScoped<IBscService, BscService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IPulseService, PulseService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<ICheckInService, CheckInService>();
builder.Services.AddScoped<IQuestionBankService, QuestionBankService>();
builder.Services.AddScoped<IAiRecommendationService, AiRecommendationService>();
builder.Services.AddScoped<IConfigService, ConfigService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IExecutionStatusService, ExecutionStatusService>();

builder.Services.AddSingleton<McpServiceRegistry>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    // placeholder para jobs de lembrete PDI, consolidacao BSC, engagement e IA
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = false);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (app.Environment.IsEnvironment("Testing"))
        db.Database.EnsureCreated();
    else
        db.Database.Migrate();
}

app.UseMiddleware<InstanceContextMiddleware>();
app.UseStaticFiles();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));
app.MapControllers();
app.Run();
```

---

## 12. Apendice E - Middleware e Controller

### 12.1 InstanceContextMiddleware

```csharp
namespace Eleva.Server.Middleware;

using Eleva.Server.Mcp;

public class InstanceContextMiddleware
{
    private readonly RequestDelegate _next;

    public InstanceContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, InstanceContext instanceContext)
    {
        if (context.Request.Headers.TryGetValue("X-Instance-Id", out var instanceIdHeader) && int.TryParse(instanceIdHeader, out var instanceId))
            instanceContext.InstanceId = instanceId;

        if (context.Request.Headers.TryGetValue("X-User-Id", out var userIdHeader) && int.TryParse(userIdHeader, out var userId))
            instanceContext.UserId = userId;

        await _next(context);
    }
}
```

### 12.2 McpController

O controller segue o contrato padrao do Escalada Framework com os ajustes abaixo:

- rota `mcp/tools`
- validacao por API key do tenant
- leitura do `InstanceContext`
- validacao de schema via `McpSchemaValidator`
- auditoria de chamadas mutating
- uso de `IExecutionStatusService` para registrar falhas

Fluxo de trabalho:

1. `GET /mcp/tools` lista tools registradas
2. `POST /mcp/tools/call` executa uma tool
3. o controller resolve o service via DI
4. o payload e validado antes da invocacao
5. falhas sao registradas no status de execucao

### 12.3 Estrutura esperada do controller

```csharp
namespace Eleva.Server.Controllers;

[ApiController]
[Route("mcp/tools")]
public class McpController : ControllerBase
{
    // Estrutura conforme Escalada Framework:
    // - registry
    // - db
    // - tenantContext
    // - configService
    // - executionStatusService
    // - lista de tools
    // - call tool
}
```

### 12.4 ScopedInstanceAccessor

`ScopedInstanceAccessor` faz a ponte entre o contexto HTTP e o contrato compartilhado `ICurrentInstanceAccessor`.

---

## 13. Apendice F - Regras de modelagem e persistencia

### 7.1 Multi-tenant

- toda entidade mutavel recebe `InstanceId`
- `InstanceId` e preenchido automaticamente no `SaveChanges`
- qualquer tentativa de salvar entidade multitenant sem contexto atual deve falhar
- queries devem ser filtradas por `InstanceId`

### 7.2 Auditoria

- toda operacao sensivel gera `AuditLogPO`
- mudancas estruturais geram `ChangeLogPO`
- logs de IA devem mascarar prompts sensiveis quando necessario

### 7.3 Conversao de enums

- enums persistem como string
- a API pode aceitar string ou enum dependendo do endpoint/tool

### 7.4 Soft delete

- objetos com historico devem suportar `DeletedAt`
- `HasQueryFilter` deve esconder registros removidos

---

## 14. Checklist de implementacao

- criar os 5 projetos da solution
- registrar os project references
- criar `Eleva.Shared` com enums, interfaces e POs
- criar `AppDbContext` em `Eleva.Services`
- criar servicos de dominio e interfaces
- criar `InstanceContextMiddleware`
- criar `McpController`
- registrar todos os MCP services no `Program.cs`
- configurar SQLite dev e MySQL/MariaDB prod
- adicionar Quartz como placeholder
- cobrir com testes unitarios e de integracao

---

## 15. Conclusao

Esta especificacao define a base estrutural do Eleva como um projeto novo de RH, implementado sobre os padroes do Escalada Framework para bootstrap, MCP, multi-tenancy e persistencia.
