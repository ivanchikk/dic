using System.Net;
using FluentAssertions;
using FruitsBasket.Data.Fruit;

namespace FruitsBasket.IntegrationTests.Fruit;

public class GetAllAsyncTests : TestBase
{
    [Fact]
    public async Task GetAllAsync_ReturnEntities_IfExist()
    {
        // Arrange
        const int expectedCount = 3;
        var expected = Enumerable.Range(1, 3).Select(i => new FruitDao
        {
            Id = i,
            Name = $"Fruit{i}",
            Weight = 10 * i,
            HarvestDate = DateTime.UtcNow.AddDays(-i),
        }).ToList();

        DbContext.Fruits.AddRange(expected);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await HttpClient.GetAsync("Fruit");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await result.Content.ReadFromJsonAsync<IEnumerable<FruitDao>>();
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
        var result = await HttpClient.GetAsync($"Fruit?pageNumber={pageNumber}&pageSize={pageSize}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}