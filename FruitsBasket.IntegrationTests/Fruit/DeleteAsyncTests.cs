using System.Net;
using FluentAssertions;
using FruitsBasket.Data.Fruit;
using FruitsBasket.Model.Fruit;
using Microsoft.EntityFrameworkCore;

namespace FruitsBasket.IntegrationTests.Fruit;

public class DeleteAsyncTests : TestBaseFruit
{
    [Fact]
    public async Task DeleteAsync_DeletesEntity_IfExist()
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
        
        await SqlDbContext.Fruits.AddAsync(fruit);
        await SqlDbContext.SaveChangesAsync();
        SqlDbContext.Fruits.Entry(fruit).State = EntityState.Detached;

        // Act
        var result = await HttpClient.DeleteAsync($"{RESOURCE_PATH}/{id}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await SqlDbContext.Fruits.SingleOrDefaultAsync(f => f.Id == id);
        actual.Should().BeNull();

        var actualFruit = await result.Content.ReadFromJsonAsync<FruitDto>();
        actualFruit.Should().BeEquivalentTo(fruit, opt => opt.ExcludingMissingMembers());
    }
}