# Eleva — Como criar um novo MCP Service

## Passo a passo

### 1. Interface de serviço
Crie em `src/Eleva.Services/Services/Xxx/IXxxService.cs`:
```csharp
namespace Eleva.Services.Services.Xxx;
public interface IXxxService
{
    Task<XxxPO> GetAsync(int instanceId, int id);
    // demais métodos
}
```

### 2. Implementação stub
Crie em `src/Eleva.Services/Services/Xxx/XxxService.cs`:
```csharp
namespace Eleva.Services.Services.Xxx;
using Eleva.Services.Data;
using Eleva.Shared.Interfaces;

public class XxxService : IXxxService
{
    private readonly AppDbContext _db;
    private readonly ICurrentInstanceAccessor _instanceAccessor;

    public XxxService(AppDbContext db, ICurrentInstanceAccessor instanceAccessor)
    {
        _db = db;
        _instanceAccessor = instanceAccessor;
    }

    public Task<XxxPO> GetAsync(int instanceId, int id) => throw new NotImplementedException();
}
```

### 3. Registrar no Program.cs
```csharp
builder.Services.AddScoped<IXxxService, XxxService>();
```

### 4. MCP Service
Crie em `src/Eleva.Server/Mcp/Services/XxxMcpService.cs`:
```csharp
namespace Eleva.Server.Mcp.Services;
using Eleva.Services.Services.Xxx;

public class XxxMcpService : IMcpService
{
    public void RegisterTools(McpServiceRegistry registry)
    {
        registry.Register(new McpFunction
        {
            Name = "xxx_get",
            Description = "Busca xxx por id",
            Annotation = ToolAnnotation.ReadOnly,
            Parameters = new Dictionary<string, McpParameter>
            {
                { "id", new McpParameter { Type = "integer", Description = "ID do registro", Required = true } }
            },
            Handler = async (args, sp) =>
            {
                var svc = sp.GetRequiredService<IXxxService>();
                var instanceId = sp.GetRequiredService<InstanceContext>().InstanceId;
                return await svc.GetAsync(instanceId, McpArgs.Int(args, "id", 0));
            }
        });
    }
}
```

### 5. Registrar MCP Service no Program.cs
```csharp
builder.Services.AddScoped<IMcpService, XxxMcpService>();
```

### 6. Build
```bash
dotnet build Eleva.sln
```
Deve passar com 0 errors.
