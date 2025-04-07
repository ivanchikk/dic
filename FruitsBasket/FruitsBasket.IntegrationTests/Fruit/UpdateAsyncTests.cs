using System.Net;
using FluentAssertions;
using FruitsBasket.Data.Fruit;
using Microsoft.EntityFrameworkCore;

namespace FruitsBasket.IntegrationTests.Fruit;

public class UpdateAsyncTests : TestBaseFruit
{
    [Fact]
    public async Task UpdateAsync_UpdateEntity_IfExists()
    {
        // Arrange
        const int id = 1;
        var fruit = new FruitDao
        {
            Id = id,
            Name = "Fruit",
            Weight = 10,
            HarvestDate = DateTime.UtcNow,
        };

        var expected = new FruitDao
        {
            Id = id,
            Name = "NewFruit",
            Weight = 20,
            HarvestDate = DateTime.UtcNow.AddDays(-1),
        };

        SqlDbContext.Fruits.Add(fruit);
        await SqlDbContext.SaveChangesAsync();
        SqlDbContext.Fruits.Entry(fruit).State = EntityState.Detached;

        // Act
        var result = await HttpClient.PutAsync($"{RESOURCE_PATH}/{id}", JsonContent.Create(expected));

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await result.Content.ReadFromJsonAsync<FruitDao>();
        actual.Should().BeEquivalentTo(expected);
    }
}