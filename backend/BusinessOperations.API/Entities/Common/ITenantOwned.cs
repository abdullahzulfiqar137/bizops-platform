namespace BusinessOperations.API.Entities.Common;

public interface ITenantOwned
{
    Guid TenantId { get; set; }
}

/// <summary>Marks shared SaaS catalog data that must not be tenant-filtered.</summary>
public interface IGlobalEntity;
