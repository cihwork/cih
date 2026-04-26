namespace Eleva.Services.Services.Core;

using System.Security.Cryptography;
using System.Text;
using Eleva.Services.Data;
using Eleva.Shared.Enums;
using Eleva.Shared.PersistenceObjects.Core;
using Microsoft.EntityFrameworkCore;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;

    public AuthService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(UserPO user, string token)> LoginAsync(string email, string password, string instanceSlug)
    {
        var instance = await _db.Instances.IgnoreQueryFilters()
            .FirstOrDefaultAsync(i => i.Slug == instanceSlug)
            ?? throw new UnauthorizedAccessException("Instância não encontrada.");

        var user = await _db.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.InstanceId == instance.Id)
            ?? throw new UnauthorizedAccessException("Credenciais inválidas.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Usuário inativo.");

        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
        if (user.PasswordHash != hash)
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        user.LastLoginAt = DateTime.UtcNow;
        _db.DisableInstanceIdInterceptor = true;
        await _db.SaveChangesAsync();
        _db.DisableInstanceIdInterceptor = false;

        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        return (user, token);
    }

    public Task<string> RefreshTokenAsync(string token)
        => Task.FromResult(Convert.ToBase64String(Guid.NewGuid().ToByteArray()));

    public async Task<InstancePO> CreateInstanceAsync(string name, string slug, string adminEmail, string adminPassword)
    {
        var slugExists = await _db.Instances.IgnoreQueryFilters().AnyAsync(i => i.Slug == slug);
        if (slugExists)
            throw new InvalidOperationException($"Slug '{slug}' já está em uso.");

        var instance = new InstancePO
        {
            Name = name,
            Slug = slug,
            McpApiKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
            Plan = SubscriptionPlan.Free,
            Status = InstanceStatus.Active,
            IsActive = true,
            MaxUsers = 10,
            CreatedAt = DateTime.UtcNow
        };

        _db.DisableInstanceIdInterceptor = true;
        _db.Instances.Add(instance);
        await _db.SaveChangesAsync();

        var adminHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(adminPassword)));
        var adminUser = new UserPO
        {
            Name = adminEmail.Split('@')[0],
            Email = adminEmail,
            PasswordHash = adminHash,
            Role = UserRole.Admin,
            IsActive = true,
            InstanceId = instance.Id,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(adminUser);
        await _db.SaveChangesAsync();
        _db.DisableInstanceIdInterceptor = false;

        return instance;
    }

    public async Task<IReadOnlyList<InstancePO>> ListInstancesAsync()
        => await _db.Instances.IgnoreQueryFilters().OrderBy(i => i.Name).ToListAsync();
}
