using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using AutoMapper;
using FruitsBasket.Api.Fruit.Contract;
using FruitsBasket.Model.Fruit;
using Microsoft.AspNetCore.Mvc;

namespace FruitsBasket.Api.Fruit;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class FruitsController(IFruitOrchestrator orchestrator, IMapper mapper) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var result = await orchestrator.GetByIdAsync(id);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(
        [Range(1, 1_000_000_000)] int pageNumber = 1,
        [Range(1, 100)] int pageSize = 10)
    {
        var result = await orchestrator.GetAllAsync(pageNumber, pageSize);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync(CreateFruit fruit)
    {
        var result = await orchestrator.CreateAsync(mapper.Map<FruitDto>(fruit));

        return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutAsync(int id, CreateFruit fruit)
    {
        var entity = mapper.Map<FruitDto>(fruit);
        entity.Id = id;

        var result = await orchestrator.UpdateAsync(entity);

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await orchestrator.DeleteAsync(id);

        return Ok(result);
    }
}