namespace Eleva.Shared.PersistenceObjects.Pdi;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class PdiActionPO : BaseEntity
{
    public int PdiGoalId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public PdiActionType ActionType { get; set; }
    public decimal? EstimatedHours { get; set; }
    public decimal? ActualHours { get; set; }
    public DateOnly? DueDate { get; set; }
    public DateOnly? CompletedDate { get; set; }
    public PdiActionStatus Status { get; set; }
    public string? ResourcesNeeded { get; set; }
    public string? EvidenceUrl { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
