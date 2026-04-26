namespace Eleva.Server.Jobs;

using Eleva.Services.Data;
using Eleva.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

[DisallowConcurrentExecution]
public class PdiNotificationJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PdiNotificationJob> _logger;

    public PdiNotificationJob(IServiceScopeFactory scopeFactory, ILogger<PdiNotificationJob> logger)
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
        var limit = today.AddDays(7);

        var plans = await db.PdiPlans
            .Where(p => p.Status == PdiStatus.Active && p.EndDate != null && p.EndDate >= today && p.EndDate <= limit)
            .ToListAsync(context.CancellationToken);

        foreach (var plan in plans)
        {
            _logger.LogWarning("[PdiNotificationJob] Plano {PlanId} (Employee {EmployeeId}) vence em {DueDate}.",
                plan.Id, plan.EmployeeId, plan.EndDate);
        }

        _logger.LogInformation("[PdiNotificationJob] {Count} planos com prazo nos próximos 7 dias.", plans.Count);
    }
}
