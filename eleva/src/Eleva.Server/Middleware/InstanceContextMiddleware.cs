namespace Eleva.Server.Middleware;

using Eleva.Server.Mcp;

public class InstanceContextMiddleware
{
    private readonly RequestDelegate _next;

    public InstanceContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, InstanceContext instanceContext)
    {
        if (context.Request.Headers.TryGetValue("X-Instance-Id", out var instanceIdHeader) && int.TryParse(instanceIdHeader, out var instanceId))
            instanceContext.InstanceId = instanceId;

        if (context.Request.Headers.TryGetValue("X-User-Id", out var userIdHeader) && int.TryParse(userIdHeader, out var userId))
            instanceContext.UserId = userId;

        await _next(context);
    }
}
