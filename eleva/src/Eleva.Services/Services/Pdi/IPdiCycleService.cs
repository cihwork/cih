namespace Eleva.Services.Services.Pdi;

using Eleva.Shared.Enums;
using Eleva.Shared.PersistenceObjects.Pdi;

public interface IPdiCycleService
{
    Task<PdiCyclePO> CreateAsync(int instanceId, PdiCyclePO cycle);
    Task<IReadOnlyList<PdiCyclePO>> ListAsync(int instanceId, int? employeeId = null, CycleStatus? status = null);
    Task<PdiCyclePO?> GetAsync(int instanceId, int cycleId);
    Task<PdiCyclePO> UpdateStatusAsync(int instanceId, int cycleId, CycleStatus status);
}
