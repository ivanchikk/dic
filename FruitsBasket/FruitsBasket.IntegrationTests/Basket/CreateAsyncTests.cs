using System.Net;
using FluentAssertions;
using FruitsBasket.Data.Basket;

namespace FruitsBasket.IntegrationTests.Basket;

public class CreateAsyncTests : TestBaseBasket
{
    [Fact]
    public async Task CreateAsync_CreatesEntity_IfDoesNotExist()
    {
        // Arrange
        var expected = new BasketDao
        {
            Name = "Basket",
            FruitsWeight = 10,
            LastFruitAdded = DateTime.UtcNow,
        };

        // Act
        var result = await HttpClient.PostAsync(RESOURCE_PATH, JsonContent.Create(expected));

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);

        var actual = await result.Content.ReadFromJsonAsync<BasketDao>();
        actual.Should().BeEquivalentTo(expected, opt => opt.Excluding(x => x.Id));
    }
}