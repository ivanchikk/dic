using FruitsBasket.Model.Fruit;

namespace FruitsBasket.Orchestrator.Fruit;

public class FruitOrchestrator(IFruitRepository repository) : IFruitOrchestrator
{
    public async Task<FruitDto> GetByIdAsync(int id)
    {
        return await repository.GetByIdAsync(id) ?? throw new Exception("Fruit not found");
    }

    public async Task<List<FruitDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            throw new ArgumentException("Page number and page size must be greater than 0");

        if (pageSize > 100)
            throw new ArgumentException("Page size must be less than 101");

        return await repository.GetAllAsync(pageNumber, pageSize);
    }

    public async Task<FruitDto> CreateAsync(FruitDto fruit)
    {
        return await repository.CreateAsync(fruit);
    }

    public async Task UpdateAsync(FruitDto fruit)
    {
        if (await repository.GetByIdAsync(fruit.Id) is null)
            throw new Exception("Fruit not found");

        await repository.UpdateAsync(fruit);
    }

    public async Task DeleteAsync(int id)
    {
        var fruit = await repository.GetByIdAsync(id) ?? throw new Exception("Fruit not found");

        await repository.DeleteAsync(fruit);
    }
}