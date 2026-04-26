namespace Eleva.Shared.PersistenceObjects.Performance;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class ReviewResponsePO : BaseEntity
{
    public int PerformanceReviewId { get; set; }
    public int? QuestionId { get; set; }
    public string? AnswerText { get; set; }
    public decimal? Score { get; set; }
    public string? EvidenceJson { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
