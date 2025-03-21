using System.ComponentModel.DataAnnotations;
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
    public async Task<IActionResult> GetById(int id)
    {
        var result = await orchestrator.GetByIdAsync(id);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [Range(1, 1_000_000_000)] int pageNumber = 1,
        [Range(1, 100)] int pageSize = 10)
    {
        var result = await orchestrator.GetAllAsync(pageNumber, pageSize);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateFruit fruit)
    {
        var result = await orchestrator.CreateAsync(mapper.Map<FruitDto>(fruit));

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, CreateFruit fruit)
    {
        var entity = mapper.Map<FruitDto>(fruit);
        entity.Id = id;

        await orchestrator.UpdateAsync(entity);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await orchestrator.DeleteAsync(id);

        return NoContent();
    }
}