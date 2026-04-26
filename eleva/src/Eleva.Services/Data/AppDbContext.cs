namespace Eleva.Services.Data;

using System.Reflection;
using Eleva.Shared.Interfaces;
using Eleva.Shared.PersistenceObjects.Core;
using Eleva.Shared.PersistenceObjects.People;
using Eleva.Shared.PersistenceObjects.Assessments;
using Eleva.Shared.PersistenceObjects.Pdi;
using Eleva.Shared.PersistenceObjects.Performance;
using Eleva.Shared.PersistenceObjects.Engagement;
using Eleva.Shared.PersistenceObjects.Questions;
using Eleva.Shared.PersistenceObjects.AICopilot;
using Eleva.Shared.PersistenceObjects.History;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    private readonly ICurrentInstanceAccessor? _instanceAccessor;

    /// <summary>
    /// Flag to disable instance filters temporarily (e.g., when loading host mapping)
    /// CRITICAL: Prevents infinite loops in ICurrentInstanceAccessor
    /// </summary>
    public bool DisableInstanceFilters { get; set; } = false;

    /// <summary>
    /// Flag to disable InstanceId interceptor during seeding
    /// Allows manual control of InstanceId assignment
    /// </summary>
    public bool DisableInstanceIdInterceptor { get; set; } = false;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ApplyGlobalQueryFilters(modelBuilder);
    }

    private void ApplyGlobalQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (typeof(IInstanceBaseEntity).IsAssignableFrom(clrType))
            {
                var method = SetGlobalQueryMethod.MakeGenericMethod(clrType);
                method.Invoke(this, new object[] { modelBuilder });
            }
        }
    }

    private static readonly MethodInfo SetGlobalQueryMethod = typeof(AppDbContext)
        .GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Single(t => t.IsGenericMethodDefinition && t.Name == nameof(SetGlobalQuery));

    public void SetGlobalQuery<T>(ModelBuilder modelBuilder) where T : class, IInstanceBaseEntity
    {
        if (_instanceAccessor != null)
        {
            modelBuilder.Entity<T>().HasQueryFilter(e =>
                DisableInstanceFilters || e.InstanceId == _instanceAccessor.GetInstanceId());
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddInstanceId();
        AddTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void AddInstanceId()
    {
        if (DisableInstanceIdInterceptor)
            return;

        var instanceId = _instanceAccessor?.GetInstanceId();
        if (instanceId == null)
            return;

        var entities = ChangeTracker.Entries()
            .Where(x => x.Entity is IInstanceBaseEntity &&
                        (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (var entity in entities)
        {
            var instanceEntity = (IInstanceBaseEntity)entity.Entity;
            if (entity.State == EntityState.Added && instanceEntity.InstanceId == 0)
            {
                instanceEntity.InstanceId = instanceId.Value;
            }
            else if (entity.State == EntityState.Modified && instanceEntity.InstanceId == 0)
            {
                instanceEntity.InstanceId = instanceId.Value;
            }
        }
    }

    private void AddTimestamps()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

        foreach (var entity in entities)
        {
            var type = entity.Entity.GetType();
            var createdAt = type.GetProperty("CreatedAt");
            var updatedAt = type.GetProperty("UpdatedAt");

            if (entity.State == EntityState.Added && createdAt != null)
            {
                if ((DateTime)(createdAt.GetValue(entity.Entity) ?? default(DateTime)) == default)
                    createdAt.SetValue(entity.Entity, DateTime.UtcNow);
            }

            if (updatedAt != null)
                updatedAt.SetValue(entity.Entity, DateTime.UtcNow);
        }
    }
}
