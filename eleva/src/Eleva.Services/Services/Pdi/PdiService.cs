namespace Eleva.Services.Services.Pdi;

using Eleva.Services.Data;
using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;
using Eleva.Shared.PersistenceObjects.Pdi;
using Microsoft.EntityFrameworkCore;

public class PdiService : IPdiService
{
    private readonly AppDbContext _db;
    private readonly ICurrentInstanceAccessor _instanceAccessor;

    public PdiService(AppDbContext db, ICurrentInstanceAccessor instanceAccessor)
    {
        _db = db;
        _instanceAccessor = instanceAccessor;
    }

    public async Task<PdiPlanPO> CreatePlanAsync(int instanceId, PdiPlanPO plan, IEnumerable<PdiGoalPO>? goals = null)
    {
        await _db.PdiPlans.AddAsync(plan);
        await _db.SaveChangesAsync();

        if (goals != null)
        {
            foreach (var goal in goals)
            {
                goal.PdiPlanId = plan.Id;
                await _db.PdiGoals.AddAsync(goal);
            }
            await _db.SaveChangesAsync();
        }

        return plan;
    }

    public async Task<PdiPlanPO?> GetAsync(int instanceId, int pdiPlanId)
    {
        return await _db.PdiPlans
            .FirstOrDefaultAsync(p => p.Id == pdiPlanId && p.InstanceId == instanceId && p.DeletedAt == null);
    }

    public async Task<IReadOnlyList<PdiPlanPO>> ListAsync(int instanceId, int? employeeId = null, PdiStatus? status = null, string? cycle = null)
    {
        var query = _db.PdiPlans.Where(p => p.InstanceId == instanceId && p.DeletedAt == null);

        if (employeeId.HasValue)
            query = query.Where(p => p.EmployeeId == employeeId.Value);

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        if (cycle != null)
        {
            var cycleIds = await _db.PdiCycles
                .Where(c => c.InstanceId == instanceId && c.CycleName == cycle && c.DeletedAt == null)
                .Select(c => c.Id)
                .ToListAsync();

            query = query.Where(p => cycleIds.Contains(p.PdiCycleId));
        }

        return await query.ToListAsync();
    }

    public async Task<PdiGoalPO> CreateGoalAsync(int instanceId, int pdiPlanId, PdiGoalPO goal)
    {
        goal.PdiPlanId = pdiPlanId;
        await _db.PdiGoals.AddAsync(goal);
        await _db.SaveChangesAsync();
        return goal;
    }

    public async Task<PdiActionPO> CreateActionAsync(int instanceId, int pdiGoalId, PdiActionPO action)
    {
        action.PdiGoalId = pdiGoalId;
        await _db.PdiActions.AddAsync(action);
        await _db.SaveChangesAsync();
        return action;
    }

    public async Task<PdiCheckpointPO> CreateCheckpointAsync(int instanceId, int pdiPlanId, int pdiGoalId, PdiCheckpointPO checkpoint)
    {
        checkpoint.PdiPlanId = pdiPlanId;
        checkpoint.PdiGoalId = pdiGoalId;
        await _db.PdiCheckpoints.AddAsync(checkpoint);
        await _db.SaveChangesAsync();
        await RecalculateProgressAsync(instanceId, pdiPlanId);
        return checkpoint;
    }

    public async Task<PdiEvidencePO> UploadEvidenceAsync(int instanceId, int pdiActionId, PdiEvidencePO evidence)
    {
        evidence.PdiActionId = pdiActionId;
        evidence.UploadedAt = DateTime.UtcNow;
        await _db.PdiEvidences.AddAsync(evidence);
        await _db.SaveChangesAsync();

        var pdiPlanId = await _db.PdiActions
            .Where(a => a.Id == pdiActionId && a.InstanceId == instanceId)
            .Join(_db.PdiGoals, a => a.PdiGoalId, g => g.Id, (a, g) => g.PdiPlanId)
            .FirstOrDefaultAsync();
        if (pdiPlanId != 0)
            await RecalculateProgressAsync(instanceId, pdiPlanId);

        return evidence;
    }

    private async Task RecalculateProgressAsync(int instanceId, int pdiPlanId)
    {
        var goals = await _db.PdiGoals
            .Where(g => g.InstanceId == instanceId && g.PdiPlanId == pdiPlanId && g.DeletedAt == null)
            .ToListAsync();

        if (goals.Count == 0) return;

        var goalIds = goals.Select(g => g.Id).ToList();
        var allActions = await _db.PdiActions
            .Where(a => a.InstanceId == instanceId && goalIds.Contains(a.PdiGoalId) && a.DeletedAt == null)
            .ToListAsync();

        var now = DateTime.UtcNow;
        decimal planProgressSum = 0m;

        foreach (var goal in goals)
        {
            var goalActions = allActions.Where(a => a.PdiGoalId == goal.Id).ToList();
            var goalProgress = goalActions.Count == 0
                ? 0m
                : (decimal)goalActions.Count(a => a.Status == PdiActionStatus.Completed) / goalActions.Count * 100m;
            goal.Progress = goalProgress;
            goal.UpdatedAt = now;
            planProgressSum += goalProgress;
        }

        var plan = await _db.PdiPlans.FirstOrDefaultAsync(p => p.Id == pdiPlanId && p.InstanceId == instanceId);
        if (plan != null)
        {
            plan.Progress = planProgressSum / goals.Count;
            plan.UpdatedAt = now;
        }

        await _db.SaveChangesAsync();
    }

    public async Task<object> GetProgressSummaryAsync(int instanceId, int pdiPlanId)
    {
        var goals = await _db.PdiGoals
            .Where(g => g.InstanceId == instanceId && g.PdiPlanId == pdiPlanId && g.DeletedAt == null)
            .ToListAsync();

        var goalIds = goals.Select(g => g.Id).ToList();

        var actions = await _db.PdiActions
            .Where(a => a.InstanceId == instanceId && goalIds.Contains(a.PdiGoalId) && a.DeletedAt == null)
            .ToListAsync();

        return new
        {
            TotalGoals = goals.Count,
            GoalsByStatus = goals
                .GroupBy(g => g.Status)
                .ToDictionary(g => g.Key.ToString(), g => g.Count()),
            TotalActions = actions.Count,
            ActionsByStatus = actions
                .GroupBy(a => a.Status)
                .ToDictionary(a => a.Key.ToString(), a => a.Count())
        };
    }
}
