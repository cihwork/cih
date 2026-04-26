namespace Eleva.Shared.PersistenceObjects.History;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class ChangeLogPO : BaseEntity
{
    public string EntityType { get; set; } = null!;
    public string EntityId { get; set; } = null!;
    public string Action { get; set; } = null!;
    public string? BeforeJson { get; set; }
    public string? AfterJson { get; set; }
    public int? ChangedById { get; set; }
    public DateTime ChangedAt { get; set; }
    public ChangeSource Source { get; set; }
    public string? CorrelationId { get; set; }
}
