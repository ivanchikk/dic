using AutoMapper;
using AutoMapper.QueryableExtensions;
using FruitsBasket.Model.Fruit;
using Microsoft.EntityFrameworkCore;

namespace FruitsBasket.Data.Fruit;

public class FruitRepository(SqlDbContext context, IMapper mapper) : IFruitRepository
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

    public async Task<FruitDto> UpdateAsync(FruitDto fruit)
    {
        var entity = mapper.Map<FruitDao>(fruit);
        var result = context.Fruits.Update(entity);

        await context.SaveChangesAsync();

        return mapper.Map<FruitDto>(result.Entity);
    }

    public async Task<FruitDto> DeleteAsync(FruitDto fruit)
    {
        var entity = mapper.Map<FruitDao>(fruit);
        var result = context.Fruits.Remove(entity);

        await context.SaveChangesAsync();

        return mapper.Map<FruitDto>(result.Entity);
    }
}