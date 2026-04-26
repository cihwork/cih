namespace Eleva.Services.Services.AICopilot;

using Eleva.Shared.PersistenceObjects.AICopilot;
using Eleva.Shared.PersistenceObjects.Questions;

public interface IAiRecommendationService
{
    Task<IReadOnlyList<AiRecommendationPO>> RecommendPdiAsync(int instanceId, int employeeId, int? competencyId = null);
    Task<IReadOnlyList<AiRecommendationPO>> DiscInsightsAsync(int instanceId, int employeeId);
    Task<object> TeamAnalysisAsync(int instanceId, int managerId, string? period = null);
    Task<object> LeadershipTipsAsync(int instanceId, int leaderId, string? context = null);
    Task<IReadOnlyList<QuestionPO>> QuestionSuggestAsync(int instanceId, int? employeeId = null, int? contextId = null);
}
