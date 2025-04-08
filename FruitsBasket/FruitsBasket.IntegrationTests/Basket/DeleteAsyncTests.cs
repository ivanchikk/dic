using System.Net;
using FluentAssertions;
using FruitsBasket.Data.Basket;
using Microsoft.EntityFrameworkCore;

namespace FruitsBasket.IntegrationTests.Basket;

public class DeleteAsyncTests : TestBaseBasket
{
    [Fact]
    public async Task DeleteAsync_DeletesEntity_IfExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        var basket = new BasketDao
        {
            Id = id,
            Name = "Basket",
            FruitsWeight = 10,
            LastFruitAdded = DateTime.UtcNow,
        };
        CosmosDbContext.Baskets.Add(basket);
        await CosmosDbContext.SaveChangesAsync();
        CosmosDbContext.Baskets.Entry(basket).State = EntityState.Detached;

        // Act
        var result = await HttpClient.DeleteAsync($"{RESOURCE_PATH}/{id}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await CosmosDbContext.Baskets.SingleOrDefaultAsync(f => f.Id == id);
        actual.Should().BeNull();

        var actualBasket = await result.Content.ReadFromJsonAsync<BasketDao>();
        actualBasket.Should().BeEquivalentTo(basket);
    }
}