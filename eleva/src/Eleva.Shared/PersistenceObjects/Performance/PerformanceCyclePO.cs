namespace Eleva.Shared.PersistenceObjects.Performance;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class PerformanceCyclePO : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public CycleStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
