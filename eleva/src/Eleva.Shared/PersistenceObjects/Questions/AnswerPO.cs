namespace Eleva.Shared.PersistenceObjects.Questions;

using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;

public class AnswerPO : BaseEntity
{
    public int QuestionId { get; set; }
    public int? EmployeeId { get; set; }
    public string? TextValue { get; set; }
    public decimal? NumericValue { get; set; }
    public bool? BooleanValue { get; set; }
    public string? JsonValue { get; set; }
    public decimal? Score { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
