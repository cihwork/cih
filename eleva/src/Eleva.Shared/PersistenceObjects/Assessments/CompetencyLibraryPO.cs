namespace Eleva.Shared.PersistenceObjects.Assessments;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class CompetencyLibraryPO : BaseEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Version { get; set; } = "1.0";
    public string? Source { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
