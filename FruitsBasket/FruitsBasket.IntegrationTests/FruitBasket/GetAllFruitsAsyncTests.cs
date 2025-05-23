using System.Net;
using FluentAssertions;

namespace FruitsBasket.IntegrationTests.FruitBasket;

public class GetAllFruitsAsyncTests(BlobStorageFixture blobStorageFixture) : TestBaseFruitBasket(blobStorageFixture)
{
    [Fact]
    public async Task GetAllFruitsAsync_Works()
    {
        // Arrange
        var expected = new List<int> { 1, 2, 3 };

        foreach (var id in expected)
        {
            await ContainerClient.GetBlobClient($"{Guid.NewGuid():N}_{id}").UploadAsync(Stream.Null);
        }

        // Act
        var result = await HttpClient.GetAsync($"{RESOURCE_PATH}/fruitIds");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await result.Content.ReadFromJsonAsync<IEnumerable<int>>();
        actual.Should().BeEquivalentTo(expected);
    }
}