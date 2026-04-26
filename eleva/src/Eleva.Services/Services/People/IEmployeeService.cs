namespace Eleva.Services.Services.People;

using Eleva.Shared.PersistenceObjects.People;

public interface IEmployeeService
{
    Task<IReadOnlyList<EmployeePO>> ListAsync(int instanceId, string? search = null, int? departmentId = null, int? managerId = null, bool? isActive = null, int skip = 0, int take = 50);
    Task<EmployeePO?> GetAsync(int instanceId, int employeeId);
    Task<EmployeePO> CreateAsync(int instanceId, EmployeePO employee);
    Task<EmployeePO> UpdateAsync(int instanceId, EmployeePO employee);
    Task<IReadOnlyList<EmployeePO>> GetTeamAsync(int instanceId, int managerId);
    Task<IReadOnlyList<EmployeePO>> GetHierarchyAsync(int instanceId, int employeeId);
}
