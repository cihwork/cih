namespace Eleva.Shared.PersistenceObjects.People;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class DepartmentPO : BaseEntity
{
    public int? ParentDepartmentId { get; set; }
    public int? ManagerId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public OrganizationalUnitType Type { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
