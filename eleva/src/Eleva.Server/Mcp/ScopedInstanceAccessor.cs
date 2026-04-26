using Eleva.Shared.Interfaces;

namespace Eleva.Server.Mcp;

public sealed class ScopedInstanceAccessor : ICurrentInstanceAccessor
{
    private readonly InstanceContext _instanceContext;

    public ScopedInstanceAccessor(InstanceContext instanceContext)
    {
        _instanceContext = instanceContext;
    }

    public int? GetInstanceId() => _instanceContext.InstanceId > 0 ? _instanceContext.InstanceId : null;

    public int? GetUserId() => _instanceContext.UserId;
}
