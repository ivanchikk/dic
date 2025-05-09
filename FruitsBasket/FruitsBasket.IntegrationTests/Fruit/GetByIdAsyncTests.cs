using System.Net;
using FluentAssertions;
using FruitsBasket.Data.Fruit;

namespace FruitsBasket.IntegrationTests.Fruit;

public class GetByIdAsyncTests : TestBaseFruit
{
    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_IfExists()
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

        SqlDbContext.Fruits.Add(expected);
        await SqlDbContext.SaveChangesAsync();

        // Act
        var result = await HttpClient.GetAsync($"{RESOURCE_PATH}/{id}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await result.Content.ReadFromJsonAsync<FruitDao>();
        actual.Should().BeEquivalentTo(expected);
    }
}