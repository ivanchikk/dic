namespace FruitsBasket.Model.Fruit;

public interface IFruitOrchestrator
{
    Task<FruitDto> GetByIdAsync(int id);
    Task<List<FruitDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
    Task<FruitDto> CreateAsync(FruitDto fruit);
    Task<FruitDto> UpdateAsync(FruitDto fruit);
    Task<FruitDto> DeleteAsync(int id);
}