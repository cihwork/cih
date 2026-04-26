using Eleva.Server.Mcp;
using Eleva.Services.Services.Core;

namespace Eleva.Server.Mcp.Services;

public class AuthMcpService : IMcpService
{
    public void RegisterTools(McpServiceRegistry registry)
    {
        registry.Register(new McpFunction
        {
            Name = "auth_login",
            Description = "Login do usuario",
            Annotation = ToolAnnotation.ExternalAction,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "email", new McpParameter { Type = "string", Description = "E-mail", Required = true } },
                { "password", new McpParameter { Type = "string", Description = "Senha", Required = true } },
                { "instanceSlug", new McpParameter { Type = "string", Description = "Slug da instancia", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var email = McpArgs.Str(args, "email", "");
                var password = McpArgs.Str(args, "password", "");
                var instanceSlug = McpArgs.Str(args, "instanceSlug", "");
                var (user, token) = await sp.GetRequiredService<IAuthService>().LoginAsync(email, password, instanceSlug);
                return new { userId = user.Id, token, instanceId = user.InstanceId };
            }
        });

        registry.Register(new McpFunction
        {
            Name = "auth_refresh",
            Description = "Renova token",
            Annotation = ToolAnnotation.ExternalAction,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "refreshToken", new McpParameter { Type = "string", Description = "Refresh token", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var token = McpArgs.Str(args, "refreshToken", "");
                var newToken = await sp.GetRequiredService<IAuthService>().RefreshTokenAsync(token);
                return new { token = newToken };
            }
        });

        registry.Register(new McpFunction
        {
            Name = "instance_create",
            Description = "Cria instancia",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "name", new McpParameter { Type = "string", Description = "Nome", Required = true } },
                { "slug", new McpParameter { Type = "string", Description = "Slug", Required = true } },
                { "adminEmail", new McpParameter { Type = "string", Description = "E-mail do admin", Required = true } },
                { "adminPassword", new McpParameter { Type = "string", Description = "Senha do admin", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var name = McpArgs.Str(args, "name", "");
                var slug = McpArgs.Str(args, "slug", "");
                var adminEmail = McpArgs.Str(args, "adminEmail", "");
                var adminPassword = McpArgs.Str(args, "adminPassword", "");
                var instance = await sp.GetRequiredService<IAuthService>().CreateInstanceAsync(name, slug, adminEmail, adminPassword);
                return new { instanceId = instance.Id, name = instance.Name, slug = instance.Slug };
            }
        });

        registry.Register(new McpFunction
        {
            Name = "instance_list",
            Description = "Lista instancias",
            Annotation = ToolAnnotation.ReadOnly,
            Handler = async (args, sp) =>
            {
                var instances = await sp.GetRequiredService<IAuthService>().ListInstancesAsync();
                return instances.Select(i => new { i.Id, i.Name, i.Slug, i.Status, i.IsActive }).ToList();
            }
        });
    }
}
