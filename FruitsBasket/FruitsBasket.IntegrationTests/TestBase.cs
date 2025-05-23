using Azure.Storage.Blobs;
using FruitsBasket.Data;
using FruitsBasket.Infrastructure.BlobStorage;
using Microsoft.AspNetCore.TestHost;

namespace FruitsBasket.IntegrationTests;

public class TestBase : IDisposable
{
    protected const string API_PATH = "api/v1";
    private readonly BlobConfiguration _blobConfig;
    private IHost _host = null!;
    private IHostBuilder _server = null!;
    protected readonly HttpClient HttpClient;
    protected readonly BlobServiceClient ServiceClient;
    protected readonly BlobContainerClient ContainerClient;
    protected SqlDbContext SqlDbContext = null!;
    protected CosmosDbContext CosmosDbContext = null!;

    protected TestBase()
    {
        HttpClient = InitTestServer().GetClient();
        _blobConfig = _host.Services.GetRequiredService<BlobConfiguration>();
        ServiceClient = new BlobServiceClient(_blobConfig.ConnectionString);
        ContainerClient = ServiceClient.GetBlobContainerClient(_blobConfig.ContainerName);

        CleanBlobContainerAsync().GetAwaiter().GetResult();
        SetupBlobContainerAsync().GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        CleanBlobContainerAsync().GetAwaiter().GetResult();

        _host.StopAsync().GetAwaiter().GetResult();
        _host.Dispose();
        SqlDbContext.Dispose();
        CosmosDbContext.Dispose();
    }

    private HttpClient GetClient()
    {
        _host = _server.Start();

        SqlDbContext = _host.Services.GetRequiredService<SqlDbContext>();
        CosmosDbContext = _host.Services.GetRequiredService<CosmosDbContext>();
        return _host.GetTestClient();
    }

    private TestBase InitTestServer()
    {
        _server = new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.UseStartup<TestStartup>();
            });

        return this;
    }

    private async Task SetupBlobContainerAsync()
    {
        if (!await ContainerClient.ExistsAsync())
            await ServiceClient.CreateBlobContainerAsync(_blobConfig.ContainerName);
    }

    private async Task CleanBlobContainerAsync()
    {
        if (await ContainerClient.ExistsAsync())
            await ServiceClient.DeleteBlobContainerAsync(_blobConfig.ContainerName);
    }
}