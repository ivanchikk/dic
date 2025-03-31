using System.Net;
using FluentAssertions;
using FruitsBasket.Data.Fruit;

namespace FruitsBasket.IntegrationTests.Fruit;

public class GetByIdAsyncTests : TestBase
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

        DbContext.Fruits.Add(expected);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await HttpClient.GetAsync($"{API_PATH}/{id}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await result.Content.ReadFromJsonAsync<FruitDao>();
        actual.Should().BeEquivalentTo(expected);
    }
}