namespace Eleva.Shared.PersistenceObjects.Assessments;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class DiscResponsePO : BaseEntity
{
    public int DiscAssessmentId { get; set; }
    public string QuestionCode { get; set; } = null!;
    public string? AnswerValue { get; set; }
    public string? Axis { get; set; }
    public DateTime AnsweredAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
