namespace Eleva.Server.Jobs;

using Eleva.Services.Data;
using Eleva.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

[DisallowConcurrentExecution]
public class PulseSurveyJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PulseSurveyJob> _logger;

    public PulseSurveyJob(IServiceScopeFactory scopeFactory, ILogger<PulseSurveyJob> logger)
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

        var surveys = await db.PulseSurveys
            .Where(s => s.Status != SurveyStatus.Closed && s.EndDate != null && s.EndDate < today)
            .ToListAsync(context.CancellationToken);

        foreach (var survey in surveys)
            survey.Status = SurveyStatus.Closed;

        if (surveys.Count > 0)
            await db.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("[PulseSurveyJob] {Count} surveys encerrados.", surveys.Count);
    }
}
