namespace Eleva.Services.Services.Questions;

using Eleva.Services.Data;
using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;
using Eleva.Shared.PersistenceObjects.Questions;
using Microsoft.EntityFrameworkCore;

public class QuestionBankService : IQuestionBankService
{
    private readonly AppDbContext _db;
    private readonly ICurrentInstanceAccessor _instanceAccessor;

    public QuestionBankService(AppDbContext db, ICurrentInstanceAccessor instanceAccessor)
    {
        _db = db;
        _instanceAccessor = instanceAccessor;
    }

    public async Task<IReadOnlyList<QuestionBankPO>> ListBanksAsync(int instanceId)
        => await _db.QuestionBanks
            .Where(b => b.InstanceId == instanceId && b.IsActive && b.DeletedAt == null)
            .OrderBy(b => b.Name)
            .ToListAsync();

    public async Task<IReadOnlyList<QuestionPO>> ListQuestionsAsync(int instanceId, int? bankId = null, string? search = null)
    {
        var query = _db.Questions.Where(q => q.InstanceId == instanceId && q.DeletedAt == null);

        if (bankId.HasValue)
            query = query.Where(q => q.QuestionBankId == bankId.Value);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(q => q.Text.Contains(search) || (q.HelpText != null && q.HelpText.Contains(search)));

        return await query.Include(q => q.Answers).OrderBy(q => q.SortOrder).ToListAsync();
    }

    public async Task<QuestionBankPO> CreateBankAsync(int instanceId, QuestionBankPO bank)
    {
        bank.IsActive = true;
        _db.QuestionBanks.Add(bank);
        await _db.SaveChangesAsync();
        return bank;
    }

    public async Task<QuestionPO> CreateQuestionAsync(int instanceId, QuestionPO question)
    {
        _db.Questions.Add(question);
        await _db.SaveChangesAsync();
        return question;
    }

    public async Task<IReadOnlyList<QuestionPO>> SelectSmartAsync(int instanceId, AiContextType contextType, int? contextId = null, int take = 10)
    {
        var contextName = contextType.ToString();

        var bankQuery = _db.QuestionBanks
            .Where(b => b.InstanceId == instanceId && b.IsActive && b.DeletedAt == null);

        if (contextId.HasValue)
            bankQuery = bankQuery.Where(b => b.Id == contextId.Value);
        else
            bankQuery = bankQuery.Where(b => b.Name.Contains(contextName));

        var activeBankIds = await bankQuery.Select(b => b.Id).ToListAsync();

        if (activeBankIds.Count == 0)
        {
            activeBankIds = await _db.QuestionBanks
                .Where(b => b.InstanceId == instanceId && b.IsActive && b.DeletedAt == null)
                .Select(b => b.Id)
                .ToListAsync();
        }

        return await _db.Questions
            .Include(q => q.Answers)
            .Where(q => q.InstanceId == instanceId && q.DeletedAt == null && activeBankIds.Contains(q.QuestionBankId))
            .OrderByDescending(q => q.CreatedAt)
            .Take(take)
            .ToListAsync();
    }

    public async Task<bool> ValidateCrossAsync(int instanceId, int questionId, int? targetBankId = null)
    {
        var question = await _db.Questions
            .FirstOrDefaultAsync(q => q.Id == questionId && q.InstanceId == instanceId && q.DeletedAt == null);

        if (question is null) return false;

        if (targetBankId.HasValue)
        {
            if (question.QuestionBankId == targetBankId.Value)
                return false;

            var prefix = question.Text.Length >= 50 ? question.Text[..50] : question.Text;
            var similarExists = await _db.Questions
                .AnyAsync(q => q.InstanceId == instanceId
                    && q.QuestionBankId == targetBankId.Value
                    && q.DeletedAt == null
                    && q.Text.StartsWith(prefix));

            if (similarExists) return false;
        }

        return true;
    }
}
