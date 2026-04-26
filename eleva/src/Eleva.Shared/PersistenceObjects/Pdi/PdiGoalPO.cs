namespace Eleva.Shared.PersistenceObjects.Pdi;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class PdiGoalPO : BaseEntity
{
    public int PdiPlanId { get; set; }
    public int? CompetencyId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public PdiGoalType GoalType { get; set; }
    public int? CurrentLevel { get; set; }
    public int? TargetLevel { get; set; }
    public PdiPriority Priority { get; set; }
    public PdiGoalStatus Status { get; set; }
    public decimal? Progress { get; set; }
    public DateOnly? Deadline { get; set; }
    public DateOnly? CompletedDate { get; set; }
    public string? SuccessCriteria { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
