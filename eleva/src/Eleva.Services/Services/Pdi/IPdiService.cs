namespace Eleva.Services.Services.Pdi;

using Eleva.Shared.Enums;
using Eleva.Shared.PersistenceObjects.Pdi;

public interface IPdiService
{
    Task<PdiPlanPO> CreatePlanAsync(int instanceId, PdiPlanPO plan, IEnumerable<PdiGoalPO>? goals = null);
    Task<PdiPlanPO?> GetAsync(int instanceId, int pdiPlanId);
    Task<IReadOnlyList<PdiPlanPO>> ListAsync(int instanceId, int? employeeId = null, PdiStatus? status = null, string? cycle = null);
    Task<PdiGoalPO> CreateGoalAsync(int instanceId, int pdiPlanId, PdiGoalPO goal);
    Task<PdiActionPO> CreateActionAsync(int instanceId, int pdiGoalId, PdiActionPO action);
    Task<PdiCheckpointPO> CreateCheckpointAsync(int instanceId, int pdiPlanId, int pdiGoalId, PdiCheckpointPO checkpoint);
    Task<PdiEvidencePO> UploadEvidenceAsync(int instanceId, int pdiActionId, PdiEvidencePO evidence);
    Task<object> GetProgressSummaryAsync(int instanceId, int pdiPlanId);
}
