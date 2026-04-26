namespace Eleva.Shared.PersistenceObjects.Engagement;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class MoodEntryPO : BaseEntity
{
    public int EmployeeId { get; set; }
    public DateTime EntryAt { get; set; }
    public SentimentLevel Mood { get; set; }
    public int EnergyLevel { get; set; }
    public string? Notes { get; set; }
    public string? Source { get; set; }
    public DateTime CreatedAt { get; set; }
}
