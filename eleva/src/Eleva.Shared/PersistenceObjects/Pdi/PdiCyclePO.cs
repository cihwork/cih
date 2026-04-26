namespace Eleva.Shared.PersistenceObjects.Pdi;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class PdiCyclePO : BaseEntity
{
    public int EmployeeId { get; set; }
    public int? CoachId { get; set; }
    public int? ManagerId { get; set; }
    public string CycleName { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public CycleStatus Status { get; set; }
    public decimal? OverallProgress { get; set; }
    public string? FocusAreasJson { get; set; }
    public string? Context { get; set; }
    public DateOnly? KickoffDate { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
