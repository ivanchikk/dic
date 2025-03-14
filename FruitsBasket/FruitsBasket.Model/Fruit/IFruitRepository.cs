namespace FruitsBasket.Model.Fruit;

public interface IFruitRepository
{
    Task<FruitDto?> GetByIdAsync(int id);
    Task<List<FruitDto>> GetAllAsync(int pageNumber, int pageSize);
    Task<FruitDto> CreateAsync(FruitDto fruit);
    Task UpdateAsync(FruitDto fruit);
    Task DeleteAsync(FruitDto fruit);
}