using FruitsBasket.Model.FruitBasket;

namespace FruitsBasket.Orchestrator.BlobStorage;

public interface IBlobStorage
{
    Task<List<Guid>> GetAllBasketsAsync();
    Task<List<int>> GetAllFruitsAsync();
    Task<List<int>> GetAllFruitsByBasketIdAsync(Guid basketId);
    Task CreateFileAsync(string fileName);
    Task<bool> ContainsFileAsync(string fileName);
    Task<FruitBasketDto> DeleteFileAsync(string fileName);
}