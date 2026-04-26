namespace Eleva.Services.Services.Engagement;

using Eleva.Shared.Enums;
using Eleva.Shared.PersistenceObjects.Engagement;

public interface IFeedbackService
{
    Task<FeedbackPO> CreateAsync(int instanceId, FeedbackPO feedback);
    Task<IReadOnlyList<FeedbackPO>> ListAsync(int instanceId, int? employeeId = null, FeedbackType? type = null, FeedbackVisibility? visibility = null);
}
