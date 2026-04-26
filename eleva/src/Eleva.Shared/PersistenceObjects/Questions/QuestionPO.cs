namespace Eleva.Shared.PersistenceObjects.Questions;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class QuestionPO : BaseEntity
{
    public int QuestionBankId { get; set; }
    public string Code { get; set; } = null!;
    public string Text { get; set; } = null!;
    public string? HelpText { get; set; }
    public QuestionType QuestionType { get; set; }
    public AnswerFormat AnswerFormat { get; set; }
    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }
    public string? MetadataJson { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public List<AnswerPO> Answers { get; set; } = new();
}
