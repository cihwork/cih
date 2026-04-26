namespace Eleva.Shared.PersistenceObjects.Pdi;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class PdiPlanPO : BaseEntity
{
    public int PdiCycleId { get; set; }
    public int EmployeeId { get; set; }
    public int? CoachId { get; set; }
    public int? ManagerId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public PdiStatus Status { get; set; }
    public decimal? Progress { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Context { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
