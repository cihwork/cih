namespace Eleva.Shared.PersistenceObjects.Engagement;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class CheckInPO : BaseEntity
{
    public int EmployeeId { get; set; }
    public int? ManagerId { get; set; }
    public DateTime Date { get; set; }
    public SentimentLevel Mood { get; set; }
    public int EnergyLevel { get; set; }
    public int ProductivityScore { get; set; }
    public string? Notes { get; set; }
    public string? Blockers { get; set; }
    public bool SupportNeeded { get; set; }
    public OneOnOneStatus? Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
