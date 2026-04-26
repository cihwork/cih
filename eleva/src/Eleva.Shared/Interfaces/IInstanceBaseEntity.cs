namespace Eleva.Shared.Interfaces;

public interface IInstanceBaseEntity
{
    int Id { get; set; }
    int InstanceId { get; set; }
    string? ReferenceCode { get; set; }
}
