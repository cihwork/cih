namespace Eleva.Shared.Interfaces;

public interface ICurrentInstanceAccessor
{
    int? GetInstanceId();
    int? GetUserId();
}
