namespace Eleva.Server.Jobs;

using Eleva.Services.Data;
using Eleva.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

[DisallowConcurrentExecution]
public class AssessmentCycleJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AssessmentCycleJob> _logger;

    public AssessmentCycleJob(IServiceScopeFactory scopeFactory, ILogger<AssessmentCycleJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.DisableInstanceFilters = true;

        var cutoff = DateTime.UtcNow.AddDays(-30);

        var expired = await db.DiscAssessments
            .Where(a => a.Status == AssessmentStatus.Pending && a.CreatedAt < cutoff)
            .ToListAsync(context.CancellationToken);

        foreach (var assessment in expired)
            assessment.Status = AssessmentStatus.Expired;

        if (expired.Count > 0)
            await db.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("[AssessmentCycleJob] {Count} assessments expirados.", expired.Count);
    }
}
