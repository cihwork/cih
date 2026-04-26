namespace Eleva.Services.Services.History;

using Eleva.Services.Data;
using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;
using Eleva.Shared.PersistenceObjects.History;
using Microsoft.EntityFrameworkCore;

public class AuditLogService : IAuditLogService
{
    private readonly AppDbContext _db;
    private readonly ICurrentInstanceAccessor _instanceAccessor;

    public AuditLogService(AppDbContext db, ICurrentInstanceAccessor instanceAccessor)
    {
        _db = db;
        _instanceAccessor = instanceAccessor;
    }

    public async Task<IReadOnlyList<AuditLogPO>> ListAsync(int instanceId, int skip = 0, int take = 50, string? entityType = null, string? action = null, int? userId = null)
    {
        var query = _db.AuditLogs.Where(a => a.InstanceId == instanceId);

        if (!string.IsNullOrEmpty(entityType))
            query = query.Where(a => a.EntityType == entityType);

        if (!string.IsNullOrEmpty(action))
            query = query.Where(a => a.Action == action);

        if (userId.HasValue)
            query = query.Where(a => a.UserId == userId.Value);

        return await query
            .OrderByDescending(a => a.Timestamp)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<AuditLogPO>> SearchAsync(int instanceId, string term, int take = 50)
        => await _db.AuditLogs
            .Where(a => a.InstanceId == instanceId &&
                (a.EntityType.Contains(term) || a.Action.Contains(term) || a.EntityId.Contains(term)))
            .OrderByDescending(a => a.Timestamp)
            .Take(take)
            .ToListAsync();

    public async Task LogAsync(int instanceId, string entityType, string entityId, string action, int? userId = null, string? details = null)
    {
        var entry = new AuditLogPO
        {
            InstanceId = instanceId,
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            UserId = userId,
            MetadataJson = details,
            Status = AuditStatus.Success,
            Timestamp = DateTime.UtcNow
        };

        _db.DisableInstanceIdInterceptor = true;
        _db.AuditLogs.Add(entry);
        await _db.SaveChangesAsync();
        _db.DisableInstanceIdInterceptor = false;
    }
}
