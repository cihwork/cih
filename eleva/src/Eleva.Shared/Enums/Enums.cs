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
    GapAnalysis,
    Disc
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
