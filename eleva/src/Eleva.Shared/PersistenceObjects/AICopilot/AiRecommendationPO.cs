namespace Eleva.Shared.PersistenceObjects.AICopilot;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class AiRecommendationPO : BaseEntity
{
    public int? EmployeeId { get; set; }
    public AiContextType ContextType { get; set; }
    public int? ContextId { get; set; }
    public AiRecommendationType RecommendationType { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? Rationale { get; set; }
    public string? SuggestedActionsJson { get; set; }
    public string? ModelUsed { get; set; }
    public decimal? ConfidenceScore { get; set; }
    public AiRecommendationStatus Status { get; set; }
    public int? ReviewedById { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
