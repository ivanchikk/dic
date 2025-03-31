using FruitsBasket.Model.Fruit;
using FruitsBasket.Orchestrator.Exceptions;

namespace FruitsBasket.Orchestrator.Fruit;

public class FruitOrchestrator(IFruitRepository repository) : IFruitOrchestrator
{
    public async Task<FruitDto> GetByIdAsync(int id)
    {
        return await repository.GetByIdAsync(id) ?? throw new NotFoundException("Fruit not found");
    }

    public async Task<List<FruitDto>> GetAllAsync(int pageNumber, int pageSize)
    {
        return await repository.GetAllAsync(pageNumber, pageSize);
    }

    public async Task<FruitDto> CreateAsync(FruitDto fruit)
    {
        return await repository.CreateAsync(fruit);
    }

    public async Task<FruitDto> UpdateAsync(FruitDto fruit)
    {
        var entity = await repository.GetByIdAsync(fruit.Id);

        if (entity is null)
            throw new NotFoundException("Fruit not found");

        entity.Name = fruit.Name;
        entity.Weight = fruit.Weight;
        entity.HarvestDate = fruit.HarvestDate;

        return await repository.UpdateAsync(entity);
    }

    public async Task<FruitDto> DeleteAsync(int id)
    {
        var fruit = await repository.GetByIdAsync(id) ?? throw new NotFoundException("Fruit not found");

        return await repository.DeleteAsync(fruit);
    }
}