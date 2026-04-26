namespace Eleva.Services.Services.Engagement;

using Eleva.Services.Data;
using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;
using Eleva.Shared.PersistenceObjects.Engagement;
using Eleva.Shared.PersistenceObjects.History;
using Microsoft.EntityFrameworkCore;

public class CheckInService : ICheckInService
{
    private readonly AppDbContext _db;
    private readonly ICurrentInstanceAccessor _instanceAccessor;

    public CheckInService(AppDbContext db, ICurrentInstanceAccessor instanceAccessor)
    {
        _db = db;
        _instanceAccessor = instanceAccessor;
    }

    public async Task<CheckInPO> CreateAsync(int instanceId, CheckInPO checkIn)
    {
        if (checkIn.Date == default)
            checkIn.Date = DateTime.UtcNow;

        await _db.CheckIns.AddAsync(checkIn);
        await _db.SaveChangesAsync();

        if (checkIn.SupportNeeded == true)
        {
            var alert = new AuditLogPO
            {
                InstanceId = instanceId,
                EntityType = "CheckIn",
                EntityId = checkIn.Id.ToString(),
                Action = "SupportNeeded",
                MetadataJson = $"{{\"message\":\"Employee {checkIn.EmployeeId} flagged support needed at {DateTime.UtcNow:O}\"}}",
                Status = AuditStatus.Success,
                Timestamp = DateTime.UtcNow
            };
            _db.AuditLogs.Add(alert);
            await _db.SaveChangesAsync();
        }

        return checkIn;
    }

    public async Task<IReadOnlyList<CheckInPO>> ListAsync(int instanceId, int? employeeId = null, DateTime? from = null, DateTime? to = null)
    {
        var query = _db.CheckIns.Where(c => c.InstanceId == instanceId);

        if (employeeId.HasValue)
            query = query.Where(c => c.EmployeeId == employeeId.Value);

        if (from.HasValue)
            query = query.Where(c => c.Date >= from.Value);

        if (to.HasValue)
            query = query.Where(c => c.Date <= to.Value);

        return await query
            .OrderByDescending(c => c.Date)
            .ToListAsync();
    }
}
