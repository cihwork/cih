namespace Eleva.Shared.PersistenceObjects.Core;

using Eleva.Shared.Interfaces;
using Eleva.Shared.Enums;

public class ConfigurationPO : BaseEntity
{
    public string Key { get; set; } = null!;
    public string? Value { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
    public bool IsSecret { get; set; }
    public bool IsEncrypted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
