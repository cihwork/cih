namespace Eleva.Services.Services.Pdi;

using Eleva.Services.Data;
using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;
using Eleva.Shared.PersistenceObjects.Pdi;
using Microsoft.EntityFrameworkCore;

public class PdiCycleService : IPdiCycleService
{
    private readonly AppDbContext _db;
    private readonly ICurrentInstanceAccessor _instanceAccessor;

    public PdiCycleService(AppDbContext db, ICurrentInstanceAccessor instanceAccessor)
    {
        _db = db;
        _instanceAccessor = instanceAccessor;
    }

    public async Task<PdiCyclePO> CreateAsync(int instanceId, PdiCyclePO cycle)
    {
        await _db.PdiCycles.AddAsync(cycle);
        await _db.SaveChangesAsync();
        return cycle;
    }

    public async Task<IReadOnlyList<PdiCyclePO>> ListAsync(int instanceId, int? employeeId = null, CycleStatus? status = null)
    {
        var query = _db.PdiCycles.Where(c => c.InstanceId == instanceId && c.DeletedAt == null);

        if (employeeId.HasValue)
            query = query.Where(c => c.EmployeeId == employeeId.Value);

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        return await query.ToListAsync();
    }

    public async Task<PdiCyclePO?> GetAsync(int instanceId, int cycleId)
    {
        return await _db.PdiCycles
            .FirstOrDefaultAsync(c => c.Id == cycleId && c.InstanceId == instanceId && c.DeletedAt == null);
    }

    public async Task<PdiCyclePO> UpdateStatusAsync(int instanceId, int cycleId, CycleStatus status)
    {
        var cycle = await _db.PdiCycles
            .FirstOrDefaultAsync(c => c.Id == cycleId && c.InstanceId == instanceId && c.DeletedAt == null)
            ?? throw new InvalidOperationException($"PdiCycle {cycleId} not found.");

        cycle.Status = status;
        cycle.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return cycle;
    }
}
