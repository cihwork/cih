namespace Eleva.Services.Services.Assessments;

using Eleva.Shared.PersistenceObjects.Assessments;

public interface IEcService
{
    Task<EcAssessmentPO> CreateAssessmentAsync(int instanceId, EcAssessmentPO assessment);
    Task<EcAssessmentPO> SubmitAssessmentAsync(int instanceId, int assessmentId, EcResponsePO[] responses);
    Task<EcResultPO?> GetResultAsync(int instanceId, int assessmentId);
    Task<object> GetOrgMaturityAsync(int instanceId, int? departmentId = null);
}
