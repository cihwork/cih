namespace Eleva.Server.Jobs;

using Eleva.Services.Data;
using Eleva.Shared.Enums;
using Eleva.Shared.PersistenceObjects.AICopilot;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

[DisallowConcurrentExecution]
public class AiRecommendationJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AiRecommendationJob> _logger;

    public AiRecommendationJob(IServiceScopeFactory scopeFactory, ILogger<AiRecommendationJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.DisableInstanceFilters = true;
        db.DisableInstanceIdInterceptor = true;

        var cutoff = DateTime.UtcNow.AddDays(-30);

        var employees = await db.Employees
            .Where(e => e.Status == EmploymentStatus.Active)
            .Take(100)
            .ToListAsync(context.CancellationToken);

        var employeeIds = employees.Select(e => e.Id).ToList();

        var recentRecs = await db.AiRecommendations
            .Where(r => r.EmployeeId != null && employeeIds.Contains(r.EmployeeId.Value) && r.CreatedAt >= cutoff)
            .Select(r => r.EmployeeId!.Value)
            .Distinct()
            .ToListAsync(context.CancellationToken);

        var newRecs = new List<AiRecommendationPO>();
        foreach (var employee in employees)
        {
            if (recentRecs.Contains(employee.Id))
                continue;

            newRecs.Add(new AiRecommendationPO
            {
                InstanceId = employee.InstanceId,
                EmployeeId = employee.Id,
                ContextType = AiContextType.Pdi,
                RecommendationType = AiRecommendationType.ActionItem,
                Title = "Revisão Periódica de PDI",
                Description = "Revisão periódica de PDI recomendada",
                Status = AiRecommendationStatus.Pending,
            });
        }

        if (newRecs.Count > 0)
        {
            db.AiRecommendations.AddRange(newRecs);
            await db.SaveChangesAsync(context.CancellationToken);
        }

        _logger.LogInformation("[AiRecommendationJob] {Count} recomendações criadas.", newRecs.Count);
    }
}
