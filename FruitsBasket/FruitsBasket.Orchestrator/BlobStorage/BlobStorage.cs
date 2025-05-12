using Azure.Storage.Blobs;
using FruitsBasket.Model.FruitBasket;
using FruitsBasket.Orchestrator.Exceptions;

namespace FruitsBasket.Orchestrator.BlobStorage;

public class BlobStorage(BlobConfiguration configuration) : IBlobStorage
{
    private readonly BlobServiceClient _blobServiceClient = new(configuration.ConnectionString);

    public async Task<List<Guid>> GetAllBasketsAsync()
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(configuration.ContainerName);
        var result = new HashSet<Guid>();

        await foreach (var blob in containerClient.GetBlobsAsync())
        {
            result.Add(Guid.Parse(blob.Name.Split('_')[0]));
        }

        return result.ToList();
    }

    public async Task<List<int>> GetAllFruitsAsync()
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(configuration.ContainerName);
        var result = new HashSet<int>();

        await foreach (var blob in containerClient.GetBlobsAsync())
        {
            result.Add(int.Parse(blob.Name.Split('_')[1]));
        }

        return result.ToList();
    }

    public async Task<List<int>> GetAllFruitsByBasketIdAsync(Guid basketId)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(configuration.ContainerName);
        var result = new List<int>();

        await foreach (var blob in containerClient.GetBlobsAsync())
        {
            if (blob.Name.StartsWith($"{basketId:N}"))
            {
                result.Add(int.Parse(blob.Name.Split('_')[1]));
            }
        }

        if (result.Count == 0)
            throw new NotFoundException("Basket not found");

        return result;
    }

    public async Task CreateFileAsync(string fileName)
    {
        await _blobServiceClient
            .GetBlobContainerClient(configuration.ContainerName)
            .GetBlobClient(fileName)
            .UploadAsync(new MemoryStream());
    }

    public async Task<bool> ContainsFileAsync(string fileName)
    {
        return await _blobServiceClient
            .GetBlobContainerClient(configuration.ContainerName)
            .GetBlobClient(fileName)
            .ExistsAsync();
    }

    public async Task<FruitBasketDto> DeleteFileAsync(string fileName)
    {
        await _blobServiceClient
            .GetBlobContainerClient(configuration.ContainerName)
            .GetBlobClient(fileName)
            .DeleteAsync();

        return new FruitBasketDto
        {
            BasketId = Guid.ParseExact(fileName.Split('_')[0], "N"),
            FruitId = int.Parse(fileName.Split('_')[1])
        };
    }
}