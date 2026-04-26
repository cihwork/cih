namespace Eleva.Shared.PersistenceObjects.History;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class AuditLogPO : BaseEntity
{
    public int? UserId { get; set; }
    public string Action { get; set; } = null!;
    public string EntityType { get; set; } = null!;
    public string EntityId { get; set; } = null!;
    public string? ChangesJson { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public AuditStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public string? MetadataJson { get; set; }
    public DateTime Timestamp { get; set; }
}
