namespace Eleva.Services.Services.Performance;

using Eleva.Services.Data;
using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;
using Eleva.Shared.PersistenceObjects.Performance;
using Microsoft.EntityFrameworkCore;

public class BscService : IBscService
{
    private readonly AppDbContext _db;
    private readonly ICurrentInstanceAccessor _instanceAccessor;

    public BscService(AppDbContext db, ICurrentInstanceAccessor instanceAccessor)
    {
        _db = db;
        _instanceAccessor = instanceAccessor;
    }

    public async Task<IReadOnlyList<BscPerspectivePO>> ListPerspectivesAsync(int instanceId)
    {
        return await _db.BscPerspectives
            .Where(p => p.InstanceId == instanceId)
            .ToListAsync();
    }

    public async Task<BscGoalPO> CreateGoalAsync(int instanceId, BscGoalPO goal)
    {
        await _db.BscGoals.AddAsync(goal);
        await _db.SaveChangesAsync();
        return goal;
    }

    public async Task<IReadOnlyList<BscGoalPO>> ListGoalsAsync(int instanceId, int? ownerId = null, BscStatus? status = null, int? perspectiveId = null)
    {
        var query = _db.BscGoals.Where(g => g.InstanceId == instanceId && g.DeletedAt == null);

        if (ownerId.HasValue)
            query = query.Where(g => g.OwnerId == ownerId);
        if (status.HasValue)
            query = query.Where(g => g.Status == status);
        if (perspectiveId.HasValue)
            query = query.Where(g => g.PerspectiveId == perspectiveId);

        return await query.ToListAsync();
    }

    public async Task<BscIndicatorPO> CreateIndicatorAsync(int instanceId, BscIndicatorPO indicator)
    {
        await _db.BscIndicators.AddAsync(indicator);
        await _db.SaveChangesAsync();
        await RecalculateBscGoalProgressAsync(instanceId, indicator.BscGoalId);
        return indicator;
    }

    public async Task<BscIndicatorPO> UpdateIndicatorAsync(int instanceId, BscIndicatorPO indicator)
    {
        var existing = await _db.BscIndicators
            .FirstOrDefaultAsync(i => i.Id == indicator.Id && i.InstanceId == instanceId && i.DeletedAt == null)
            ?? throw new KeyNotFoundException($"Indicator {indicator.Id} not found for instance {instanceId}.");

        if (!string.IsNullOrEmpty(indicator.Code)) existing.Code = indicator.Code;
        if (!string.IsNullOrEmpty(indicator.Name)) existing.Name = indicator.Name;
        if (indicator.Description != null) existing.Description = indicator.Description;
        if (indicator.Formula != null) existing.Formula = indicator.Formula;
        if (indicator.UnitOfMeasure != null) existing.UnitOfMeasure = indicator.UnitOfMeasure;
        if (indicator.BaselineValue.HasValue) existing.BaselineValue = indicator.BaselineValue;
        existing.TargetValue = indicator.TargetValue;
        if (indicator.CurrentValue.HasValue) existing.CurrentValue = indicator.CurrentValue;
        if (indicator.MeasurementFrequency != null) existing.MeasurementFrequency = indicator.MeasurementFrequency;
        if (indicator.DataSource != null) existing.DataSource = indicator.DataSource;
        existing.IsHigherBetter = indicator.IsHigherBetter;
        existing.Status = indicator.Status;
        if (indicator.LastMeasuredDate.HasValue) existing.LastMeasuredDate = indicator.LastMeasuredDate;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        await RecalculateBscGoalProgressAsync(instanceId, existing.BscGoalId);
        return existing;
    }

    private async Task RecalculateBscGoalProgressAsync(int instanceId, int goalId)
    {
        var goal = await _db.BscGoals
            .FirstOrDefaultAsync(g => g.Id == goalId && g.InstanceId == instanceId && g.DeletedAt == null);

        if (goal is null) return;

        var indicators = await _db.BscIndicators
            .Where(i => i.BscGoalId == goalId && i.InstanceId == instanceId && i.DeletedAt == null && i.TargetValue != 0)
            .ToListAsync();

        if (indicators.Count == 0)
        {
            goal.Progress = 0;
        }
        else
        {
            var progressValues = indicators
                .Where(i => i.CurrentValue.HasValue)
                .Select(i => Math.Clamp(i.CurrentValue!.Value / i.TargetValue * 100m, 0m, 100m))
                .ToList();

            goal.Progress = progressValues.Count > 0
                ? progressValues.Average()
                : 0;
        }

        goal.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}
