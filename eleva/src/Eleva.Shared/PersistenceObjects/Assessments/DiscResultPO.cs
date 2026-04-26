namespace Eleva.Shared.PersistenceObjects.Assessments;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

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
