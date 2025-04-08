using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using AutoMapper;
using FruitsBasket.Api.Basket.Contract;
using FruitsBasket.Model.Basket;
using Microsoft.AspNetCore.Mvc;

namespace FruitsBasket.Api.Basket;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BasketsController(IBasketOrchestrator orchestrator, IMapper mapper) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
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
    public async Task<IActionResult> PostAsync(CreateBasket basket)
    {
        var result = await orchestrator.CreateAsync(mapper.Map<BasketDto>(basket));

        return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutAsync(Guid id, CreateBasket basket)
    {
        var entity = mapper.Map<BasketDto>(basket);
        entity.Id = id;

        var result = await orchestrator.UpdateAsync(entity);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await orchestrator.DeleteAsync(id);

        return Ok(result);
    }
}