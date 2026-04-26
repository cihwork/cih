namespace Eleva.Shared.PersistenceObjects.People;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class PositionPO : BaseEntity
{
    public int? DepartmentId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Level { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
