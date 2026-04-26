namespace Eleva.Shared.PersistenceObjects.Performance;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class PerformanceReviewPO : BaseEntity
{
    public int PerformanceCycleId { get; set; }
    public int EmployeeId { get; set; }
    public int ReviewerId { get; set; }
    public int? CalibrationOwnerId { get; set; }
    public string? ReviewType { get; set; }
    public ReviewStatus Status { get; set; }
    public decimal? OverallScore { get; set; }
    public decimal? CalibrationScore { get; set; }
    public string? Comments { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
