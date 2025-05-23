namespace FruitsBasket.Infrastructure.BlobStorage;

public class BlobConfiguration
{
    public virtual string? ConnectionString { get; set; }
    public virtual string? ContainerName { get; set; }
}