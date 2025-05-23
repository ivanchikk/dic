using System.Net;
using FluentAssertions;
using FruitsBasket.Data.Basket;
using FruitsBasket.Model.Basket;
using Microsoft.EntityFrameworkCore;

namespace FruitsBasket.IntegrationTests.Basket;

public class UpdateAsyncTests : TestBaseBasket
{
    [Fact]
    public async Task UpdateAsync_UpdateEntity_IfExists()
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
        var expected = new BasketDao
        {
            Id = id,
            Name = "NewBasket",
            FruitsWeight = 20,
            LastFruitAdded = DateTime.UtcNow.AddDays(-1),
        };

        await CosmosDbContext.Baskets.AddAsync(basket);
        await CosmosDbContext.SaveChangesAsync();
        CosmosDbContext.Baskets.Entry(basket).State = EntityState.Detached;

        // Act
        var result = await HttpClient.PutAsync($"{RESOURCE_PATH}/{id}", JsonContent.Create(expected));

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await result.Content.ReadFromJsonAsync<BasketDto>();
        actual.Should().BeEquivalentTo(expected, opt => opt.ExcludingMissingMembers());
    }
}