using AutoMapper;
using AutoMapper.QueryableExtensions;
using FruitsBasket.Model.Basket;
using Microsoft.EntityFrameworkCore;

namespace FruitsBasket.Data.Basket;

public class BasketRepository(CosmosDbContext context, IMapper mapper) : IBasketRepository
{
    public async Task<BasketDto?> GetByIdAsync(Guid id)
    {
        var result = await context.Baskets
            .AsNoTracking()
            .SingleOrDefaultAsync(fruit => fruit.Id == id);

        return mapper.Map<BasketDto>(result);
    }

    public async Task<List<BasketDto>> GetAllAsync(int pageNumber, int pageSize)
    {
        var result = await context.Baskets
            .AsNoTracking()
            .OrderBy(f => f.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<BasketDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        return result;
    }

    public async Task<BasketDto> CreateAsync(BasketDto basket)
    {
        var result = await context.Baskets.AddAsync(mapper.Map<BasketDao>(basket));

        await context.SaveChangesAsync();

        return mapper.Map<BasketDto>(result.Entity);
    }

    public async Task<BasketDto> UpdateAsync(BasketDto basket)
    {
        var entity = mapper.Map<BasketDao>(basket);
        var result = context.Baskets.Update(entity);

        await context.SaveChangesAsync();

        return mapper.Map<BasketDto>(result.Entity);
    }

    public async Task<BasketDto> DeleteAsync(BasketDto basket)
    {
        var entity = mapper.Map<BasketDao>(basket);
        var result = context.Baskets.Remove(entity);

        await context.SaveChangesAsync();

        return mapper.Map<BasketDto>(result.Entity);
    }
}