using Azure.Storage.Blobs;
using DotNet.Testcontainers.Builders;
using FruitsBasket.Infrastructure.BlobStorage;
using Testcontainers.Azurite;

namespace FruitsBasket.IntegrationTests;

public class BlobStorageFixture : IAsyncLifetime
{
    private readonly AzuriteContainer AzuriteContainer = new AzuriteBuilder()
        .WithImage("mcr.microsoft.com/azure-storage/azurite")
        .WithName($"azurite-test-{Guid.NewGuid():N}")
        .WithPortBinding(10000, true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(10000))
        .WithCleanUp(true)
        .Build();

    public BlobServiceClient ServiceClient = null!;
    public BlobContainerClient ContainerClient = null!;
    public BlobConfiguration Configuration { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await AzuriteContainer.StartAsync();

        Configuration = new BlobConfiguration
        {
            ConnectionString = AzuriteContainer.GetConnectionString(),
            ContainerName = $"test-container-{Guid.NewGuid():N}",
        };

        ServiceClient = new BlobServiceClient(Configuration.ConnectionString);
        ContainerClient = ServiceClient.GetBlobContainerClient(Configuration.ContainerName);

        await ContainerClient.CreateIfNotExistsAsync();
    }

    public async Task DisposeAsync()
    {
        await ContainerClient.DeleteIfExistsAsync();
        await AzuriteContainer.DisposeAsync();
    }
}