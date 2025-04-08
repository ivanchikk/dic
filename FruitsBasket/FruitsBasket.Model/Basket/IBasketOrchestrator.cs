namespace FruitsBasket.Model.Basket;

public interface IBasketOrchestrator
{
    Task<BasketDto> GetByIdAsync(Guid id);
    Task<List<BasketDto>> GetAllAsync(int pageNumber, int pageSize);
    Task<BasketDto> CreateAsync(BasketDto Basket);
    Task<BasketDto> UpdateAsync(BasketDto Basket);
    Task<BasketDto> DeleteAsync(Guid id);
}