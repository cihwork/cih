using Eleva.Server.Mcp;
using Microsoft.AspNetCore.Mvc;

namespace Eleva.Server.Controllers;

[ApiController]
[Route("mcp/tools")]
public class McpController : ControllerBase
{
    private readonly McpServiceRegistry _registry;
    private readonly InstanceContext _instanceContext;

    public McpController(McpServiceRegistry registry, InstanceContext instanceContext)
    {
        _registry = registry;
        _instanceContext = instanceContext;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var tools = _registry.GetAllTools().Values.Select(tool => new
        {
            tool.Name,
            tool.Description,
            Annotation = tool.Annotation.ToString(),
            tool.Parameters
        });

        return Ok(tools);
    }

    [HttpPost("call")]
    public async Task<IActionResult> Call([FromBody] McpCallRequest request)
    {
        try
        {
            if (_instanceContext.InstanceId <= 0)
                return StatusCode(500, new { message = "Instance context is missing." });

            var tool = _registry.GetTool(request.Name);
            if (tool is null)
                return StatusCode(500, new { message = $"Tool '{request.Name}' not found." });

            var arguments = request.Arguments ?? new Dictionary<string, object?>();
            if (!McpSchemaValidator.Validate(tool.SchemaJson, arguments))
                return StatusCode(500, new { message = $"Invalid payload for '{request.Name}'." });

            var result = await tool.Handler(arguments, HttpContext.RequestServices);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}

public sealed class McpCallRequest
{
    public string Name { get; set; } = null!;
    public Dictionary<string, object?>? Arguments { get; set; }
}
