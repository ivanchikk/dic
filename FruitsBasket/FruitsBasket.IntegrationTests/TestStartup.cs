using EntityFrameworkCore.Testing.Common.Helpers;
using EntityFrameworkCore.Testing.Moq.Helpers;
using FruitsBasket.Api;
using FruitsBasket.Data;
using FruitsBasket.Infrastructure.BlobStorage;
using Microsoft.EntityFrameworkCore;

namespace FruitsBasket.IntegrationTests;

public class TestStartup(IConfiguration configuration) : Startup(configuration)
{
    protected override void ConfigureDb(IServiceCollection services)
    {
        var sqlDbContext = ConfigureDb<SqlDbContext>().MockedDbContext;
        var cosmosDbContext = ConfigureDb<CosmosDbContext>().MockedDbContext;

        services.AddSingleton(sqlDbContext);
        services.AddSingleton(cosmosDbContext);
    }

    private static IMockedDbContextBuilder<T> ConfigureDb<T>() where T : DbContext
    {
        var options = new DbContextOptionsBuilder<T>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var context = (T)Activator.CreateInstance(typeof(T), options)!;

        return new MockedDbContextBuilder<T>()
            .UseDbContext(context)
            .UseConstructorWithParameters(options);
    }

    protected override void ConfigureBlobStorage(IServiceCollection services)
    {
        var blobConfig = new BlobConfiguration
        {
            ConnectionString = "UseDevelopmentStorage=true;",
            ContainerName = $"test-container-{Guid.NewGuid():N}"
        };
        services.AddSingleton(blobConfig);
    }
}