namespace Eleva.Services.Services.Assessments;

using Eleva.Shared.Enums;
using Eleva.Shared.PersistenceObjects.Assessments;

public interface ICompetencyService
{
    Task<IReadOnlyList<CompetencyPO>> ListAsync(int instanceId, CompetencyCategory? category = null, string? search = null);
    Task<CompetencyPO?> GetAsync(int instanceId, int competencyId);
    Task<CompetencyPO> CreateAsync(int instanceId, CompetencyPO competency);
    Task<CompetencyPO> UpdateAsync(int instanceId, CompetencyPO competency);
    Task<IReadOnlyList<CompetencyLibraryPO>> ListLibrariesAsync(int instanceId);
}
