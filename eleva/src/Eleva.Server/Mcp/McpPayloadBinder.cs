using System.Text.Json;

namespace Eleva.Server.Mcp;

internal static class McpPayloadBinder
{
    public static T? Read<T>(Dictionary<string, object?> args, string key)
    {
        var raw = McpArgs.ObjOrNull(args, key);
        if (raw is null)
            return default;

        if (raw is T typed)
            return typed;

        return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(raw));
    }
}
