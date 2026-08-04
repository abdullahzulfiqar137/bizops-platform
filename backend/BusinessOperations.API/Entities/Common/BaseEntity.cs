namespace BusinessOperations.API.Entities.Common;

public abstract class BaseEntity : ITenantOwned
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
}
