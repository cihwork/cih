namespace Eleva.Shared.PersistenceObjects.Assessments;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class DiscAssessmentPO : BaseEntity
{
    public int EmployeeId { get; set; }
    public int? AssessedById { get; set; }
    public AssessmentStatus Status { get; set; }
    public DiscProfile? PrimaryStyle { get; set; }
    public DiscProfile? SecondaryStyle { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? AssessmentLink { get; set; }
    public string? ReportUrl { get; set; }
    public string? RawDataJson { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
