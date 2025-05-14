using System.Net;
using FluentAssertions;
using FruitsBasket.Model.FruitBasket;

namespace FruitsBasket.IntegrationTests.FruitBasket;

public class DeleteAsyncTests : TestBaseFruitBasket
{
    [Fact]
    public async Task DeleteAsync_Works()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        const int fruitId = 1;
        var filename = $"{basketId:N}_{fruitId}";
        var expected = new FruitBasketDto
        {
            BasketId = basketId,
            FruitId = fruitId,
        };

        await ContainerClient.GetBlobClient(filename).UploadAsync(Stream.Null);

        // Act
        var result = await HttpClient.DeleteAsync($"{RESOURCE_PATH}/{basketId}/{fruitId}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = (await ContainerClient.GetBlobClient(filename).ExistsAsync()).Value;
        actual.Should().Be(false);

        var actualFile = await result.Content.ReadFromJsonAsync<FruitBasketDto>();
        actualFile.Should().BeEquivalentTo(expected);
    }
}