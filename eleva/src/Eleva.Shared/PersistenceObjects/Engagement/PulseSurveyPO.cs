namespace Eleva.Shared.PersistenceObjects.Engagement;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class PulseSurveyPO : BaseEntity
{
    public int? OrganizationalUnitId { get; set; }
    public string Title { get; set; } = null!;
    public PulseType Type { get; set; }
    public PulseFrequency Frequency { get; set; }
    public string QuestionsJson { get; set; } = null!;
    public bool IsAnonymous { get; set; }
    public SurveyStatus Status { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
