using Azure.Storage.Blobs;
using FruitsBasket.Infrastructure.BlobStorage;

namespace FruitsBasket.IntegrationTests.FruitBasket;

public abstract class TestBaseFruitBasket(BlobStorageFixture blobStorageFixture)
    : TestBase, IClassFixture<BlobStorageFixture>
{
    protected const string RESOURCE_PATH = API_PATH;

    private readonly BlobConfiguration BlobConfiguration = blobStorageFixture.Configuration;
    protected readonly BlobServiceClient ServiceClient = blobStorageFixture.ServiceClient;
    protected readonly BlobContainerClient ContainerClient = blobStorageFixture.ContainerClient;

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(BlobConfiguration);
    }
}