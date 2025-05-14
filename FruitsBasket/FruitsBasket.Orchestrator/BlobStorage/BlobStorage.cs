using Azure.Storage.Blobs;
using FruitsBasket.Model.FruitBasket;
using FruitsBasket.Orchestrator.Exceptions;

namespace FruitsBasket.Orchestrator.BlobStorage;

public class BlobStorage(BlobConfiguration configuration) : IBlobStorage
{
    private readonly BlobContainerClient _containerClient =
        new(configuration.ConnectionString, configuration.ContainerName);

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

        if (result.Count == 0)
            throw new NotFoundException("Basket not found");

        return result;
    }

    public async Task CreateFileAsync(string fileName)
    {
        await _containerClient
            .GetBlobClient(fileName)
            .UploadAsync(Stream.Null);
    }

    public async Task<bool> ContainsFileAsync(string fileName)
    {
        return await _containerClient
            .GetBlobClient(fileName)
            .ExistsAsync();
    }

    public async Task<FruitBasketDto> DeleteFileAsync(string fileName)
    {
        await _containerClient
            .GetBlobClient(fileName)
            .DeleteAsync();

        return new FruitBasketDto
        {
            BasketId = ParseFilename(fileName).basketId,
            FruitId = ParseFilename(fileName).fruitId,
        };
    }
}