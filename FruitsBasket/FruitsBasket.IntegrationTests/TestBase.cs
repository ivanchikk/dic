using FruitsBasket.Data;
using Microsoft.AspNetCore.TestHost;

namespace FruitsBasket.IntegrationTests;

public abstract class TestBase : IAsyncLifetime
{
    protected const string API_PATH = "api/v1";

    private IHost _host = null!;
    private IHostBuilder _server = null!;
    protected HttpClient HttpClient = null!;
    protected SqlDbContext SqlDbContext = null!;
    protected CosmosDbContext CosmosDbContext = null!;

    public async Task InitializeAsync()
    {
        _server = new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.UseStartup<TestStartup>();
                webHost.ConfigureServices(ConfigureServices);
            });

        _host = await _server.StartAsync();
        HttpClient = _host.GetTestClient();

        SqlDbContext = _host.Services.GetRequiredService<SqlDbContext>();
        CosmosDbContext = _host.Services.GetRequiredService<CosmosDbContext>();
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
    }

    public async Task DisposeAsync()
    {
        await SqlDbContext.DisposeAsync();
        await CosmosDbContext.DisposeAsync();

        _host.Dispose();
    }
}