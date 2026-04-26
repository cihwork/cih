namespace Eleva.Server.Accessors;

using Eleva.Shared.Interfaces;

public sealed class JobInstanceAccessor : ICurrentInstanceAccessor
{
    private static readonly AsyncLocal<int?> _instanceId = new();
    private static readonly AsyncLocal<int?> _userId = new();

    public int? GetInstanceId() => _instanceId.Value > 0 ? _instanceId.Value : null;
    public int? GetUserId() => _userId.Value;

    public static void SetInstance(int instanceId, int? userId = null)
    {
        _instanceId.Value = instanceId;
        _userId.Value = userId;
    }

    public static void Clear()
    {
        _instanceId.Value = null;
        _userId.Value = null;
    }
}
