using System.Net;
using FluentAssertions;
using FruitsBasket.Data.Fruit;
using FruitsBasket.Model.Fruit;

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

        await SqlDbContext.Fruits.AddAsync(expected);
        await SqlDbContext.SaveChangesAsync();

        // Act
        var result = await HttpClient.GetAsync($"{RESOURCE_PATH}/{id}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await result.Content.ReadFromJsonAsync<FruitDto>();
        actual.Should().BeEquivalentTo(expected, opt => opt.ExcludingMissingMembers());
    }
}