using System.Net;
using FluentAssertions;
using FruitsBasket.Data.Fruit;
using Microsoft.EntityFrameworkCore;

namespace FruitsBasket.IntegrationTests.Fruit;

public class DeleteAsyncTests : TestBase
{
    [Fact]
    public async Task CreateAsync_CreatesEntity_IfDoesNotExist()
    {
        // Arrange
        const int id = 1;
        var fruit = new FruitDao
        {
            Id = id,
            Name = $"Fruit",
            Weight = 10,
            HarvestDate = DateTime.UtcNow,
        };
        DbContext.Fruits.Add(fruit);
        await DbContext.SaveChangesAsync();
        DbContext.Fruits.Entry(fruit).State = EntityState.Detached;

        // Act
        var result = await HttpClient.DeleteAsync($"Fruit/{id}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var actual = await DbContext.Fruits.SingleOrDefaultAsync(f => f.Id == id);
        actual.Should().BeNull();
    }
}