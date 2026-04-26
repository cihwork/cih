namespace Eleva.Shared.PersistenceObjects.Engagement;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

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
