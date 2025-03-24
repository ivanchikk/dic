using AutoMapper;
using AutoMapper.QueryableExtensions;
using FruitsBasket.Model.Fruit;
using Microsoft.EntityFrameworkCore;

namespace FruitsBasket.Data.Fruit;

public class FruitRepository(FruitDbContext context, IMapper mapper) : IFruitRepository
{
    public async Task<FruitDto?> GetByIdAsync(int id)
    {
        var result = await context.Fruits
            .AsNoTracking()
            .SingleOrDefaultAsync(fruit => fruit.Id == id);

        return mapper.Map<FruitDto>(result);
    }

    public Task<List<FruitDto>> GetAllAsync(int pageNumber, int pageSize)
    {
        var result = context.Fruits
            .AsNoTracking()
            .OrderBy(f => f.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<FruitDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        return result;
    }

    public async Task<FruitDto> CreateAsync(FruitDto fruit)
    {
        var result = await context.Fruits.AddAsync(mapper.Map<FruitDao>(fruit));

        await context.SaveChangesAsync();

        return mapper.Map<FruitDto>(result.Entity);
    }

    public async Task UpdateAsync(FruitDto fruit)
    {
        context.Fruits.Update(mapper.Map<FruitDao>(fruit));

        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(FruitDto fruit)
    {
        context.Fruits.Remove(mapper.Map<FruitDao>(fruit));

        await context.SaveChangesAsync();
    }
}