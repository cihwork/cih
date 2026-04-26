namespace Eleva.Services.Services.Core;

using Eleva.Services.Data;
using Eleva.Shared.Interfaces;
using Eleva.Shared.PersistenceObjects.Core;
using Microsoft.EntityFrameworkCore;

public class ConfigService : IConfigService
{
    private readonly AppDbContext _db;
    private readonly ICurrentInstanceAccessor _instanceAccessor;

    public ConfigService(AppDbContext db, ICurrentInstanceAccessor instanceAccessor)
    {
        _db = db;
        _instanceAccessor = instanceAccessor;
    }

    public async Task<ConfigurationPO?> GetAsync(int instanceId, string key)
    {
        var config = await _db.Configurations
            .FirstOrDefaultAsync(c => c.InstanceId == instanceId && c.Key == key);

        if (config is not null && (config.IsSecret || config.IsEncrypted))
            config.Value = "***";

        return config;
    }

    public async Task<IReadOnlyList<ConfigurationPO>> ListAsync(int instanceId, string? prefix = null)
    {
        var query = _db.Configurations.Where(c => c.InstanceId == instanceId);
        if (!string.IsNullOrEmpty(prefix))
            query = query.Where(c => c.Key.StartsWith(prefix));
        return await query.OrderBy(c => c.Key).ToListAsync();
    }

    public async Task<ConfigurationPO> SetAsync(int instanceId, string key, string? value, string? type = null, string? description = null)
    {
        var config = await _db.Configurations
            .FirstOrDefaultAsync(c => c.InstanceId == instanceId && c.Key == key);

        if (config is null)
        {
            config = new ConfigurationPO
            {
                Key = key,
                Value = value,
                Type = type,
                Description = description,
                IsSecret = type == "secret",
            };
            _db.Configurations.Add(config);
        }
        else
        {
            config.Value = value;
            if (type is not null)
            {
                config.Type = type;
                if (type == "secret") config.IsSecret = true;
            }
            if (description is not null) config.Description = description;
            config.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        return config;
    }
}
