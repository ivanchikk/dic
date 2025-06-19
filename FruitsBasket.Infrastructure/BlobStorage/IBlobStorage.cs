using FruitsBasket.Model.FruitBasket;

namespace FruitsBasket.Infrastructure.BlobStorage;

public interface IBlobStorage
{
    Task<List<Guid>> GetAllBasketsAsync();
    Task<List<int>> GetAllFruitsAsync();
    Task<List<int>> GetAllFruitsByBasketIdAsync(Guid basketId);
    Task CreateFileAsync(string filename);
    Task<bool> ContainsFileAsync(string filename);
    Task<FruitBasketDto> DeleteFileAsync(string filename);
}