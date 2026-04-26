namespace Eleva.Services.Services.Performance;

using Eleva.Shared.Enums;
using Eleva.Shared.PersistenceObjects.Performance;

public interface IReviewService
{
    Task<PerformanceReviewPO> CreateAsync(int instanceId, PerformanceReviewPO review, IEnumerable<ReviewResponsePO>? responses = null);
    Task<PerformanceReviewPO> SubmitAsync(int instanceId, int reviewId);
    Task<PerformanceReviewPO> Create360Async(int instanceId, int employeeId, int cycleId, int reviewerId);
    Task<PerformanceCyclePO> CreateCycleAsync(int instanceId, PerformanceCyclePO cycle);
    Task<IReadOnlyList<PerformanceReviewPO>> ListAsync(int instanceId, int? employeeId = null, ReviewStatus? status = null, int? cycleId = null);
}
