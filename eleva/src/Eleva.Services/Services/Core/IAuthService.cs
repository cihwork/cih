namespace Eleva.Services.Services.Core;

using Eleva.Shared.PersistenceObjects.Core;

public interface IAuthService
{
    Task<(UserPO user, string token)> LoginAsync(string email, string password, string instanceSlug);
    Task<string> RefreshTokenAsync(string token);
    Task<InstancePO> CreateInstanceAsync(string name, string slug, string adminEmail, string adminPassword);
    Task<IReadOnlyList<InstancePO>> ListInstancesAsync();
}
