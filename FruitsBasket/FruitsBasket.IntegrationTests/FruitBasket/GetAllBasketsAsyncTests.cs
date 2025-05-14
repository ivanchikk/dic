using System.Net;
using FluentAssertions;

namespace FruitsBasket.IntegrationTests.FruitBasket;

public class GetAllBasketsAsyncTests : TestBaseFruitBasket
{
    [Fact]
    public async Task GetAllBasketsAsync_Works()
    {
        // Arrange
        var expected = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        foreach (var (id, index) in expected.Select((id, i) => (id, i)))
        {
            await ContainerClient.GetBlobClient($"{id:N}_{index + 1}").UploadAsync(Stream.Null);
        }

        // Act
        var result = await HttpClient.GetAsync($"{RESOURCE_PATH}/baskets");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await result.Content.ReadFromJsonAsync<IEnumerable<Guid>>();
        actual.Should().BeEquivalentTo(expected);
    }
}