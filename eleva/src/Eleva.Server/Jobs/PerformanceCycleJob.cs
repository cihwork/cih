namespace Eleva.Server.Jobs;

using Eleva.Services.Data;
using Eleva.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

[DisallowConcurrentExecution]
public class PerformanceCycleJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PerformanceCycleJob> _logger;

    public PerformanceCycleJob(IServiceScopeFactory scopeFactory, ILogger<PerformanceCycleJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.DisableInstanceFilters = true;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var cycles = await db.PerformanceCycles
            .Where(c => c.EndDate < today && c.Status != CycleStatus.Closed)
            .ToListAsync(context.CancellationToken);

        foreach (var cycle in cycles)
            cycle.Status = CycleStatus.Closed;

        if (cycles.Count > 0)
            await db.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("[PerformanceCycleJob] {Count} ciclos finalizados.", cycles.Count);
    }
}
