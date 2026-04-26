using Eleva.Server.Mcp;
using Eleva.Services.Services.Core;

namespace Eleva.Server.Mcp.Services;

public class ConfigMcpService : IMcpService
{
    public void RegisterTools(McpServiceRegistry registry)
    {
        registry.Register(new McpFunction
        {
            Name = "config_get",
            Description = "Busca configuracao",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "key", new McpParameter { Type = "string", Description = "Chave", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IConfigService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.GetAsync(instanceId, McpArgs.Str(args, "key", string.Empty));
            }
        });

        registry.Register(new McpFunction
        {
            Name = "config_set",
            Description = "Define configuracao",
            Annotation = ToolAnnotation.Mutating,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "key", new McpParameter { Type = "string", Description = "Chave", Required = true } },
                { "value", new McpParameter { Type = "string", Description = "Valor", Required = false } },
                { "type", new McpParameter { Type = "string", Description = "Tipo", Required = false } },
                { "description", new McpParameter { Type = "string", Description = "Descricao", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IConfigService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.SetAsync(instanceId, McpArgs.Str(args, "key", string.Empty), McpArgs.StrOrNull(args, "value"), McpArgs.StrOrNull(args, "type"), McpArgs.StrOrNull(args, "description"));
            }
        });

        registry.Register(new McpFunction
        {
            Name = "config_list",
            Description = "Lista configuracoes",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "prefix", new McpParameter { Type = "string", Description = "Prefixo", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IConfigService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.ListAsync(instanceId, McpArgs.StrOrNull(args, "prefix"));
            }
        });
    }
}
