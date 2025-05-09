using FruitsBasket.Data;
using Microsoft.AspNetCore.TestHost;

namespace FruitsBasket.IntegrationTests;

public class TestBase : IDisposable
{
    private IHostBuilder _server = null!;
    private IHost _host = null!;
    protected SqlDbContext SqlDbContext = null!;
    protected CosmosDbContext CosmosDbContext = null!;
    protected readonly HttpClient HttpClient;
    protected const string API_PATH = "api/v1";

    protected TestBase()
    {
        HttpClient = InitTestServer().GetClient();
    }

    public void Dispose()
    {
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
}