using System.Net;
using FluentAssertions;
using FruitsBasket.Api.Basket.Contract;
using FruitsBasket.Model.Basket;

namespace FruitsBasket.IntegrationTests.Basket;

public class CreateAsyncTests : TestBaseBasket
{
    [Fact]
    public async Task CreateAsync_CreatesEntity_IfDoesNotExist()
    {
        // Arrange
        var expected = new CreateBasket
        {
            Name = "Basket",
            FruitsWeight = 10,
            LastFruitAdded = DateTime.UtcNow,
        };

        // Act
        var result = await HttpClient.PostAsync(RESOURCE_PATH, JsonContent.Create(expected));

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);

        var actual = await result.Content.ReadFromJsonAsync<BasketDto>();
        actual.Should().BeEquivalentTo(expected);
    }
}