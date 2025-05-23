namespace FruitsBasket.Model.FruitBasket;

public interface IFruitBasketOrchestrator
{
    Task<List<Guid>> GetAllBasketsAsync();
    Task<List<int>> GetAllFruitsAsync();
    Task<List<int>> GetAllFruitsByBasketIdAsync(Guid basketId);
    Task<FruitBasketDto> CreateAsync(Guid basketId, int fruitId);
    Task<FruitBasketDto> DeleteAsync(Guid basketId, int fruitId);
}