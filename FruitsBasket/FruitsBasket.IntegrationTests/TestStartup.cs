using EntityFrameworkCore.Testing.Common.Helpers;
using EntityFrameworkCore.Testing.Moq.Helpers;
using FruitsBasket.Api;
using FruitsBasket.Data;
using FruitsBasket.Infrastructure.MessageBroker;
using Microsoft.EntityFrameworkCore;
using Moq;

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

    protected override void ConfigureEdgeServices(IServiceCollection services)
    {
        var publisher = new Mock<IPublisher<Guid>>().Object;
        services.AddSingleton(publisher);
    }
}