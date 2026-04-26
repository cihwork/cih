namespace Eleva.Services.Services.Questions;

using Eleva.Shared.Enums;
using Eleva.Shared.PersistenceObjects.Questions;

public interface IQuestionBankService
{
    Task<IReadOnlyList<QuestionBankPO>> ListBanksAsync(int instanceId);
    Task<IReadOnlyList<QuestionPO>> ListQuestionsAsync(int instanceId, int? bankId = null, string? search = null);
    Task<QuestionBankPO> CreateBankAsync(int instanceId, QuestionBankPO bank);
    Task<QuestionPO> CreateQuestionAsync(int instanceId, QuestionPO question);
    Task<IReadOnlyList<QuestionPO>> SelectSmartAsync(int instanceId, AiContextType contextType, int? contextId = null, int take = 10);
    Task<bool> ValidateCrossAsync(int instanceId, int questionId, int? targetBankId = null);
}
