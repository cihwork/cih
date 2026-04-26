namespace Eleva.Services.Services.Performance;

using Eleva.Shared.Enums;
using Eleva.Shared.PersistenceObjects.Performance;

public interface IBscService
{
    Task<IReadOnlyList<BscPerspectivePO>> ListPerspectivesAsync(int instanceId);
    Task<BscGoalPO> CreateGoalAsync(int instanceId, BscGoalPO goal);
    Task<IReadOnlyList<BscGoalPO>> ListGoalsAsync(int instanceId, int? ownerId = null, BscStatus? status = null, int? perspectiveId = null);
    Task<BscIndicatorPO> CreateIndicatorAsync(int instanceId, BscIndicatorPO indicator);
    Task<BscIndicatorPO> UpdateIndicatorAsync(int instanceId, BscIndicatorPO indicator);
}
