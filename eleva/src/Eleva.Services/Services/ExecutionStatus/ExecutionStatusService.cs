namespace Eleva.Services.Services.ExecutionStatus;

using Eleva.Services.Data;
using Eleva.Shared.Interfaces;

public class ExecutionStatusService : IExecutionStatusService
{
    private static readonly List<ExecutionStatusEntry> _entries = new();
    private static readonly object _lock = new();

    private readonly AppDbContext _db;
    private readonly ICurrentInstanceAccessor _instanceAccessor;

    public ExecutionStatusService(AppDbContext db, ICurrentInstanceAccessor instanceAccessor)
    {
        _db = db;
        _instanceAccessor = instanceAccessor;
    }

    public Task RecordAsync(string serviceName, string item, string status, string message, string? details = null)
    {
        var entry = new ExecutionStatusEntry(
            ExecutionKey: $"{serviceName}:{item}:{DateTime.UtcNow:yyyyMMddHHmmssfff}",
            ServiceName: serviceName,
            Item: item,
            Status: status,
            Message: message,
            Details: details,
            RecordedAt: DateTime.UtcNow
        );
        lock (_lock)
        {
            _entries.Add(entry);
            if (_entries.Count > 10_000)
                _entries.RemoveRange(0, _entries.Count - 10_000);
        }
        return Task.CompletedTask;
    }

    public Task RecordSuccess(string serviceName, string item, string message, string? details = null)
        => RecordAsync(serviceName, item, "Success", message, details);

    public Task RecordInformation(string serviceName, string item, string message, string? details = null)
        => RecordAsync(serviceName, item, "Information", message, details);

    public Task RecordWarning(string serviceName, string item, string message, string? details = null)
        => RecordAsync(serviceName, item, "Warning", message, details);

    public Task RecordError(string serviceName, string item, string message, string? details = null)
        => RecordAsync(serviceName, item, "Error", message, details);

    public Task<object> GetLastAsync(string? serviceName, string? item, int count)
    {
        List<ExecutionStatusEntry> snapshot;
        lock (_lock) snapshot = _entries.ToList();

        IEnumerable<ExecutionStatusEntry> query = snapshot;
        if (!string.IsNullOrEmpty(serviceName))
            query = query.Where(e => e.ServiceName == serviceName);
        if (!string.IsNullOrEmpty(item))
            query = query.Where(e => e.Item == item);

        var result = query.OrderByDescending(e => e.RecordedAt).Take(count).ToList();
        return Task.FromResult<object>(result);
    }

    public Task<object> GetByExecutionKeyAsync(string executionKey)
    {
        List<ExecutionStatusEntry> snapshot;
        lock (_lock) snapshot = _entries.ToList();

        var result = snapshot.FirstOrDefault(e => e.ExecutionKey == executionKey);
        return Task.FromResult<object>(result!);
    }

    private record ExecutionStatusEntry(
        string ExecutionKey,
        string ServiceName,
        string Item,
        string Status,
        string Message,
        string? Details,
        DateTime RecordedAt
    );
}
