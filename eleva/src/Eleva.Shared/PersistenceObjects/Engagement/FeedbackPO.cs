namespace Eleva.Shared.PersistenceObjects.Engagement;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class FeedbackPO : BaseEntity
{
    public int FromEmployeeId { get; set; }
    public int ToEmployeeId { get; set; }
    public int? OneOnOneId { get; set; }
    public string? Context { get; set; }
    public string Content { get; set; } = null!;
    public FeedbackType FeedbackType { get; set; }
    public SentimentLevel? Sentiment { get; set; }
    public int? CompetencyId { get; set; }
    public bool IsPrivate { get; set; }
    public FeedbackVisibility Visibility { get; set; }
    public DateTime? DeliveredDate { get; set; }
    public FeedbackRecordStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
