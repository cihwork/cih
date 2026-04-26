namespace Eleva.Services.Services.ExecutionStatus;

public interface IExecutionStatusService
{
    Task RecordAsync(string serviceName, string item, string status, string message, string? details = null);
    Task RecordSuccess(string serviceName, string item, string message, string? details = null);
    Task RecordInformation(string serviceName, string item, string message, string? details = null);
    Task RecordWarning(string serviceName, string item, string message, string? details = null);
    Task RecordError(string serviceName, string item, string message, string? details = null);
    Task<object> GetLastAsync(string? serviceName, string? item, int count);
    Task<object> GetByExecutionKeyAsync(string executionKey);
}
