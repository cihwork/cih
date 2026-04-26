namespace Eleva.Shared.PersistenceObjects.Performance;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class BscGoalPO : BaseEntity
{
    public int PerformanceCycleId { get; set; }
    public int PerspectiveId { get; set; }
    public int? DepartmentId { get; set; }
    public int? OwnerId { get; set; }
    public int? CascadedFromGoalId { get; set; }
    public string Code { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public BscStatus Status { get; set; }
    public decimal? Weight { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal? Progress { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
