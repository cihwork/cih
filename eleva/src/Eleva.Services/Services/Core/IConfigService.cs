namespace Eleva.Services.Services.Core;

using Eleva.Shared.PersistenceObjects.Core;

public interface IConfigService
{
    Task<ConfigurationPO?> GetAsync(int instanceId, string key);
    Task<IReadOnlyList<ConfigurationPO>> ListAsync(int instanceId, string? prefix = null);
    Task<ConfigurationPO> SetAsync(int instanceId, string key, string? value, string? type = null, string? description = null);
}
