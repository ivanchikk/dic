namespace FruitsBasket.Infrastructure.BlobStorage;

public class BlobStorageConfiguration
{
    public virtual string ConnectionString { get; set; } = null!;
    public virtual string ContainerName { get; set; } = null!;
}