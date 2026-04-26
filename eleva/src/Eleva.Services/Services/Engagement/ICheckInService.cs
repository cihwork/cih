namespace Eleva.Services.Services.Engagement;

using Eleva.Shared.PersistenceObjects.Engagement;

public interface ICheckInService
{
    Task<CheckInPO> CreateAsync(int instanceId, CheckInPO checkIn);
    Task<IReadOnlyList<CheckInPO>> ListAsync(int instanceId, int? employeeId = null, DateTime? from = null, DateTime? to = null);
}
