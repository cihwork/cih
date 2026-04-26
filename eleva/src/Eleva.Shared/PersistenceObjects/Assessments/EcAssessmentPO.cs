namespace Eleva.Shared.PersistenceObjects.Assessments;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

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
