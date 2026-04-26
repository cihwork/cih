namespace Eleva.Shared.PersistenceObjects.AICopilot;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class AiInteractionPO : BaseEntity
{
    public int? EmployeeId { get; set; }
    public AiContextType ContextType { get; set; }
    public int? ContextId { get; set; }
    public string Prompt { get; set; } = null!;
    public string? PromptHash { get; set; }
    public string? ResponseSummary { get; set; }
    public string? ModelUsed { get; set; }
    public int? TokensIn { get; set; }
    public int? TokensOut { get; set; }
    public decimal? ConfidenceScore { get; set; }
    public int? CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
