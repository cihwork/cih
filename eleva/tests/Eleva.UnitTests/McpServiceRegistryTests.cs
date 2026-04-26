namespace Eleva.UnitTests;

using Eleva.Server.Mcp;

public class McpServiceRegistryTests
{
    [Fact]
    public void Register_AddsToolToRegistry()
    {
        var registry = new McpServiceRegistry();
        var fn = new McpFunction { Name = "test_tool", Description = "Test", Annotation = ToolAnnotation.ReadOnly, Handler = (_, _) => Task.FromResult<object?>(null) };
        registry.Register(fn);
        Assert.NotNull(registry.GetTool("test_tool"));
    }

    [Fact]
    public void GetAllTools_ReturnsAllRegistered()
    {
        var registry = new McpServiceRegistry();
        registry.Register(new McpFunction { Name = "tool_a", Description = "A", Annotation = ToolAnnotation.ReadOnly, Handler = (_, _) => Task.FromResult<object?>(null) });
        registry.Register(new McpFunction { Name = "tool_b", Description = "B", Annotation = ToolAnnotation.Mutating, Handler = (_, _) => Task.FromResult<object?>(null) });
        Assert.Equal(2, registry.GetAllTools().Count);
    }

    [Fact]
    public void GetTool_ReturnsNull_WhenNotFound()
    {
        var registry = new McpServiceRegistry();
        Assert.Null(registry.GetTool("nonexistent"));
    }
}
