namespace Eleva.Server.Mcp;

public class McpServiceRegistry
{
    private readonly Dictionary<string, McpFunction> _tools = new();

    public void Register(McpFunction function) => _tools[function.Name] = function;

    public McpFunction? GetTool(string name) => _tools.GetValueOrDefault(name);

    public IReadOnlyDictionary<string, McpFunction> GetAllTools() => _tools;

    public IEnumerable<McpFunction> GetToolsByAnnotation(ToolAnnotation annotation) => _tools.Values.Where(t => t.Annotation == annotation);
}
