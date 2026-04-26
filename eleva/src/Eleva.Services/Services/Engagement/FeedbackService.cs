namespace Eleva.Services.Services.Engagement;

using Eleva.Services.Data;
using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;
using Eleva.Shared.PersistenceObjects.Engagement;
using Microsoft.EntityFrameworkCore;

public class FeedbackService : IFeedbackService
{
    private readonly AppDbContext _db;
    private readonly ICurrentInstanceAccessor _instanceAccessor;

    public FeedbackService(AppDbContext db, ICurrentInstanceAccessor instanceAccessor)
    {
        _db = db;
        _instanceAccessor = instanceAccessor;
    }

    public async Task<FeedbackPO> CreateAsync(int instanceId, FeedbackPO feedback)
    {
        if (feedback.FromEmployeeId == feedback.ToEmployeeId)
            throw new ArgumentException("Não é possível enviar feedback para si mesmo.");

        if (string.IsNullOrWhiteSpace(feedback.Content))
            throw new ArgumentException("Conteúdo do feedback é obrigatório.");

        await _db.Feedbacks.AddAsync(feedback);
        await _db.SaveChangesAsync();
        return feedback;
    }

    public async Task<IReadOnlyList<FeedbackPO>> ListAsync(int instanceId, int? employeeId = null, FeedbackType? type = null, FeedbackVisibility? visibility = null)
    {
        var query = _db.Feedbacks.Where(f => f.InstanceId == instanceId && f.DeletedAt == null);

        if (employeeId.HasValue)
            query = query.Where(f => f.FromEmployeeId == employeeId.Value || f.ToEmployeeId == employeeId.Value);

        if (type.HasValue)
            query = query.Where(f => f.FeedbackType == type.Value);

        if (visibility.HasValue)
            query = query.Where(f => f.Visibility == visibility.Value);

        return await query
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }
}
