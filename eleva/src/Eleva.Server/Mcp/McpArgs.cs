using System.Globalization;
using System.Text.Json;

namespace Eleva.Server.Mcp;

public static class McpArgs
{
    public static string? StrOrNull(Dictionary<string, object?> args, string key)
    {
        if (!args.TryGetValue(key, out var value) || value is null)
            return null;

        return value switch
        {
            string s => s,
            JsonElement element => element.ValueKind == JsonValueKind.Null || element.ValueKind == JsonValueKind.Undefined
                ? null
                : element.ValueKind == JsonValueKind.String
                    ? element.GetString()
                    : element.ToString(),
            _ => Convert.ToString(value, CultureInfo.InvariantCulture)
        };
    }

    public static string Str(Dictionary<string, object?> args, string key, string defaultValue)
        => StrOrNull(args, key) ?? defaultValue;

    public static int? IntOrNull(Dictionary<string, object?> args, string key)
    {
        if (!args.TryGetValue(key, out var value) || value is null)
            return null;

        return value switch
        {
            int i => i,
            long l when l is >= int.MinValue and <= int.MaxValue => (int)l,
            decimal d when d >= int.MinValue && d <= int.MaxValue => (int)d,
            double d when d >= int.MinValue && d <= int.MaxValue => (int)d,
            JsonElement element when element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out var i) => i,
            JsonElement element when element.ValueKind == JsonValueKind.String && int.TryParse(element.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) => parsed,
            string s when int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) => parsed,
            _ => null
        };
    }

    public static int Int(Dictionary<string, object?> args, string key, int defaultValue)
        => IntOrNull(args, key) ?? defaultValue;

    public static bool Bool(Dictionary<string, object?> args, string key)
    {
        if (!args.TryGetValue(key, out var value) || value is null)
            return false;

        return value switch
        {
            bool b => b,
            JsonElement element when element.ValueKind == JsonValueKind.True => true,
            JsonElement element when element.ValueKind == JsonValueKind.False => false,
            JsonElement element when element.ValueKind == JsonValueKind.String && bool.TryParse(element.GetString(), out var parsed) => parsed,
            string s when bool.TryParse(s, out var parsed) => parsed,
            int i => i != 0,
            long l => l != 0,
            _ => false
        };
    }

    public static decimal? DecimalOrNull(Dictionary<string, object?> args, string key)
    {
        if (!args.TryGetValue(key, out var value) || value is null)
            return null;

        return value switch
        {
            decimal d => d,
            double d => (decimal)d,
            float f => (decimal)f,
            int i => i,
            long l => l,
            JsonElement element when element.ValueKind == JsonValueKind.Number && element.TryGetDecimal(out var parsed) => parsed,
            JsonElement element when element.ValueKind == JsonValueKind.String && decimal.TryParse(element.GetString(), NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed) => parsed,
            string s when decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed) => parsed,
            _ => null
        };
    }

    public static DateOnly? DateOrNull(Dictionary<string, object?> args, string key)
    {
        if (!args.TryGetValue(key, out var value) || value is null)
            return null;

        return value switch
        {
            DateOnly dateOnly => dateOnly,
            DateTime dateTime => DateOnly.FromDateTime(dateTime),
            DateTimeOffset dateTimeOffset => DateOnly.FromDateTime(dateTimeOffset.UtcDateTime),
            JsonElement element when element.ValueKind == JsonValueKind.String && DateOnly.TryParse(element.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed) => parsed,
            JsonElement element when element.ValueKind == JsonValueKind.String && DateTime.TryParse(element.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed) => DateOnly.FromDateTime(parsed),
            string s when DateOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed) => parsed,
            string s when DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed) => DateOnly.FromDateTime(parsed),
            _ => null
        };
    }

    public static object? ObjOrNull(Dictionary<string, object?> args, string key)
    {
        if (!args.TryGetValue(key, out var value) || value is null)
            return null;

        return value switch
        {
            JsonElement element => ConvertJsonElement(element),
            _ => value
        };
    }

    private static object? ConvertJsonElement(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Null or JsonValueKind.Undefined => null,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Number when element.TryGetInt32(out var i) => i,
            JsonValueKind.Number when element.TryGetInt64(out var l) => l,
            JsonValueKind.Number when element.TryGetDecimal(out var d) => d,
            JsonValueKind.String when DateTime.TryParse(element.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dateTime) => dateTime,
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Array => element.EnumerateArray().Select(ConvertJsonElement).ToList(),
            JsonValueKind.Object => element.EnumerateObject().ToDictionary(p => p.Name, p => ConvertJsonElement(p.Value)),
            _ => element.ToString()
        };
    }
}
