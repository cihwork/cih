namespace Eleva.Services.Services.Assessments;

using Eleva.Services.Data;
using Eleva.Shared.Enums;
using Eleva.Shared.Interfaces;
using Eleva.Shared.PersistenceObjects.Assessments;
using Microsoft.EntityFrameworkCore;

public class CompetencyService : ICompetencyService
{
    private readonly AppDbContext _db;
    private readonly ICurrentInstanceAccessor _instanceAccessor;

    public CompetencyService(AppDbContext db, ICurrentInstanceAccessor instanceAccessor)
    {
        _db = db;
        _instanceAccessor = instanceAccessor;
    }

    public async Task<IReadOnlyList<CompetencyPO>> ListAsync(int instanceId, CompetencyCategory? category = null, string? search = null)
    {
        var query = _db.Competencies
            .Where(c => c.InstanceId == instanceId && c.DeletedAt == null);

        if (category.HasValue)
            query = query.Where(c => c.Category == category.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.Name.Contains(search));

        return await query.ToListAsync();
    }

    public async Task<CompetencyPO?> GetAsync(int instanceId, int competencyId)
    {
        return await _db.Competencies
            .FirstOrDefaultAsync(c => c.Id == competencyId && c.InstanceId == instanceId && c.DeletedAt == null);
    }

    public async Task<CompetencyPO> CreateAsync(int instanceId, CompetencyPO competency)
    {
        _db.Competencies.Add(competency);
        await _db.SaveChangesAsync();
        return competency;
    }

    public async Task<CompetencyPO> UpdateAsync(int instanceId, CompetencyPO competency)
    {
        var existing = await _db.Competencies
            .FirstOrDefaultAsync(c => c.Id == competency.Id && c.InstanceId == instanceId && c.DeletedAt == null)
            ?? throw new InvalidOperationException("Competency not found.");

        existing.Code = competency.Code;
        existing.Name = competency.Name;
        existing.Description = competency.Description;
        existing.Category = competency.Category;
        existing.LevelType = competency.LevelType;
        existing.LevelDefinitionsJson = competency.LevelDefinitionsJson;
        existing.IsCore = competency.IsCore;
        existing.ParentCompetencyId = competency.ParentCompetencyId;
        existing.SortOrder = competency.SortOrder;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<IReadOnlyList<CompetencyLibraryPO>> ListLibrariesAsync(int instanceId)
    {
        return await _db.CompetencyLibraries
            .Where(l => l.InstanceId == instanceId)
            .ToListAsync();
    }
}
