namespace Eleva.Shared.PersistenceObjects.Assessments;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

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
