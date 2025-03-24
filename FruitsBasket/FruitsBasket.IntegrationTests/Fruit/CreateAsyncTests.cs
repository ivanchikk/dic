using System.Net;
using FluentAssertions;
using FruitsBasket.Data.Fruit;

namespace FruitsBasket.IntegrationTests.Fruit;

public class CreateAsyncTests : TestBase
{
    [Fact]
    public async Task CreateAsync_CreatesEntity_IfDoesNotExist()
    {
        // Arrange
        const int id = 1;
        var expected = new FruitDao
        {
            Id = id,
            Name = "Fruit",
            Weight = 10,
            HarvestDate = DateTime.UtcNow,
        };

        // Act
        var result = await HttpClient.PostAsync("Fruit", JsonContent.Create(expected));

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);

        var actual = await result.Content.ReadFromJsonAsync<FruitDao>();
        actual.Should().BeEquivalentTo(expected);
    }
}