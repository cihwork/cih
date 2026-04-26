namespace Eleva.Shared.PersistenceObjects.Assessments;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class CompetencyPO : BaseEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public CompetencyCategory Category { get; set; }
    public LevelType LevelType { get; set; }
    public string? LevelDefinitionsJson { get; set; }
    public bool IsCore { get; set; }
    public int? ParentCompetencyId { get; set; }
    public int? SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
