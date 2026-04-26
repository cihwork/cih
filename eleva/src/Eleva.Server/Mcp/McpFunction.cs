namespace Eleva.Server.Mcp;

public class McpFunction
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ToolAnnotation Annotation { get; set; }
    public Func<Dictionary<string, object?>, IServiceProvider, Task<object?>> Handler { get; set; } = null!;
    public Dictionary<string, McpParameter> Parameters { get; set; } = new();
    public string? SchemaJson { get; set; }
}

public class McpParameter
{
    public string Type { get; set; } = "string";
    public string? Description { get; set; }
    public bool Required { get; set; }
    public bool IsMoney { get; set; }
    public string[]? Enum { get; set; }
}
