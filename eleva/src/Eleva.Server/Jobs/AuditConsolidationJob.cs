namespace Eleva.Server.Jobs;

using Eleva.Services.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

[DisallowConcurrentExecution]
public class AuditConsolidationJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AuditConsolidationJob> _logger;

    public AuditConsolidationJob(IServiceScopeFactory scopeFactory, ILogger<AuditConsolidationJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.DisableInstanceFilters = true;

        var cutoff = DateTime.UtcNow.AddDays(-90);

        var count = await db.AuditLogs
            .CountAsync(a => a.Timestamp < cutoff, context.CancellationToken);

        _logger.LogInformation("[AuditConsolidationJob] {Count} registros de auditoria anteriores a {Cutoff} aguardam consolidação.",
            count, cutoff);
    }
}
