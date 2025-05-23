using System.Net;
using FluentAssertions;
using FruitsBasket.Data.Basket;

namespace FruitsBasket.IntegrationTests.FruitBasket;

public class GetAllFruitsByBasketIdAsyncTests(BlobStorageFixture blobStorageFixture) : TestBaseFruitBasket(blobStorageFixture)
{
    [Fact]
    public async Task GetAllFruitsByBasketIdAsync_Works()
    {
        // Arrange
        var expected = new List<int> { 1, 2, 3 };
        var basketId = Guid.NewGuid();
        var basket = new BasketDao
        {
            Id = basketId,
            Name = "Basket",
            FruitsWeight = 10,
            LastFruitAdded = DateTime.UtcNow,
        };

        await CosmosDbContext.Baskets.AddAsync(basket);
        await CosmosDbContext.SaveChangesAsync();

        foreach (var id in expected)
        {
            await ContainerClient.GetBlobClient($"{basketId:N}_{id}").UploadAsync(Stream.Null);
            await ContainerClient.GetBlobClient($"{Guid.NewGuid():N}_{id}").UploadAsync(Stream.Null);
        }

        // Act
        var result = await HttpClient.GetAsync($"{RESOURCE_PATH}/baskets/{basketId}/fruits");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await result.Content.ReadFromJsonAsync<IEnumerable<int>>();
        actual.Should().BeEquivalentTo(expected);
    }
}