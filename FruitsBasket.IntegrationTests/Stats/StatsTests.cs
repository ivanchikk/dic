using System.Net;
using System.Reflection;
using Azure.Messaging.ServiceBus;
using FluentAssertions;
using FruitsBasket.Infrastructure.MessageBroker;
using Moq;

namespace FruitsBasket.IntegrationTests.Stats;

public class StatsTests : TestBaseStats
{
    [Fact]
    public async Task GetAllBasketStatsAsync_Works()
    {
        // Arrange
        var expected = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        StoreMock
            .Setup(sm => sm.GetAll())
            .Returns(expected);

        // Act
        var result = await HttpClient.GetAsync($"{RESOURCE_PATH}/baskets");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await result.Content.ReadFromJsonAsync<IEnumerable<Guid>>();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateBasketAsync_CallsAddMethod()
    {
        // Arrange
        var expectedBasketId = Guid.NewGuid();
        var messageBody = expectedBasketId.ToString();

        var clientMock = new Mock<ServiceBusClient>();
        var receiverMock = new Mock<ServiceBusReceiver>();
        var processorMock = new Mock<ServiceBusProcessor>();
        var queueNameProviderMock = new Mock<IQueueNameProvider>();

        clientMock
            .Setup(c => c.CreateProcessor(It.IsAny<string>()))
            .Returns(processorMock.Object);

        var subscriber = new BasketStatsSubscriber(
            clientMock.Object,
            StoreMock.Object,
            queueNameProviderMock.Object
        );

        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(BinaryData.FromString(messageBody));

        var args = new ProcessMessageEventArgs(
            message,
            receiverMock.Object,
            CancellationToken.None
        );

        // Act
        var methodInfo = typeof(BasketStatsSubscriber).GetMethod(
            "ProcessMessageAsync",
            BindingFlags.NonPublic | BindingFlags.Instance
        )!;

        await (Task)methodInfo.Invoke(subscriber, [args])!;

        // Assert
        StoreMock.Verify(sm => sm.Add(expectedBasketId), Times.Once);
    }
}