using FruitsBasket.Infrastructure.BlobStorage;
using FruitsBasket.Model.Basket;
using FruitsBasket.Model.Fruit;
using FruitsBasket.Model.FruitBasket;
using FruitsBasket.Orchestrator.Exceptions;

namespace FruitsBasket.Orchestrator.FruitBasket;

public class FruitBasketOrchestrator(
    IBasketOrchestrator basketOrchestrator,
    IFruitOrchestrator fruitOrchestrator,
    IBlobStorage fruitBasketStorage)
    : IFruitBasketOrchestrator
{
    public async Task<List<Guid>> GetAllBasketsAsync()
    {
        return await fruitBasketStorage.GetAllBasketsAsync();
    }

    public async Task<List<int>> GetAllFruitsAsync()
    {
        return await fruitBasketStorage.GetAllFruitsAsync();
    }

    public async Task<List<int>> GetAllFruitsByBasketIdAsync(Guid basketId)
    {
        await basketOrchestrator.GetByIdAsync(basketId);

        var result = await fruitBasketStorage.GetAllFruitsByBasketIdAsync(basketId);

        if (result.Count == 0)
            throw new NotFoundException("Basket not found");

        return result;
    }

    public async Task<FruitBasketDto> CreateAsync(Guid basketId, int fruitId)
    {
        await fruitOrchestrator.GetByIdAsync(fruitId);
        await basketOrchestrator.GetByIdAsync(basketId);

        var filename = $"{basketId:N}_{fruitId}";
        var exists = await fruitBasketStorage.ContainsFileAsync(filename);

        if (exists)
            throw new InvalidOperationException("This fruit is already in a basket");

        var fruits = await fruitBasketStorage.GetAllFruitsAsync();
        if (fruits.Contains(fruitId))
            throw new InvalidOperationException("This fruit is already in another basket");

        await fruitBasketStorage.CreateFileAsync(filename);

        return new FruitBasketDto
        {
            BasketId = basketId,
            FruitId = fruitId
        };
    }

    public async Task<FruitBasketDto> DeleteAsync(Guid basketId, int fruitId)
    {
        var filename = $"{basketId:N}_{fruitId}";
        var exists = await fruitBasketStorage.ContainsFileAsync(filename);

        if (!exists)
            throw new NotFoundException("File not found");

        return await fruitBasketStorage.DeleteFileAsync(filename);
    }
}