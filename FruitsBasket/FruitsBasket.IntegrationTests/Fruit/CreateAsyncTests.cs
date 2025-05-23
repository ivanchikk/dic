using System.Net;
using FluentAssertions;
using FruitsBasket.Api.Fruit.Contract;
using FruitsBasket.Model.Fruit;

namespace FruitsBasket.IntegrationTests.Fruit;

public class CreateAsyncTests : TestBaseFruit
{
    [Fact]
    public async Task CreateAsync_CreatesEntity_IfDoesNotExist()
    {
        // Arrange
        var expected = new CreateFruit
        {
            Name = "Fruit",
            Weight = 10,
            HarvestDate = DateTime.UtcNow,
        };

        // Act
        var result = await HttpClient.PostAsync(RESOURCE_PATH, JsonContent.Create(expected));

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);

        var actual = await result.Content.ReadFromJsonAsync<FruitDto>();
        actual.Should().BeEquivalentTo(expected);
    }
}