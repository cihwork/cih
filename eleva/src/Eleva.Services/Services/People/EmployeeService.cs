namespace Eleva.Services.Services.People;

using Eleva.Services.Data;
using Eleva.Shared.Enums;
using Eleva.Shared.PersistenceObjects.History;
using Eleva.Shared.PersistenceObjects.People;
using Microsoft.EntityFrameworkCore;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _db;

    public EmployeeService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<EmployeePO>> ListAsync(int instanceId, string? search = null, int? departmentId = null, int? managerId = null, bool? isActive = null, int skip = 0, int take = 50)
    {
        var query = _db.Employees
            .Where(e => e.InstanceId == instanceId && e.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(e => e.Name.Contains(search) || e.Email.Contains(search));

        if (departmentId.HasValue)
            query = query.Where(e => e.DepartmentId == departmentId.Value);

        if (managerId.HasValue)
            query = query.Where(e => e.ManagerId == managerId.Value);

        if (isActive.HasValue)
        {
            if (isActive.Value)
                query = query.Where(e => e.Status == EmploymentStatus.Active);
            else
                query = query.Where(e => e.Status != EmploymentStatus.Active);
        }

        return await query.Skip(skip).Take(take).ToListAsync();
    }

    public async Task<EmployeePO?> GetAsync(int instanceId, int employeeId)
    {
        return await _db.Employees
            .FirstOrDefaultAsync(e => e.Id == employeeId && e.InstanceId == instanceId && e.DeletedAt == null);
    }

    public async Task<EmployeePO> CreateAsync(int instanceId, EmployeePO employee)
    {
        _db.Employees.Add(employee);
        await _db.SaveChangesAsync();
        return employee;
    }

    public async Task<EmployeePO> UpdateAsync(int instanceId, EmployeePO employee)
    {
        var existing = await _db.Employees
            .FirstOrDefaultAsync(e => e.Id == employee.Id && e.InstanceId == instanceId && e.DeletedAt == null)
            ?? throw new KeyNotFoundException($"Employee {employee.Id} not found.");

        if (employee.Name != null) existing.Name = employee.Name;
        if (employee.Email != null) existing.Email = employee.Email;
        if (employee.DocumentNumber != null) existing.DocumentNumber = employee.DocumentNumber;
        if (employee.Phone != null) existing.Phone = employee.Phone;
        if (employee.AvatarUrl != null) existing.AvatarUrl = employee.AvatarUrl;
        if (employee.UserId.HasValue) existing.UserId = employee.UserId;
        if (employee.DepartmentId.HasValue) existing.DepartmentId = employee.DepartmentId;
        if (employee.PositionId.HasValue) existing.PositionId = employee.PositionId;
        if (employee.ManagerId.HasValue) existing.ManagerId = employee.ManagerId;
        if (employee.HireDate.HasValue) existing.HireDate = employee.HireDate;
        if (employee.BirthDate.HasValue) existing.BirthDate = employee.BirthDate;
        if (employee.PositionTitle != null) existing.PositionTitle = employee.PositionTitle;
        if (employee.Timezone != null) existing.Timezone = employee.Timezone;
        if (employee.Language != null) existing.Language = employee.Language;
        if (employee.SettingsJson != null) existing.SettingsJson = employee.SettingsJson;
        existing.Status = employee.Status;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        await _db.ChangeLogs.AddAsync(new ChangeLogPO
        {
            EntityType = "Employee",
            EntityId = existing.Id.ToString(),
            Action = "Update",
            ChangedAt = DateTime.UtcNow,
            Source = ChangeSource.Api,
        });
        await _db.SaveChangesAsync();

        return existing;
    }

    public async Task<IReadOnlyList<EmployeePO>> GetTeamAsync(int instanceId, int managerId)
    {
        return await _db.Employees
            .Where(e => e.InstanceId == instanceId && e.ManagerId == managerId && e.DeletedAt == null)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<EmployeePO>> GetHierarchyAsync(int instanceId, int employeeId)
    {
        var hierarchy = new List<EmployeePO>();

        var current = await _db.Employees
            .FirstOrDefaultAsync(e => e.Id == employeeId && e.InstanceId == instanceId && e.DeletedAt == null);

        while (current?.ManagerId != null)
        {
            var parentId = current.ManagerId.Value;
            current = await _db.Employees
                .FirstOrDefaultAsync(e => e.Id == parentId && e.InstanceId == instanceId && e.DeletedAt == null);

            if (current != null)
                hierarchy.Add(current);
        }

        return hierarchy;
    }
}
