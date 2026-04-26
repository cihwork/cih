using Eleva.Server.Mcp;
using Eleva.Services.Services.History;

namespace Eleva.Server.Mcp.Services;

public class AuditMcpService : IMcpService
{
    public void RegisterTools(McpServiceRegistry registry)
    {
        registry.Register(new McpFunction
        {
            Name = "audit_log_list",
            Description = "Lista logs de auditoria",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "skip", new McpParameter { Type = "integer", Description = "Itens a ignorar", Required = false } },
                { "take", new McpParameter { Type = "integer", Description = "Itens a retornar", Required = false } },
                { "entityType", new McpParameter { Type = "string", Description = "Tipo de entidade", Required = false } },
                { "action", new McpParameter { Type = "string", Description = "Acao", Required = false } },
                { "userId", new McpParameter { Type = "integer", Description = "Usuario", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IAuditLogService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.ListAsync(instanceId, McpArgs.Int(args, "skip", 0), McpArgs.Int(args, "take", 50), McpArgs.StrOrNull(args, "entityType"), McpArgs.StrOrNull(args, "action"), McpArgs.IntOrNull(args, "userId"));
            }
        });

        registry.Register(new McpFunction
        {
            Name = "audit_log_search",
            Description = "Busca logs de auditoria",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "term", new McpParameter { Type = "string", Description = "Termo", Required = true } },
                { "take", new McpParameter { Type = "integer", Description = "Quantidade", Required = false } }
            },
            Handler = async (args, sp) =>
            {
                var service = sp.GetRequiredService<IAuditLogService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await service.SearchAsync(instanceId, McpArgs.Str(args, "term", string.Empty), McpArgs.Int(args, "take", 50));
            }
        });
    }
}
