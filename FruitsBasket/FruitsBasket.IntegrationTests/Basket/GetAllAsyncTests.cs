using System.Net;
using FluentAssertions;
using FruitsBasket.Data.Basket;

namespace FruitsBasket.IntegrationTests.Basket;

public class GetAllAsyncTests : TestBaseBasket
{
    [Fact]
    public async Task GetAllAsync_ReturnEntities_IfExist()
    {
        // Arrange
        const int expectedCount = 3;
        var expected = Enumerable.Range(1, expectedCount)
            .Select(i => new BasketDao
            {
                Id = Guid.NewGuid(),
                Name = $"Basket{i}",
                FruitsWeight = 10 * i,
                LastFruitAdded = DateTime.UtcNow.AddDays(-i),
            }).ToList();

        CosmosDbContext.Baskets.AddRange(expected);
        await CosmosDbContext.SaveChangesAsync();

        // Act
        var result = await HttpClient.GetAsync($"{RESOURCE_PATH}?pageNumber=1&pageSize={expectedCount}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await result.Content.ReadFromJsonAsync<IEnumerable<BasketDao>>();
        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, -1)]
    [InlineData(-1, 0)]
    [InlineData(-1, -1)]
    [InlineData(1, 101)]
    [InlineData(101, 102)]
    public async Task GetAllAsync_ReturnsBadRequest_IfPageNumberOrPageSizeInvalid(int pageNumber, int pageSize)
    {
        // Arrange

        // Act
        var result = await HttpClient.GetAsync($"{RESOURCE_PATH}?pageNumber={pageNumber}&pageSize={pageSize}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}