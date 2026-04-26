namespace Eleva.Services.Services.People;

using Eleva.Shared.PersistenceObjects.People;

public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentPO>> ListAsync(int instanceId, string? search = null, bool? isActive = null);
    Task<DepartmentPO?> GetAsync(int instanceId, int departmentId);
    Task<DepartmentPO> CreateAsync(int instanceId, DepartmentPO department);
    Task<DepartmentPO> UpdateAsync(int instanceId, DepartmentPO department);
}
