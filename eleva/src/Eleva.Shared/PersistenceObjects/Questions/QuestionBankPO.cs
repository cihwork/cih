namespace Eleva.Shared.PersistenceObjects.Questions;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class QuestionBankPO : BaseEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Version { get; set; } = "1.0";
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
