namespace Eleva.Shared.PersistenceObjects.Core;

using Eleva.Shared.Interfaces;
using Eleva.Shared.Enums;

public class InstancePO : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string McpApiKey { get; set; } = null!;
    public SubscriptionPlan Plan { get; set; }
    public InstanceStatus Status { get; set; }
    public bool IsActive { get; set; }
    public int MaxUsers { get; set; }
    public string? SettingsJson { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
