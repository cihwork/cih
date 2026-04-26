namespace Eleva.Services.Services.History;

using Eleva.Shared.PersistenceObjects.History;

public interface IAuditLogService
{
    Task<IReadOnlyList<AuditLogPO>> ListAsync(int instanceId, int skip = 0, int take = 50, string? entityType = null, string? action = null, int? userId = null);
    Task<IReadOnlyList<AuditLogPO>> SearchAsync(int instanceId, string term, int take = 50);
    Task LogAsync(int instanceId, string entityType, string entityId, string action, int? userId = null, string? details = null);
}
