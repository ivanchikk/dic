using FruitsBasket.Model.Fruit;
using FruitsBasket.Orchestrator.Exceptions;

namespace FruitsBasket.Orchestrator.Fruit;

public class FruitOrchestrator(IFruitRepository repository) : IFruitOrchestrator
{
    public async Task<FruitDto> GetByIdAsync(int id)
    {
        return await repository.GetByIdAsync(id) ?? throw new NotFoundException("Fruit not found");
    }

    public async Task<List<FruitDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        return await repository.GetAllAsync(pageNumber, pageSize);
    }

    public async Task<FruitDto> CreateAsync(FruitDto fruit)
    {
        return await repository.CreateAsync(fruit);
    }

    public async Task UpdateAsync(FruitDto fruit)
    {
        if (await repository.GetByIdAsync(fruit.Id) is null)
            throw new NotFoundException("Fruit not found");

        await repository.UpdateAsync(fruit);
    }

    public async Task DeleteAsync(int id)
    {
        var fruit = await repository.GetByIdAsync(id) ?? throw new NotFoundException("Fruit not found");

        await repository.DeleteAsync(fruit);
    }
}