namespace Eleva.Shared.PersistenceObjects.Engagement;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

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
