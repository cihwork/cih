namespace Eleva.Shared.PersistenceObjects.People;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class TeamPO : BaseEntity
{
    public int? DepartmentId { get; set; }
    public int? LeadId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
