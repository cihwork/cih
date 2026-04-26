namespace Eleva.Shared.PersistenceObjects.Pdi;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class PdiCheckpointPO : BaseEntity
{
    public int PdiPlanId { get; set; }
    public int? PdiGoalId { get; set; }
    public DateOnly CheckpointDate { get; set; }
    public PdiCheckpointType Type { get; set; }
    public PdiCheckpointStatus Status { get; set; }
    public SentimentLevel? OverallSentiment { get; set; }
    public string? ProgressSummary { get; set; }
    public string? Wins { get; set; }
    public string? Challenges { get; set; }
    public string? NextSteps { get; set; }
    public string? CoachNotes { get; set; }
    public int? CompletedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
