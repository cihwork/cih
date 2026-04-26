namespace Eleva.UnitTests;

using Eleva.Shared.PersistenceObjects.Core;
using Eleva.Shared.PersistenceObjects.People;
using Eleva.Shared.PersistenceObjects.Assessments;
using Eleva.Shared.PersistenceObjects.Pdi;
using Eleva.Shared.PersistenceObjects.Performance;
using Eleva.Shared.PersistenceObjects.Engagement;
using Eleva.Shared.PersistenceObjects.Questions;
using Eleva.Shared.PersistenceObjects.AICopilot;
using Eleva.Shared.PersistenceObjects.History;
using Eleva.Shared.Enums;

public class PersistenceObjectsTests
{
    // ── Core ──────────────────────────────────────────────────────────────

    [Fact]
    public void InstancePO_Properties_CanBeSetAndRead()
    {
        var po = new InstancePO
        {
            Id = 1, InstanceId = 1, ReferenceCode = "REF",
            Name = "Acme", Slug = "acme", McpApiKey = "key123",
            Plan = SubscriptionPlan.Professional, Status = InstanceStatus.Active,
            IsActive = true, MaxUsers = 100, SettingsJson = "{}",
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        Assert.Equal(1, po.Id); Assert.Equal("Acme", po.Name);
        Assert.Equal("acme", po.Slug); Assert.Equal(SubscriptionPlan.Professional, po.Plan);
        Assert.True(po.IsActive); Assert.Equal(100, po.MaxUsers);
    }

    [Fact]
    public void UserPO_Properties_CanBeSetAndRead()
    {
        var po = new UserPO
        {
            Id = 2, InstanceId = 1, Name = "Ana", Email = "ana@corp.com",
            PasswordHash = "hash", Role = UserRole.Admin, IsActive = true,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, LastLoginAt = DateTime.UtcNow
        };
        Assert.Equal("Ana", po.Name); Assert.Equal("ana@corp.com", po.Email);
        Assert.Equal(UserRole.Admin, po.Role); Assert.True(po.IsActive);
    }

    [Fact]
    public void ConfigurationPO_Properties_CanBeSetAndRead()
    {
        var po = new ConfigurationPO
        {
            Id = 3, InstanceId = 1, Key = "smtp.host", Value = "smtp.example.com",
            Type = "string", Description = "SMTP host", IsSecret = false, IsEncrypted = false,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        Assert.Equal("smtp.host", po.Key); Assert.Equal("smtp.example.com", po.Value);
        Assert.False(po.IsSecret); Assert.False(po.IsEncrypted);
    }

    // ── People ────────────────────────────────────────────────────────────

    [Fact]
    public void EmployeePO_Properties_CanBeSetAndRead()
    {
        var po = new EmployeePO
        {
            Id = 1, InstanceId = 1, Name = "João Silva", Email = "joao@corp.com",
            DocumentNumber = "123.456.789-00", Phone = "11999999999",
            AvatarUrl = "https://img.com/avatar.jpg", UserId = 5, DepartmentId = 2,
            PositionId = 3, ManagerId = 4, HireDate = new DateOnly(2023, 1, 15),
            BirthDate = new DateOnly(1990, 6, 20), Status = EmploymentStatus.Active,
            PositionTitle = "Dev", Timezone = "America/Sao_Paulo", Language = "pt-BR",
            SettingsJson = "{}", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, DeletedAt = null
        };
        Assert.Equal("João Silva", po.Name); Assert.Equal("joao@corp.com", po.Email);
        Assert.Equal(EmploymentStatus.Active, po.Status); Assert.Equal(2, po.DepartmentId);
    }

    [Fact]
    public void DepartmentPO_Properties_CanBeSetAndRead()
    {
        var po = new DepartmentPO
        {
            Id = 1, InstanceId = 1, Code = "TI", Name = "Tecnologia", Description = "Dept TI",
            ParentDepartmentId = null, ManagerId = 10, Type = OrganizationalUnitType.Department,
            IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, DeletedAt = null
        };
        Assert.Equal("TI", po.Code); Assert.Equal("Tecnologia", po.Name);
        Assert.Equal(OrganizationalUnitType.Department, po.Type); Assert.True(po.IsActive);
    }

    [Fact]
    public void PositionPO_Properties_CanBeSetAndRead()
    {
        var po = new PositionPO
        {
            Id = 1, InstanceId = 1, Code = "DEV", Name = "Desenvolvedor", Level = "Pleno",
            Description = "Dev pleno", DepartmentId = 2, IsActive = true,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, DeletedAt = null
        };
        Assert.Equal("DEV", po.Code); Assert.Equal("Desenvolvedor", po.Name);
        Assert.Equal("Pleno", po.Level); Assert.True(po.IsActive);
    }

    [Fact]
    public void TeamPO_Properties_CanBeSetAndRead()
    {
        var po = new TeamPO
        {
            Id = 1, InstanceId = 1, Code = "SQUAD-A", Name = "Squad Alpha",
            Description = "Time principal", DepartmentId = 1, LeadId = 5,
            IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, DeletedAt = null
        };
        Assert.Equal("SQUAD-A", po.Code); Assert.Equal("Squad Alpha", po.Name);
        Assert.Equal(5, po.LeadId); Assert.True(po.IsActive);
    }

    [Fact]
    public void TeamMemberPO_Properties_CanBeSetAndRead()
    {
        var po = new TeamMemberPO
        {
            Id = 1, InstanceId = 1, TeamId = 2, EmployeeId = 7,
            RoleInTeam = "Developer", JoinedAt = new DateOnly(2024, 1, 1),
            LeftAt = null, IsPrimary = true,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        Assert.Equal(2, po.TeamId); Assert.Equal(7, po.EmployeeId);
        Assert.Equal("Developer", po.RoleInTeam); Assert.True(po.IsPrimary);
    }

    // ── Assessments ───────────────────────────────────────────────────────

    [Fact]
    public void CompetencyPO_Properties_CanBeSetAndRead()
    {
        var po = new CompetencyPO
        {
            Id = 1, InstanceId = 1, Code = "COM-01", Name = "Comunicação",
            Description = "Clareza na comunicação", Category = CompetencyCategory.Behavioral,
            LevelType = LevelType.Numeric, LevelDefinitionsJson = "[]", IsCore = true,
            ParentCompetencyId = null, SortOrder = 1,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, DeletedAt = null
        };
        Assert.Equal("COM-01", po.Code); Assert.Equal(CompetencyCategory.Behavioral, po.Category);
        Assert.True(po.IsCore); Assert.Equal(LevelType.Numeric, po.LevelType);
    }

    [Fact]
    public void CompetencyLibraryPO_Properties_CanBeSetAndRead()
    {
        var po = new CompetencyLibraryPO
        {
            Id = 1, InstanceId = 1, Code = "LIB-01", Name = "Biblioteca Principal",
            Description = "Competências core", Version = "2.0", Source = "RH",
            IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        Assert.Equal("LIB-01", po.Code); Assert.Equal("2.0", po.Version); Assert.True(po.IsActive);
    }

    [Fact]
    public void DiscAssessmentPO_Properties_CanBeSetAndRead()
    {
        var po = new DiscAssessmentPO
        {
            Id = 1, InstanceId = 1, EmployeeId = 5, AssessedById = 2,
            Status = AssessmentStatus.Completed, PrimaryStyle = DiscProfile.D,
            SecondaryStyle = DiscProfile.I, StartedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow, ExpiresAt = DateTime.UtcNow.AddYears(1),
            AssessmentLink = "https://disc.com", ReportUrl = "https://report.com",
            RawDataJson = "{}", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        Assert.Equal(5, po.EmployeeId); Assert.Equal(AssessmentStatus.Completed, po.Status);
        Assert.Equal(DiscProfile.D, po.PrimaryStyle); Assert.Equal(DiscProfile.I, po.SecondaryStyle);
    }

    [Fact]
    public void DiscResponsePO_Properties_CanBeSetAndRead()
    {
        var po = new DiscResponsePO
        {
            Id = 1, InstanceId = 1, DiscAssessmentId = 3, QuestionCode = "Q01",
            AnswerValue = "A", Axis = "D", AnsweredAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow
        };
        Assert.Equal(3, po.DiscAssessmentId); Assert.Equal("Q01", po.QuestionCode);
        Assert.Equal("A", po.AnswerValue); Assert.Equal("D", po.Axis);
    }

    [Fact]
    public void DiscResultPO_Properties_CanBeSetAndRead()
    {
        var po = new DiscResultPO
        {
            Id = 1, InstanceId = 1, DiscAssessmentId = 3, DScore = 75m, IScore = 60m,
            SScore = 40m, CScore = 50m, PrimaryStyle = DiscProfile.D,
            SecondaryStyle = DiscProfile.I, ProfileCombination = "DI",
            Interpretation = "Dominante", StrengthsJson = "[]", DevelopmentAreasJson = "[]",
            CommunicationTipsJson = "[]", StressIndicatorsJson = "[]", CreatedAt = DateTime.UtcNow
        };
        Assert.Equal(75m, po.DScore); Assert.Equal(DiscProfile.D, po.PrimaryStyle);
        Assert.Equal("DI", po.ProfileCombination);
    }

    [Fact]
    public void EcAssessmentPO_Properties_CanBeSetAndRead()
    {
        var po = new EcAssessmentPO
        {
            Id = 1, InstanceId = 1, OrganizationalUnitId = 2,
            AssessmentType = AssessmentType.CollectiveListening,
            Title = "Escuta 2024", Description = "Diagnóstico anual",
            StartDate = new DateOnly(2024, 1, 1), EndDate = new DateOnly(2024, 3, 31),
            Status = AssessmentStatus.Completed, ConsciousnessLevel = EcLevel.Level3,
            ParticipationRate = 0.85m, OverallScore = 72m, DimensionsJson = "{}",
            Findings = "Bom engajamento", Recommendations = "Melhorar comunicação",
            ConductedById = 4, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, DeletedAt = null
        };
        Assert.Equal("Escuta 2024", po.Title); Assert.Equal(EcLevel.Level3, po.ConsciousnessLevel);
        Assert.Equal(0.85m, po.ParticipationRate); Assert.Equal(AssessmentStatus.Completed, po.Status);
    }

    [Fact]
    public void EcResponsePO_Properties_CanBeSetAndRead()
    {
        var po = new EcResponsePO
        {
            Id = 1, InstanceId = 1, EcAssessmentId = 5, EmployeeId = 7,
            QuestionId = 2, ResponseDate = DateTime.UtcNow, AnswersJson = "{}",
            Sentiment = SentimentLevel.Positive, EnergyLevel = 4,
            MetadataJson = "{}", CreatedAt = DateTime.UtcNow
        };
        Assert.Equal(5, po.EcAssessmentId); Assert.Equal(SentimentLevel.Positive, po.Sentiment);
        Assert.Equal(4, po.EnergyLevel);
    }

    [Fact]
    public void EcResultPO_Properties_CanBeSetAndRead()
    {
        var po = new EcResultPO
        {
            Id = 1, InstanceId = 1, EcAssessmentId = 5, MaturityLevel = EcLevel.Level4,
            ParticipationRate = 0.9m, OverallScore = 78m, StrengthsJson = "[]",
            RisksJson = "[]", RecommendationsJson = "[]",
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        Assert.Equal(EcLevel.Level4, po.MaturityLevel); Assert.Equal(0.9m, po.ParticipationRate);
    }

    // ── PDI ───────────────────────────────────────────────────────────────

    [Fact]
    public void PdiCyclePO_Properties_CanBeSetAndRead()
    {
        var po = new PdiCyclePO
        {
            Id = 1, InstanceId = 1, EmployeeId = 5, CoachId = 2, ManagerId = 3,
            CycleName = "Q1 2024", StartDate = new DateOnly(2024, 1, 1),
            EndDate = new DateOnly(2024, 3, 31), Status = CycleStatus.Active,
            OverallProgress = 0.45m, FocusAreasJson = "[]", Context = "Desenvolvimento técnico",
            KickoffDate = new DateOnly(2024, 1, 5), CompletionDate = null,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, DeletedAt = null
        };
        Assert.Equal("Q1 2024", po.CycleName); Assert.Equal(CycleStatus.Active, po.Status);
        Assert.Equal(0.45m, po.OverallProgress);
    }

    [Fact]
    public void PdiPlanPO_Properties_CanBeSetAndRead()
    {
        var po = new PdiPlanPO
        {
            Id = 1, InstanceId = 1, PdiCycleId = 2, EmployeeId = 5, CoachId = 3, ManagerId = 4,
            Title = "PDI Q1", Description = "Plano de desenvolvimento", Status = PdiStatus.Active,
            Progress = 0.3m, StartDate = new DateOnly(2024, 1, 1), EndDate = new DateOnly(2024, 3, 31),
            Context = "Foco em liderança", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, DeletedAt = null
        };
        Assert.Equal("PDI Q1", po.Title); Assert.Equal(PdiStatus.Active, po.Status);
        Assert.Equal(0.3m, po.Progress);
    }

    [Fact]
    public void PdiGoalPO_Properties_CanBeSetAndRead()
    {
        var po = new PdiGoalPO
        {
            Id = 1, InstanceId = 1, PdiPlanId = 2, CompetencyId = 3,
            Title = "Melhorar comunicação", Description = "Desenvolver clareza verbal",
            GoalType = PdiGoalType.Competency, CurrentLevel = 2, TargetLevel = 4,
            Priority = PdiPriority.High, Status = PdiGoalStatus.InProgress,
            Progress = 0.5m, Deadline = new DateOnly(2024, 3, 31),
            CompletedDate = null, SuccessCriteria = "Avaliação 360 acima de 4",
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, DeletedAt = null
        };
        Assert.Equal("Melhorar comunicação", po.Title); Assert.Equal(PdiGoalType.Competency, po.GoalType);
        Assert.Equal(PdiPriority.High, po.Priority); Assert.Equal(0.5m, po.Progress);
    }

    [Fact]
    public void PdiActionPO_Properties_CanBeSetAndRead()
    {
        var po = new PdiActionPO
        {
            Id = 1, InstanceId = 1, PdiGoalId = 2, Title = "Curso de oratória",
            Description = "Treinamento presencial", ActionType = PdiActionType.Formal10,
            EstimatedHours = 16m, ActualHours = 16m,
            DueDate = new DateOnly(2024, 2, 28), CompletedDate = new DateOnly(2024, 2, 25),
            Status = PdiActionStatus.Completed, ResourcesNeeded = "R$ 500",
            EvidenceUrl = "https://certificado.com", Notes = "Excelente curso",
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, DeletedAt = null
        };
        Assert.Equal("Curso de oratória", po.Title); Assert.Equal(PdiActionType.Formal10, po.ActionType);
        Assert.Equal(PdiActionStatus.Completed, po.Status); Assert.Equal(16m, po.EstimatedHours);
    }

    [Fact]
    public void PdiCheckpointPO_Properties_CanBeSetAndRead()
    {
        var po = new PdiCheckpointPO
        {
            Id = 1, InstanceId = 1, PdiPlanId = 2, PdiGoalId = 3,
            CheckpointDate = new DateOnly(2024, 2, 1), Type = PdiCheckpointType.Monthly,
            Status = PdiCheckpointStatus.Completed, OverallSentiment = SentimentLevel.Positive,
            ProgressSummary = "Bom progresso", Wins = "Curso concluído", Challenges = "Tempo limitado",
            NextSteps = "Praticar em reuniões", CoachNotes = "Evoluindo bem",
            CompletedById = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        Assert.Equal(PdiCheckpointType.Monthly, po.Type); Assert.Equal(PdiCheckpointStatus.Completed, po.Status);
        Assert.Equal(SentimentLevel.Positive, po.OverallSentiment);
    }

    [Fact]
    public void PdiEvidencePO_Properties_CanBeSetAndRead()
    {
        var po = new PdiEvidencePO
        {
            Id = 1, InstanceId = 1, PdiActionId = 3, FileName = "certificado.pdf",
            FileUrl = "https://storage.com/cert.pdf", FileType = "application/pdf",
            ContentHash = "sha256hash", UploadedById = 5,
            UploadedAt = DateTime.UtcNow, Notes = "Certificado original",
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        Assert.Equal("certificado.pdf", po.FileName); Assert.Equal("application/pdf", po.FileType);
        Assert.Equal("sha256hash", po.ContentHash);
    }

    // ── Performance ───────────────────────────────────────────────────────

    [Fact]
    public void PerformanceCyclePO_Properties_CanBeSetAndRead()
    {
        var po = new PerformanceCyclePO
        {
            Id = 1, InstanceId = 1, Name = "Avaliação 2024", Description = "Ciclo anual",
            StartDate = new DateOnly(2024, 1, 1), EndDate = new DateOnly(2024, 12, 31),
            Status = CycleStatus.Active, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, DeletedAt = null
        };
        Assert.Equal("Avaliação 2024", po.Name); Assert.Equal(CycleStatus.Active, po.Status);
    }

    [Fact]
    public void BscPerspectivePO_Properties_CanBeSetAndRead()
    {
        var po = new BscPerspectivePO
        {
            Id = 1, InstanceId = 1, Code = "FIN", Name = "Financeira",
            Description = "Perspectiva financeira", SortOrder = 1,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        Assert.Equal("FIN", po.Code); Assert.Equal("Financeira", po.Name); Assert.Equal(1, po.SortOrder);
    }

    [Fact]
    public void BscGoalPO_Properties_CanBeSetAndRead()
    {
        var po = new BscGoalPO
        {
            Id = 1, InstanceId = 1, PerformanceCycleId = 2, PerspectiveId = 1,
            DepartmentId = 3, OwnerId = 5, CascadedFromGoalId = null,
            Code = "GOAL-01", Title = "Aumentar receita", Description = "Meta de crescimento",
            Status = BscStatus.Active, Weight = 0.3m,
            StartDate = new DateOnly(2024, 1, 1), EndDate = new DateOnly(2024, 12, 31),
            Progress = 0.4m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, DeletedAt = null
        };
        Assert.Equal("GOAL-01", po.Code); Assert.Equal(BscStatus.Active, po.Status);
        Assert.Equal(0.3m, po.Weight);
    }

    [Fact]
    public void BscIndicatorPO_Properties_CanBeSetAndRead()
    {
        var po = new BscIndicatorPO
        {
            Id = 1, InstanceId = 1, BscGoalId = 2, Code = "IND-01", Name = "NPS",
            Description = "Net Promoter Score", Formula = "Promotores - Detratores",
            UnitOfMeasure = "%", BaselineValue = 30m, TargetValue = 60m, CurrentValue = 45m,
            MeasurementFrequency = "Mensal", DataSource = "CRM", IsHigherBetter = true,
            Status = IndicatorStatus.OnTrack, LastMeasuredDate = new DateOnly(2024, 3, 31),
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, DeletedAt = null
        };
        Assert.Equal("NPS", po.Name); Assert.Equal(IndicatorStatus.OnTrack, po.Status);
        Assert.True(po.IsHigherBetter); Assert.Equal(60m, po.TargetValue);
    }

    [Fact]
    public void PerformanceReviewPO_Properties_CanBeSetAndRead()
    {
        var po = new PerformanceReviewPO
        {
            Id = 1, InstanceId = 1, PerformanceCycleId = 2, EmployeeId = 5,
            ReviewerId = 3, CalibrationOwnerId = 4, ReviewType = "360",
            Status = ReviewStatus.Submitted, OverallScore = 4.2m, CalibrationScore = 4.0m,
            Comments = "Ótimo desempenho", SubmittedAt = DateTime.UtcNow, ClosedAt = null,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, DeletedAt = null
        };
        Assert.Equal(ReviewStatus.Submitted, po.Status); Assert.Equal(4.2m, po.OverallScore);
        Assert.Equal("360", po.ReviewType);
    }

    [Fact]
    public void ReviewResponsePO_Properties_CanBeSetAndRead()
    {
        var po = new ReviewResponsePO
        {
            Id = 1, InstanceId = 1, PerformanceReviewId = 2, QuestionId = 5,
            AnswerText = "Excelente colaboração", Score = 4.5m, EvidenceJson = "[]",
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        Assert.Equal(2, po.PerformanceReviewId); Assert.Equal(4.5m, po.Score);
        Assert.Equal("Excelente colaboração", po.AnswerText);
    }

    // ── Engagement ────────────────────────────────────────────────────────

    [Fact]
    public void PulseSurveyPO_Properties_CanBeSetAndRead()
    {
        var po = new PulseSurveyPO
        {
            Id = 1, InstanceId = 1, OrganizationalUnitId = 2, Title = "Pulse Semanal",
            Type = PulseType.Engagement, Frequency = PulseFrequency.Weekly,
            QuestionsJson = "[]", IsAnonymous = true, Status = SurveyStatus.Active,
            StartDate = new DateOnly(2024, 3, 1), EndDate = new DateOnly(2024, 3, 31),
            CreatedById = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, DeletedAt = null
        };
        Assert.Equal("Pulse Semanal", po.Title); Assert.Equal(PulseType.Engagement, po.Type);
        Assert.True(po.IsAnonymous); Assert.Equal(SurveyStatus.Active, po.Status);
    }

    [Fact]
    public void PulseQuestionPO_Properties_CanBeSetAndRead()
    {
        var po = new PulseQuestionPO
        {
            Id = 1, InstanceId = 1, PulseSurveyId = 3,
            QuestionText = "Como está sua energia hoje?", SortOrder = 1,
            Required = true, QuestionType = QuestionType.Scale, OptionsJson = null,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        Assert.Equal("Como está sua energia hoje?", po.QuestionText);
        Assert.True(po.Required); Assert.Equal(QuestionType.Scale, po.QuestionType);
    }

    [Fact]
    public void PulseResponsePO_Properties_CanBeSetAndRead()
    {
        var po = new PulseResponsePO
        {
            Id = 1, InstanceId = 1, PulseSurveyId = 3, PulseQuestionId = 1,
            EmployeeId = 7, ResponseDate = DateTime.UtcNow, AnswersJson = "{\"score\":4}",
            OverallSentiment = SentimentLevel.Positive, EnergyLevel = 4,
            MetadataJson = "{}", CreatedAt = DateTime.UtcNow
        };
        Assert.Equal(3, po.PulseSurveyId); Assert.Equal(SentimentLevel.Positive, po.OverallSentiment);
        Assert.Equal(4, po.EnergyLevel);
    }

    [Fact]
    public void FeedbackPO_Properties_CanBeSetAndRead()
    {
        var po = new FeedbackPO
        {
            Id = 1, InstanceId = 1, FromEmployeeId = 3, ToEmployeeId = 5,
            OneOnOneId = null, Context = "Sprint review", Content = "Ótimo trabalho!",
            FeedbackType = FeedbackType.Positive, Sentiment = SentimentLevel.VeryPositive,
            CompetencyId = 2, IsPrivate = false, Visibility = FeedbackVisibility.PersonAndManager,
            DeliveredDate = DateTime.UtcNow, Status = FeedbackRecordStatus.Delivered,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, DeletedAt = null
        };
        Assert.Equal(FeedbackType.Positive, po.FeedbackType); Assert.Equal("Ótimo trabalho!", po.Content);
        Assert.Equal(FeedbackRecordStatus.Delivered, po.Status); Assert.False(po.IsPrivate);
    }

    [Fact]
    public void CheckInPO_Properties_CanBeSetAndRead()
    {
        var po = new CheckInPO
        {
            Id = 1, InstanceId = 1, EmployeeId = 5, ManagerId = 3,
            Date = DateTime.UtcNow, Mood = SentimentLevel.Positive,
            EnergyLevel = 4, ProductivityScore = 8, Notes = "Bom dia",
            Blockers = "Nenhum", SupportNeeded = false, Status = OneOnOneStatus.Completed,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        Assert.Equal(SentimentLevel.Positive, po.Mood); Assert.Equal(4, po.EnergyLevel);
        Assert.Equal(8, po.ProductivityScore); Assert.False(po.SupportNeeded);
    }

    [Fact]
    public void MoodEntryPO_Properties_CanBeSetAndRead()
    {
        var po = new MoodEntryPO
        {
            Id = 1, InstanceId = 1, EmployeeId = 5, EntryAt = DateTime.UtcNow,
            Mood = SentimentLevel.Neutral, EnergyLevel = 3,
            Notes = "Dia tranquilo", Source = "app", CreatedAt = DateTime.UtcNow
        };
        Assert.Equal(SentimentLevel.Neutral, po.Mood); Assert.Equal(3, po.EnergyLevel);
        Assert.Equal("app", po.Source);
    }

    // ── Questions ─────────────────────────────────────────────────────────

    [Fact]
    public void QuestionBankPO_Properties_CanBeSetAndRead()
    {
        var po = new QuestionBankPO
        {
            Id = 1, InstanceId = 1, Code = "QB-DISC", Name = "Banco DISC",
            Description = "Perguntas para avaliação DISC", Version = "1.0",
            IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, DeletedAt = null
        };
        Assert.Equal("QB-DISC", po.Code); Assert.Equal("1.0", po.Version); Assert.True(po.IsActive);
    }

    [Fact]
    public void QuestionPO_Properties_CanBeSetAndRead()
    {
        var po = new QuestionPO
        {
            Id = 1, InstanceId = 1, QuestionBankId = 2, Code = "Q-001",
            Text = "Como você se comporta sob pressão?", HelpText = "Pense em situações reais",
            QuestionType = QuestionType.SingleChoice, AnswerFormat = AnswerFormat.Text,
            IsRequired = true, SortOrder = 1, MetadataJson = "{}",
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, DeletedAt = null
        };
        Assert.Equal("Q-001", po.Code); Assert.Equal(QuestionType.SingleChoice, po.QuestionType);
        Assert.True(po.IsRequired); Assert.Equal(AnswerFormat.Text, po.AnswerFormat);
    }

    [Fact]
    public void AnswerPO_Properties_CanBeSetAndRead()
    {
        var po = new AnswerPO
        {
            Id = 1, InstanceId = 1, QuestionId = 3, EmployeeId = 7,
            TextValue = "Mantenho a calma", NumericValue = null,
            BooleanValue = null, JsonValue = null, Score = 4.5m,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        Assert.Equal(3, po.QuestionId); Assert.Equal("Mantenho a calma", po.TextValue);
        Assert.Equal(4.5m, po.Score);
    }

    // ── AICopilot ─────────────────────────────────────────────────────────

    [Fact]
    public void AiRecommendationPO_Properties_CanBeSetAndRead()
    {
        var po = new AiRecommendationPO
        {
            Id = 1, InstanceId = 1, EmployeeId = 5,
            ContextType = AiContextType.Pdi, ContextId = 2,
            RecommendationType = AiRecommendationType.LearningPath,
            Title = "Curso de liderança", Description = "Recomendação baseada no PDI",
            Rationale = "Perfil D-I identificado", SuggestedActionsJson = "[]",
            ModelUsed = "claude-opus-4-6", ConfidenceScore = 0.87m,
            Status = AiRecommendationStatus.Pending, ReviewedById = null, ReviewedAt = null,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        Assert.Equal("Curso de liderança", po.Title); Assert.Equal(AiContextType.Pdi, po.ContextType);
        Assert.Equal(AiRecommendationStatus.Pending, po.Status); Assert.Equal(0.87m, po.ConfidenceScore);
    }

    [Fact]
    public void AiInteractionPO_Properties_CanBeSetAndRead()
    {
        var po = new AiInteractionPO
        {
            Id = 1, InstanceId = 1, EmployeeId = 5,
            ContextType = AiContextType.OneOnOne, ContextId = 3,
            Prompt = "Quais tópicos abordar no 1:1?", PromptHash = "abc123",
            ResponseSummary = "Foco em bloqueios técnicos", ModelUsed = "claude-opus-4-6",
            TokensIn = 150, TokensOut = 300, ConfidenceScore = 0.92m,
            CreatedById = 7, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        Assert.Equal(AiContextType.OneOnOne, po.ContextType); Assert.Equal(150, po.TokensIn);
        Assert.Equal(0.92m, po.ConfidenceScore);
    }

    // ── History ───────────────────────────────────────────────────────────

    [Fact]
    public void AuditLogPO_Properties_CanBeSetAndRead()
    {
        var po = new AuditLogPO
        {
            Id = 1, InstanceId = 1, UserId = 5, Action = "CREATE",
            EntityType = "EmployeePO", EntityId = "42", ChangesJson = "{}",
            IpAddress = "192.168.1.1", UserAgent = "Mozilla/5.0",
            Status = AuditStatus.Success, ErrorMessage = null, MetadataJson = "{}",
            Timestamp = DateTime.UtcNow
        };
        Assert.Equal("CREATE", po.Action); Assert.Equal("EmployeePO", po.EntityType);
        Assert.Equal(AuditStatus.Success, po.Status); Assert.Equal("192.168.1.1", po.IpAddress);
    }

    [Fact]
    public void ChangeLogPO_Properties_CanBeSetAndRead()
    {
        var po = new ChangeLogPO
        {
            Id = 1, InstanceId = 1, EntityType = "EmployeePO", EntityId = "42",
            Action = "UPDATE", BeforeJson = "{\"name\":\"João\"}",
            AfterJson = "{\"name\":\"João Silva\"}", ChangedById = 3,
            ChangedAt = DateTime.UtcNow, Source = ChangeSource.Api, CorrelationId = "corr-xyz"
        };
        Assert.Equal("UPDATE", po.Action); Assert.Equal(ChangeSource.Api, po.Source);
        Assert.Equal("corr-xyz", po.CorrelationId);
    }
}
