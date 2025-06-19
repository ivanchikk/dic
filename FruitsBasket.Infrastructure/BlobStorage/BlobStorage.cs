using Azure.Storage.Blobs;
using FruitsBasket.Model.FruitBasket;
using Microsoft.Extensions.Options;

namespace FruitsBasket.Infrastructure.BlobStorage;

public class BlobStorage(IOptions<BlobStorageConfiguration> options) : IBlobStorage
{
    private readonly BlobContainerClient _containerClient =
        new(options.Value.ConnectionString, options.Value.ContainerName);

    private static (Guid basketId, int fruitId) ParseFilename(string filename)
    {
        var parts = filename.Split('_');
        return (Guid.ParseExact(parts[0], "N"), int.Parse(parts[1]));
    }

    public async Task<List<Guid>> GetAllBasketsAsync()
    {
        var result = new HashSet<Guid>();

        await foreach (var blob in _containerClient.GetBlobsAsync())
        {
            result.Add(ParseFilename(blob.Name).basketId);
        }

        return result.ToList();
    }

    public async Task<List<int>> GetAllFruitsAsync()
    {
        var result = new HashSet<int>();

        await foreach (var blob in _containerClient.GetBlobsAsync())
        {
            result.Add(ParseFilename(blob.Name).fruitId);
        }

        return result.ToList();
    }

    public async Task<List<int>> GetAllFruitsByBasketIdAsync(Guid basketId)
    {
        var result = new List<int>();

        await foreach (var blob in _containerClient.GetBlobsAsync())
        {
            if (blob.Name.StartsWith($"{basketId:N}"))
            {
                result.Add(ParseFilename(blob.Name).fruitId);
            }
        }

        return result;
    }

    public async Task CreateFileAsync(string filename)
    {
        await _containerClient
            .GetBlobClient(filename)
            .UploadAsync(Stream.Null);
    }

    public async Task<bool> ContainsFileAsync(string filename)
    {
        return await _containerClient
            .GetBlobClient(filename)
            .ExistsAsync();
    }

    public async Task<FruitBasketDto> DeleteFileAsync(string filename)
    {
        await _containerClient
            .GetBlobClient(filename)
            .DeleteAsync();

        return new FruitBasketDto
        {
            BasketId = ParseFilename(filename).basketId,
            FruitId = ParseFilename(filename).fruitId,
        };
    }
}