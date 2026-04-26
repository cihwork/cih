namespace Eleva.Services.Services.People;

using Eleva.Shared.PersistenceObjects.Assessments;

public interface IDiscService
{
    Task<DiscAssessmentPO> CreateAssessmentAsync(int instanceId, int employeeId, string assessmentType);
    Task<DiscAssessmentPO> SubmitAssessmentAsync(int instanceId, int assessmentId, DiscResponsePO[] responses);
    Task<DiscResultPO?> GetResultAsync(int instanceId, int assessmentId);
    Task<object> GetTeamMapAsync(int instanceId, int managerId);
    Task<object> MatchAsync(int instanceId, int employeeId, int targetEmployeeId);
}
