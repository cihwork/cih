namespace Eleva.Shared.Interfaces;

public abstract class BaseEntity : IInstanceBaseEntity
{
    public int Id { get; set; }
    public int InstanceId { get; set; }
    public string? ReferenceCode { get; set; }
}
