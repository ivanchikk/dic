namespace FruitsBasket.Infrastructure.BlobStorage;

public class BlobConfiguration
{
    public virtual string ConnectionString { get; set; } = null!;
    public virtual string ContainerName { get; set; } = null!;
}