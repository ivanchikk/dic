using System.Net;
using FluentAssertions;
using FruitsBasket.Data.Basket;
using FruitsBasket.Model.Basket;

namespace FruitsBasket.IntegrationTests.Basket;

public class GetByIdAsyncTests : TestBaseBasket
{
    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_IfExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expected = new BasketDao
        {
            Id = id,
            Name = "Basket",
            FruitsWeight = 10,
            LastFruitAdded = DateTime.UtcNow,
        };

        await CosmosDbContext.Baskets.AddAsync(expected);
        await CosmosDbContext.SaveChangesAsync();

        // Act
        var result = await HttpClient.GetAsync($"{RESOURCE_PATH}/{id}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await result.Content.ReadFromJsonAsync<BasketDto>();
        actual.Should().BeEquivalentTo(expected, opt => opt.ExcludingMissingMembers());
    }
}