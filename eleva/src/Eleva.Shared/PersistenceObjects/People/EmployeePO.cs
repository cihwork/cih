namespace Eleva.Shared.PersistenceObjects.People;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class EmployeePO : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? DocumentNumber { get; set; }
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public int? UserId { get; set; }
    public int? DepartmentId { get; set; }
    public int? PositionId { get; set; }
    public int? ManagerId { get; set; }
    public DateOnly? HireDate { get; set; }
    public DateOnly? BirthDate { get; set; }
    public EmploymentStatus Status { get; set; }
    public string? PositionTitle { get; set; }
    public string? Timezone { get; set; }
    public string? Language { get; set; }
    public string? SettingsJson { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
