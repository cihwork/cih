namespace Eleva.Services.Services.Engagement;

using Eleva.Shared.PersistenceObjects.Engagement;

public interface IPulseService
{
    Task<PulseSurveyPO> CreateAsync(int instanceId, PulseSurveyPO survey, IEnumerable<PulseQuestionPO>? questions = null);
    Task<PulseResponsePO> RespondAsync(int instanceId, int surveyId, PulseResponsePO response);
    Task<object> GetResultsAsync(int instanceId, int surveyId, int? departmentId = null);
}
