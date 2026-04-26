using Eleva.Server.Mcp;

namespace Eleva.Server.Mcp.Services;

public class SystemMcpService : IMcpService
{
    public void RegisterTools(McpServiceRegistry registry)
    {
        registry.Register(new McpFunction
        {
            Name = "system_version",
            Description = "Versao do sistema",
            Annotation = ToolAnnotation.ReadOnly,
            Handler = (_, _) => Task.FromResult<object?>(new
            {
                version = typeof(SystemMcpService).Assembly.GetName().Version?.ToString() ?? "unknown"
            })
        });

        registry.Register(new McpFunction
        {
            Name = "system_health",
            Description = "Health do sistema",
            Annotation = ToolAnnotation.ReadOnly,
            Handler = (_, _) => Task.FromResult<object?>(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow
            })
        });
    }
}
