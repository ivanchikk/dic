using AutoMapper;
using FruitsBasket.Api.Fruit.Contract;
using FruitsBasket.Model.Fruit;
using Microsoft.AspNetCore.Mvc;

namespace FruitsBasket.Api.Fruit;

[ApiController]
[Route("[controller]")]
public class FruitController(IFruitOrchestrator orchestrator, IMapper mapper) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var result = await orchestrator.GetByIdAsync(id);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        var result = await orchestrator.GetAllAsync(pageNumber, pageSize);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync(CreateFruit fruit)
    {
        var result = await orchestrator.CreateAsync(mapper.Map<FruitDto>(fruit));

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutAsync(int id, CreateFruit fruit)
    {
        var entity = mapper.Map<FruitDto>(fruit);
        entity.Id = id;

        await orchestrator.UpdateAsync(entity);

        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await orchestrator.DeleteAsync(id);

        return NoContent();
    }
}