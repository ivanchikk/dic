using FruitsBasket.Infrastructure.MessageBroker;
using Moq;

namespace FruitsBasket.IntegrationTests.Stats;

public class TestBaseStats : TestBase
{
    protected const string RESOURCE_PATH = API_PATH + "/basket-stats";

    protected readonly Mock<IStatsStore<Guid>> StoreMock = new();

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(StoreMock.Object);
    }
}