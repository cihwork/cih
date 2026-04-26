using System.Text.Json;
using Eleva.Server.Mcp;
using Microsoft.AspNetCore.Mvc;

namespace Eleva.Server.Controllers;

[ApiController]
[Route("mcp")]
public class McpProtocolController : ControllerBase
{
    private readonly McpServiceRegistry _registry;
    private readonly InstanceContext _instanceContext;

    public McpProtocolController(McpServiceRegistry registry, InstanceContext instanceContext)
    {
        _registry = registry;
        _instanceContext = instanceContext;
    }

    [HttpPost]
    public async Task<IActionResult> Handle([FromBody] JsonElement body)
    {
        var id = body.TryGetProperty("id", out var idEl) ? idEl : default;
        var method = body.TryGetProperty("method", out var mEl) ? mEl.GetString() ?? "" : "";
        var @params = body.TryGetProperty("params", out var pEl) ? pEl : default;

        return method switch
        {
            "initialize" => Ok(JsonRpcResult(id, new
            {
                protocolVersion = "2024-11-05",
                capabilities = new { tools = new { } },
                serverInfo = new { name = "eleva-rh", version = "1.0.0" }
            })),

            "notifications/initialized" => Ok(),

            "tools/list" => Ok(JsonRpcResult(id, new
            {
                tools = _registry.GetAllTools().Values.Select(t => new
                {
                    name = t.Name,
                    description = t.Description,
                    inputSchema = BuildInputSchema(t)
                })
            })),

            "tools/call" => await HandleToolCall(id, @params),

            _ => Ok(JsonRpcError(id, -32601, $"Method not found: {method}"))
        };
    }

    private async Task<IActionResult> HandleToolCall(JsonElement id, JsonElement @params)
    {
        var toolName = @params.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";
        var argsEl = @params.TryGetProperty("arguments", out var a) ? a : default;

        var args = new Dictionary<string, object?>();
        if (argsEl.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in argsEl.EnumerateObject())
                args[prop.Name] = JsonElementToObject(prop.Value);
        }

        var tool = _registry.GetTool(toolName);
        if (tool is null)
            return Ok(JsonRpcError(id, -32602, $"Tool '{toolName}' not found"));

        try
        {
            var result = await tool.Handler(args, HttpContext.RequestServices);
            var json = JsonSerializer.Serialize(result);
            return Ok(JsonRpcResult(id, new
            {
                content = new[] { new { type = "text", text = json } }
            }));
        }
        catch (Exception ex)
        {
            return Ok(JsonRpcError(id, -32603, ex.Message));
        }
    }

    private static object JsonRpcResult(JsonElement id, object result) => new
    {
        jsonrpc = "2.0",
        id = id.ValueKind != JsonValueKind.Undefined ? (object?)id : null,
        result
    };

    private static object JsonRpcError(JsonElement id, int code, string message) => new
    {
        jsonrpc = "2.0",
        id = id.ValueKind != JsonValueKind.Undefined ? (object?)id : null,
        error = new { code, message }
    };

    private static object BuildInputSchema(McpFunction tool)
    {
        if (tool.Parameters == null || tool.Parameters.Count == 0)
            return new { type = "object", properties = new { } };

        var props = tool.Parameters.ToDictionary(
            kvp => kvp.Key,
            kvp => (object)new { type = kvp.Value.Type, description = kvp.Value.Description }
        );

        var required = tool.Parameters
            .Where(kvp => kvp.Value.Required)
            .Select(kvp => kvp.Key)
            .ToArray();

        return new { type = "object", properties = props, required };
    }

    private static object? JsonElementToObject(JsonElement el) => el.ValueKind switch
    {
        JsonValueKind.String => el.GetString(),
        JsonValueKind.Number => el.TryGetInt64(out var l) ? l : el.GetDouble(),
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Null => null,
        JsonValueKind.Array => el.EnumerateArray().Select(JsonElementToObject).ToList(),
        JsonValueKind.Object => el.EnumerateObject().ToDictionary(p => p.Name, p => JsonElementToObject(p.Value)),
        _ => null
    };
}
