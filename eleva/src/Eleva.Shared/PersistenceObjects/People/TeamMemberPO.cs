namespace Eleva.Shared.PersistenceObjects.People;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class TeamMemberPO : BaseEntity
{
    public int TeamId { get; set; }
    public int EmployeeId { get; set; }
    public string? RoleInTeam { get; set; }
    public DateOnly JoinedAt { get; set; }
    public DateOnly? LeftAt { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
