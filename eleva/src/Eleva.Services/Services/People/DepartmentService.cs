namespace Eleva.Services.Services.People;

using Eleva.Services.Data;
using Eleva.Shared.PersistenceObjects.People;
using Microsoft.EntityFrameworkCore;

public class DepartmentService : IDepartmentService
{
    private readonly AppDbContext _db;

    public DepartmentService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<DepartmentPO>> ListAsync(int instanceId, string? search = null, bool? isActive = null)
    {
        var query = _db.Departments
            .Where(d => d.InstanceId == instanceId && d.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(d => d.Name.Contains(search));

        if (isActive.HasValue)
            query = query.Where(d => d.IsActive == isActive.Value);

        return await query.ToListAsync();
    }

    public async Task<DepartmentPO?> GetAsync(int instanceId, int departmentId)
    {
        return await _db.Departments
            .FirstOrDefaultAsync(d => d.Id == departmentId && d.InstanceId == instanceId && d.DeletedAt == null);
    }

    public async Task<DepartmentPO> CreateAsync(int instanceId, DepartmentPO department)
    {
        _db.Departments.Add(department);
        await _db.SaveChangesAsync();
        return department;
    }

    public async Task<DepartmentPO> UpdateAsync(int instanceId, DepartmentPO department)
    {
        var existing = await _db.Departments
            .FirstOrDefaultAsync(d => d.Id == department.Id && d.InstanceId == instanceId && d.DeletedAt == null)
            ?? throw new KeyNotFoundException($"Department {department.Id} not found.");

        if (department.Code != null) existing.Code = department.Code;
        if (department.Name != null) existing.Name = department.Name;
        if (department.Description != null) existing.Description = department.Description;
        if (department.ParentDepartmentId.HasValue) existing.ParentDepartmentId = department.ParentDepartmentId;
        if (department.ManagerId.HasValue) existing.ManagerId = department.ManagerId;
        existing.Type = department.Type;
        existing.IsActive = department.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return existing;
    }
}
