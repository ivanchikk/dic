namespace FruitsBasket.Model.Fruit;

public interface IFruitRepository
{
    Task<FruitDto?> GetByIdAsync(int id);
    Task<List<FruitDto>> GetAllAsync(int pageNumber, int pageSize);
    Task<FruitDto> CreateAsync(FruitDto fruit);
    Task<FruitDto> UpdateAsync(FruitDto fruit);
    Task<FruitDto> DeleteAsync(FruitDto fruit);
    Task<int> CountAsync();
}