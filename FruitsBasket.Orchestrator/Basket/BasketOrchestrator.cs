using FruitsBasket.Infrastructure.MessageBroker;
using FruitsBasket.Model.Basket;
using FruitsBasket.Orchestrator.Exceptions;

namespace FruitsBasket.Orchestrator.Basket;

public class BasketOrchestrator(IBasketRepository repository, IPublisher<Guid> publisher) : IBasketOrchestrator
{
    public async Task<BasketDto> GetByIdAsync(Guid id)
    {
        return await repository.GetByIdAsync(id) ?? throw new NotFoundException("Basket not found");
    }

    public async Task<List<BasketDto>> GetAllAsync(int pageNumber, int pageSize)
    {
        return await repository.GetAllAsync(pageNumber, pageSize);
    }

    public async Task<BasketDto> CreateAsync(BasketDto basket)
    {
        var result = await repository.CreateAsync(basket);

        await publisher.PublishAsync(result.Id);

        return result;
    }

    public async Task<BasketDto> UpdateAsync(BasketDto basket)
    {
        var entity = await repository.GetByIdAsync(basket.Id);

        if (entity is null)
            throw new NotFoundException("Basket not found");

        entity.Name = basket.Name;
        entity.FruitsWeight = basket.FruitsWeight;
        entity.LastFruitAdded = basket.LastFruitAdded;

        return await repository.UpdateAsync(entity);
    }

    public async Task<BasketDto> DeleteAsync(Guid id)
    {
        var entity = await repository.GetByIdAsync(id) ?? throw new NotFoundException("Basket not found");

        return await repository.DeleteAsync(entity);
    }
}