namespace Eleva.Shared.PersistenceObjects.Performance;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class BscIndicatorPO : BaseEntity
{
    public int BscGoalId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Formula { get; set; }
    public string? UnitOfMeasure { get; set; }
    public decimal? BaselineValue { get; set; }
    public decimal TargetValue { get; set; }
    public decimal? CurrentValue { get; set; }
    public string? MeasurementFrequency { get; set; }
    public string? DataSource { get; set; }
    public bool IsHigherBetter { get; set; }
    public IndicatorStatus Status { get; set; }
    public DateOnly? LastMeasuredDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
