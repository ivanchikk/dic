using System.Net;
using FluentAssertions;
using FruitsBasket.Data.Basket;
using FruitsBasket.Data.Fruit;
using FruitsBasket.Model.FruitBasket;

namespace FruitsBasket.IntegrationTests.FruitBasket;

public class CreateAsyncTests : TestBaseFruitBasket
{
    [Fact]
    public async Task CreateAsync_CreatesEntity_IfDoesNotExist()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        const int fruitId = 1;
        var expected = new FruitBasketDto
        {
            BasketId = basketId,
            FruitId = fruitId,
        };
        var basket = new BasketDao
        {
            Id = basketId,
            Name = "Basket",
            FruitsWeight = 10,
            LastFruitAdded = DateTime.UtcNow,
        };
        var fruit = new FruitDao
        {
            Id = fruitId,
            Name = "Fruit",
            Weight = 10,
            HarvestDate = DateTime.UtcNow,
        };

        await SqlDbContext.Fruits.AddAsync(fruit);
        await SqlDbContext.SaveChangesAsync();
        await CosmosDbContext.Baskets.AddAsync(basket);
        await CosmosDbContext.SaveChangesAsync();

        // Act
        var result = await HttpClient.PostAsync($"{RESOURCE_PATH}/{basketId}/{fruitId}", null);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await result.Content.ReadFromJsonAsync<FruitBasketDto>();
        actual.Should().BeEquivalentTo(expected);
    }
}