namespace Eleva.Shared.PersistenceObjects.Assessments;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

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
